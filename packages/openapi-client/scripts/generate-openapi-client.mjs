import { spawn } from "node:child_process";
import path from "node:path";
import { fileURLToPath } from "node:url";
import { mkdir } from "node:fs/promises";

const __filename = fileURLToPath(import.meta.url);
const __dirname = path.dirname(__filename);

const packageDir = path.resolve(__dirname, "..");
const rootDir = path.resolve(packageDir, "..", "..");
const apiDir = path.join(rootDir, "apps", "api");
const openApiDocumentPath = path.join(packageDir, "openapi.json");
const schemaPath = path.join(packageDir, "src", "generated", "schema.d.ts");
// const schemaPath = path.join(packageDir, "src", "kiota");

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

async function main() {
  await mkdir(path.dirname(schemaPath), { recursive: true });
  await runCommand(
    "bun",
    ["run", "openapi:generate-document", "--", openApiDocumentPath],
    {
      cwd: apiDir,
      stdio: "inherit",
    },
  );
  await runCommand(
    "npx",
    ["openapi-typescript", openApiDocumentPath, "-o", schemaPath, "--root-types", "--root-types-no-schema-prefix"],
    {
      cwd: rootDir,
      stdio: "inherit",
    },
  );
  try {
    await runCommand(
      "bunx",
      ["openapi-generator-cli", "generate", "-i", "./openapi.json", "-g", "typescript-fetch", "-o", "./src/generated/ts-fetch.ts"],
      {
        cwd: packageDir,
        stdio: "inherit",
      },
    );
  } catch {
    await runCommand(
      "npx",
      ["@openapitools/openapi-generator-cli", "generate", "-i", "./openapi.json", "-g", "typescript-fetch", "-o", "./src/generated/ts-fetch.ts"],
      {
        cwd: packageDir,
        stdio: "inherit",
      },
    );
  }
  // await runCommand(
  //   "dnx",
  //   ["Microsoft.OpenApi.Kiota@1.29.0", "--allow-roll-forward", "--yes", "--", "generate", "-l", "typescript", "-d", openApiDocumentPath, "-c", "Clientasdf", "-o", schemaPath],
  //   {
  //     cwd: rootDir,
  //     stdio: "inherit",
  //   },
  // );
}

main().catch((error) => {
  console.error(error instanceof Error ? error.message : error);
  process.exit(1);
});
