import { spawn } from "node:child_process";
import path from "node:path";
import { fileURLToPath } from "node:url";
import { mkdir, writeFile } from "node:fs/promises";

const __filename = fileURLToPath(import.meta.url);
const __dirname = path.dirname(__filename);

const packageDir = path.resolve(__dirname, "..");
const rootDir = path.resolve(packageDir, "..", "..");
const apiDir = path.join(rootDir, "apps", "api");
const openApiPath = path.join(packageDir, "openapi.json");
const schemaPath = path.join(packageDir, "src", "generated", "schema.d.ts");
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

function toSchemaNamePart(input) {
  const words = String(input)
    .replace(/[^a-zA-Z0-9]+/g, " ")
    .trim()
    .split(/\s+/)
    .filter(Boolean)
    .map((word) => word.charAt(0).toUpperCase() + word.slice(1));

  if (words.length === 0) return "Schema";
  if (/^[0-9]/.test(words[0])) words.unshift("Schema");
  return words.join("");
}

function addComponentSchema(openApiDoc, baseName, suffix, schema, usedNames) {
  if (!schema || typeof schema !== "object" || "$ref" in schema) {
    return schema;
  }

  const schemas =
    (openApiDoc.components ??= {}).schemas ??=
      {};
  let name = `${baseName}${suffix}`;
  let copyIndex = 2;
  while (usedNames.has(name) || name in schemas) {
    name = `${baseName}${suffix}${copyIndex}`;
    copyIndex += 1;
  }

  usedNames.add(name);
  schemas[name] = schema;
  return { $ref: `#/components/schemas/${name}` };
}

function hoistInlineSchemas(openApiDoc) {
  const usedNames = new Set(Object.keys(openApiDoc.components?.schemas ?? {}));

  for (const [pathKey, pathItem] of Object.entries(openApiDoc.paths ?? {})) {
    if (!pathItem || typeof pathItem !== "object") continue;

    for (const [method, operation] of Object.entries(pathItem)) {
      if (!operation || typeof operation !== "object") continue;

      const operationBaseName = toSchemaNamePart(operation.operationId ?? `${method}_${pathKey}`);

      const requestBody = operation.requestBody;
      if (requestBody?.content && typeof requestBody.content === "object") {
        for (const [contentType, mediaType] of Object.entries(requestBody.content)) {
          if (!mediaType || typeof mediaType !== "object") continue;
          mediaType.schema = addComponentSchema(
            openApiDoc,
            operationBaseName,
            `Request${toSchemaNamePart(contentType)}`,
            mediaType.schema,
            usedNames,
          );
        }
      }

      const responses = operation.responses;
      if (!responses || typeof responses !== "object") continue;

      for (const [statusCode, response] of Object.entries(responses)) {
        if (!response || typeof response !== "object" || !response.content) continue;

        for (const [contentType, mediaType] of Object.entries(response.content)) {
          if (!mediaType || typeof mediaType !== "object") continue;
          mediaType.schema = addComponentSchema(
            openApiDoc,
            operationBaseName,
            `Response${toSchemaNamePart(statusCode)}${toSchemaNamePart(contentType)}`,
            mediaType.schema,
            usedNames,
          );
        }
      }
    }
  }

  return openApiDoc;
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
    const response = await fetch(openApiUrl);
    if (!response.ok) {
      throw new Error(`Failed to fetch OpenAPI document from ${openApiUrl}: HTTP ${response.status}`);
    }
    const openApiDoc = hoistInlineSchemas(await response.json());
    await writeFile(openApiPath, `${JSON.stringify(openApiDoc, null, 2)}\n`);
    await runCommand(
      "npx",
      ["openapi-typescript", openApiPath, "-o", schemaPath, "--root-types", "--root-types-no-schema-prefix"],
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
