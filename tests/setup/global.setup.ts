import {
  existsSync,
  mkdirSync,
  readFileSync,
  rmSync,
  writeFileSync,
} from "node:fs";
import path from "node:path";
import { spawnSync } from "node:child_process";
import { test as setup } from "@playwright/test";
import { GenericContainer, Wait } from "testcontainers";
import { spawnDetachedProcess, stopProcessTree } from "./api-process";

const ARTIFACTS_DIR = path.resolve(process.cwd(), ".playwright");
const STATE_FILE = path.join(ARTIFACTS_DIR, "testcontainers-state.json");
const DB_URI_FILE = path.join(ARTIFACTS_DIR, "db-uri.txt");
const API_LOG_FILE = path.join(ARTIFACTS_DIR, "api.log");
const WEB_LOG_FILE = path.join(ARTIFACTS_DIR, "web.log");

const API_URL = "http://127.0.0.1:3001/health";
const WEB_URL = "http://127.0.0.1:5173";

type SetupState = {
  containerId?: string;
  dbUri?: string;
  apiPid?: number;
  webPid?: number;
};

function persistStateFile(state: SetupState) {
  writeFileSync(STATE_FILE, JSON.stringify(state, null, 2), "utf8");
}

async function waitForUrl(url: string, timeoutMs: number) {
  const started = Date.now();

  while (Date.now() - started < timeoutMs) {
    try {
      const response = await fetch(url);
      if (response.ok) {
        return;
      }
    } catch {
      // Service is not up yet.
    }

    await new Promise((resolve) => setTimeout(resolve, 500));
  }

  throw new Error(`Timed out waiting for ${url}`);
}

function readStateFile(): SetupState | undefined {
  if (!existsSync(STATE_FILE)) {
    return undefined;
  }

  return JSON.parse(readFileSync(STATE_FILE, "utf8")) as SetupState;
}

async function cleanupState(state?: SetupState): Promise<void> {
  if (!state) {
    return;
  }

  await Promise.all([
    stopProcessTree(state.apiPid),
    stopProcessTree(state.webPid),
  ]);

  if (state.containerId) {
    const result = spawnSync("docker", ["rm", "-f", state.containerId], {
      encoding: "utf8",
    });

    if (result.status !== 0) {
      const stderr = result.stderr ?? "";
      if (!stderr.includes("No such container")) {
        throw new Error(
          `Failed to remove test container ${state.containerId}: ${stderr.trim()}`,
        );
      }
    }
  }
}

async function stopProcessesListeningOnPort(port: number): Promise<void> {
  const result = spawnSync(
    "lsof",
    ["-tiTCP:" + String(port), "-sTCP:LISTEN"],
    { encoding: "utf8" },
  );

  if (result.status !== 0 && result.status !== 1) {
    throw new Error(
      `Failed to list listeners on port ${port}: ${(result.stderr ?? "").trim()}`,
    );
  }

  const pids = (result.stdout ?? "")
    .split(/\s+/)
    .map((value) => Number.parseInt(value, 10))
    .filter((value) => Number.isInteger(value) && value > 0);

  await Promise.all(pids.map((pid) => stopProcessTree(pid)));
}

setup.setTimeout(180_000);

setup("start postgres and api", async () => {
  mkdirSync(ARTIFACTS_DIR, { recursive: true });

  await cleanupState(readStateFile());
  await Promise.all([
    stopProcessesListeningOnPort(3001),
    stopProcessesListeningOnPort(5173),
  ]);

  rmSync(STATE_FILE, { force: true });
  rmSync(DB_URI_FILE, { force: true });
  rmSync(API_LOG_FILE, { force: true });
  rmSync(WEB_LOG_FILE, { force: true });

  let currentState: SetupState = {};
  let shouldCleanupOnExit = true;

  const onSignal = () => {
    if (!shouldCleanupOnExit) {
      return;
    }

    void cleanupState(currentState);
  };

  process.once("SIGINT", onSignal);
  process.once("SIGTERM", onSignal);

  try {
    const container = await new GenericContainer("postgres:17.6")
      .withEnvironment({
        POSTGRES_DB: "beamos",
        POSTGRES_USER: "beamos",
        POSTGRES_PASSWORD: "beamos",
      })
      .withExposedPorts(5432)
      .withWaitStrategy(Wait.forListeningPorts())
      .start();

    const host = container.getHost();
    const port = container.getMappedPort(5432);
    const dbUri = `postgresql://beamos:beamos@${host}:${port}/beamos`;
    currentState = { containerId: container.getId(), dbUri };

    writeFileSync(DB_URI_FILE, dbUri, "utf8");
    persistStateFile(currentState);

    const api = spawnDetachedProcess(
      "bun",
      ["run", "--cwd", "apps/api", "dev:test"],
      API_LOG_FILE,
      {
        ...process.env,
        DB_URI: dbUri,
        PORT: "3001",
      },
    );
    currentState.apiPid = api.pid;
    persistStateFile(currentState);

    await waitForUrl(API_URL, 60_000);

    const web = spawnDetachedProcess(
      "bun",
      ["run", "--cwd", "apps/web", "dev:test"],
      WEB_LOG_FILE,
      {
        ...process.env,
        BEAMOS_WEB_ADDITIONAL_PLUGINS_MODULE:
          process.env.BEAMOS_WEB_ADDITIONAL_PLUGINS_MODULE ??
          "./src/plugins/additional-plugins.ts",
      },
    );
    currentState.webPid = web.pid;
    persistStateFile(currentState);

    await waitForUrl(WEB_URL, 60_000);

    shouldCleanupOnExit = false;
  } catch (error) {
    await cleanupState(currentState);
    rmSync(STATE_FILE, { force: true });
    rmSync(DB_URI_FILE, { force: true });
    throw error;
  } finally {
    process.removeListener("SIGINT", onSignal);
    process.removeListener("SIGTERM", onSignal);
  }
});
