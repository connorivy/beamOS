import { existsSync, readFileSync, rmSync } from "node:fs";
import path from "node:path";
import { spawnSync } from "node:child_process";
import { test as teardown } from "@playwright/test";
import { stopProcessTree } from "./api-process";

const ARTIFACTS_DIR = path.resolve(process.cwd(), ".playwright");
const STATE_FILE = path.join(ARTIFACTS_DIR, "testcontainers-state.json");
const DB_URI_FILE = path.join(ARTIFACTS_DIR, "db-uri.txt");

type SetupState = {
  containerId?: string;
  apiPid?: number;
  webPid?: number;
};

teardown("stop postgres, api, and web", async () => {
  if (!existsSync(STATE_FILE)) {
    return;
  }

  try {
    const state = JSON.parse(readFileSync(STATE_FILE, "utf8")) as SetupState;
    await Promise.all([stopProcessTree(state.apiPid), stopProcessTree(state.webPid)]);

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
  } finally {
    rmSync(STATE_FILE, { force: true });
    rmSync(DB_URI_FILE, { force: true });
  }
});
