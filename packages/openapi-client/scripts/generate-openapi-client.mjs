import { spawn } from "node:child_process";
import path from "node:path";
import { fileURLToPath } from "node:url";
import { mkdir } from "node:fs/promises";

const __filename = fileURLToPath(import.meta.url);
const __dirname = path.dirname(__filename);

const packageDir = path.resolve(__dirname, "..");
const rootDir = path.resolve(packageDir, "..", "..");
const apiDir = path.join(rootDir, "apps", "api");
const schemaPath = path.join(packageDir, "src", "generated", "schema.d.ts");
// const schemaPath = path.join(packageDir, "src", "kiota");
const openApiUrl = "http://127.0.0.1:3001/openapi/json";

function sleep(ms) {
  return new Promise((resolve) => setTimeout(resolve, ms));
}

function runCommand(cmd, args, options = {}) {
  return new Promise((resolve, reject) => {
    const child = spawn(cmd, args, options);
    const command = `${cmd} ${args.join(" ")}`;
    const cwd = options.cwd ?? process.cwd();
    let stdout = "";
    let stderr = "";

    // Capture output when stdio is piped, so failures include useful context.
    child.stdout?.on("data", (chunk) => {
      stdout += chunk.toString();
    });
    child.stderr?.on("data", (chunk) => {
      stderr += chunk.toString();
    });

    child.on("error", (error) => {
      reject(
        new Error(
          [
            `Failed to start command: ${command}`,
            `cwd: ${cwd}`,
            `error: ${error instanceof Error ? error.message : String(error)}`,
          ].join("\n"),
        ),
      );
    });
    child.on("exit", (code, signal) => {
      if (code === 0) {
        resolve();
        return;
      }

      const output = [
        stdout.trim() ? `stdout:\n${stdout.trim()}` : null,
        stderr.trim() ? `stderr:\n${stderr.trim()}` : null,
      ]
        .filter(Boolean)
        .join("\n\n");

      reject(
        new Error(
          [
            `Command failed: ${command}`,
            `cwd: ${cwd}`,
            signal ? `signal: ${signal}` : `exit code: ${code}`,
            output || "No captured stdout/stderr (command may be using stdio: \"inherit\").",
          ].join("\n"),
        ),
      );
    });
  });
}

async function waitForOpenApi(devProcess, timeoutMs = 120_000) {
  const deadline = Date.now() + timeoutMs;

  while (Date.now() < deadline) {
    if (devProcess.exitCode !== null) {
      throw new Error(`bun run dev:db exited early with code ${devProcess.exitCode}`);
    }

    try {
      const response = await fetch(openApiUrl);
      if (response.ok) return;
    } catch {
      // Retry until timeout.
    }

    await sleep(1_000);
  }

  throw new Error(`Timed out waiting for OpenAPI endpoint at ${openApiUrl}`);
}

async function stopProcess(devProcess) {
  if (!devProcess || devProcess.exitCode !== null || !devProcess.pid) return;

  const waitForExit = async (timeoutMs) => {
    const deadline = Date.now() + timeoutMs;
    while (devProcess.exitCode === null && Date.now() < deadline) {
      await sleep(100);
    }
  };

  try {
    process.kill(-devProcess.pid, "SIGTERM");
  } catch {
    devProcess.kill("SIGTERM");
  }

  await waitForExit(10_000);

  if (devProcess.exitCode === null) {
    try {
      process.kill(-devProcess.pid, "SIGKILL");
    } catch {
      devProcess.kill("SIGKILL");
    }
    await waitForExit(2_000);
  }
}

async function main() {
  await mkdir(path.dirname(schemaPath), { recursive: true });

  const devProcess = spawn("bun", ["run", "dev:db"], {
    cwd: apiDir,
    stdio: "inherit",
    detached: true,
  });

  try {
    await waitForOpenApi(devProcess);
    await runCommand(
      "npx",
      ["openapi-typescript", openApiUrl, "-o", schemaPath, "--root-types"],
      {
        cwd: rootDir,
        stdio: "inherit",
      },
    );
    // await runCommand(
    //   "dnx",
    //   ["Microsoft.OpenApi.Kiota@1.29.0", "--allow-roll-forward", "--yes", "--", "generate", "-l", "typescript", "-d", openApiUrl, "-c", "Clientasdf", "-o", schemaPath],
    //   {
    //     cwd: rootDir,
    //     stdio: "inherit",
    //   },
    // );
  } finally {
    await stopProcess(devProcess);
  }
}

main().catch((error) => {
  console.error(error instanceof Error ? error.message : error);
  process.exit(1);
});
