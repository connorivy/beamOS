import { openSync } from "node:fs";
import { spawn } from "node:child_process";
import treeKill from "tree-kill";

export type SpawnedProcessInfo = {
  pid: number;
};

export async function killProcessTree(
  pid: number,
  signal: NodeJS.Signals = "SIGTERM",
): Promise<void> {
  try {
    process.kill(-pid, signal);
  } catch {
    // Fall back to tree-kill below.
  }

  await new Promise<void>((resolve, reject) => {
    treeKill(pid, signal, (error) => {
      if (error) {
        reject(error);
        return;
      }
      resolve();
    });
  });
}

export async function stopProcessTree(pid?: number): Promise<void> {
  if (!pid || !Number.isInteger(pid)) {
    return;
  }

  try {
    await killProcessTree(pid, "SIGTERM");
  } catch {
    // Process may already be gone. Verify below and escalate only if needed.
  }

  for (let attempt = 0; attempt < 10; attempt++) {
    try {
      process.kill(pid, 0);
      await new Promise((resolve) => setTimeout(resolve, 200));
    } catch {
      return;
    }
  }

  try {
    await killProcessTree(pid, "SIGKILL");
  } catch {
    // Ignore final kill failures.
  }
}

export function spawnDetachedProcess(
  command: string,
  args: string[],
  logFile: string,
  env: NodeJS.ProcessEnv,
): SpawnedProcessInfo {
  const logFd = openSync(logFile, "a");
  const logEnv: NodeJS.ProcessEnv = {
    ...env,
    NO_COLOR: "1",
    FORCE_COLOR: "0",
    CLICOLOR: "0",
    CLICOLOR_FORCE: "0",
    npm_config_color: "false",
  };

  const child = spawn(command, args, {
    cwd: process.cwd(),
    detached: true,
    stdio: ["ignore", logFd, logFd],
    env: logEnv,
  });

  child.unref();

  if (!child.pid) {
    throw new Error(`Failed to spawn process: ${command} ${args.join(" ")}`);
  }

  return { pid: child.pid };
}
