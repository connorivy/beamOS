import { spawn } from "node:child_process";
import { mkdtempSync, rmSync } from "node:fs";
import os from "node:os";
import path from "node:path";
import { fileURLToPath } from "node:url";

const waitForMs = (ms: number): Promise<void> =>
  new Promise((resolve) => setTimeout(resolve, ms));

const waitForExit = async (child: ReturnType<typeof spawn>, timeoutMs: number): Promise<void> => {
  if (child.exitCode !== null) {
    return;
  }

  const deadline = Date.now() + timeoutMs;
  while (child.exitCode === null && Date.now() < deadline) {
    await waitForMs(100);
  }
};

const beamosDir = path.dirname(fileURLToPath(import.meta.url));
const apiEntry = path.join(beamosDir, "apps/api/src/index.ts");
const tempDir = mkdtempSync(path.join(os.tmpdir(), "beamos-e2e-"));
const dbUri = `file:${path.join(tempDir, "test.sqlite")}`;

let shuttingDown = false;
let api: ReturnType<typeof spawn> | null = null;
let restartCount = 0;

const spawnApi = (): ReturnType<typeof spawn> =>
  spawn("bun", [apiEntry], {
    cwd: beamosDir,
    env: {
      ...process.env,
      DB_URI: dbUri,
      PORT: process.env.PORT ?? "3001",
    },
    stdio: "inherit",
  });

const shutdown = async (exitCode: number): Promise<void> => {
  if (shuttingDown) {
    return;
  }

  shuttingDown = true;
  if (api) {
    api.kill("SIGTERM");
    await waitForExit(api, 5_000);
  }
  rmSync(tempDir, { recursive: true, force: true });
  process.exit(exitCode);
};

process.on("SIGINT", () => {
  void shutdown(0);
});

process.on("SIGTERM", () => {
  void shutdown(0);
});

const startAndWatchApi = (): void => {
  const startedAt = Date.now();
  api = spawnApi();

  api.on("exit", (code) => {
    if (shuttingDown) {
      return;
    }

    const ranForMs = Date.now() - startedAt;
    const shouldRetry = ranForMs < 15_000 && restartCount < 5;
    if (shouldRetry) {
      restartCount += 1;
      setTimeout(startAndWatchApi, 1_000);
      return;
    }

    const exitCode = code ?? 1;
    void shutdown(exitCode);
  });
};

startAndWatchApi();

await new Promise<void>(() => {
  // Keep this process alive until a signal or child exit triggers shutdown.
});
