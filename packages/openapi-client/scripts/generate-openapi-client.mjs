import { spawn } from "node:child_process";
import path from "node:path";
import { fileURLToPath } from "node:url";
import { mkdir, writeFile } from "node:fs/promises";

const __filename = fileURLToPath(import.meta.url);
const __dirname = path.dirname(__filename);

const packageDir = path.resolve(__dirname, "..");
const rootDir = path.resolve(packageDir, "..", "..");
const apiDir = path.join(rootDir, "apps", "api");
const schemaPath = path.join(packageDir, "src", "generated", "schema.d.ts");
const preprocessedOpenApiPath = "/tmp/beamos-openapi.preprocessed.json";
// const schemaPath = path.join(packageDir, "src", "kiota");
const openApiUrl = "http://127.0.0.1:3001/openapi/json";

const METHODS = ["get", "put", "post", "delete", "options", "head", "patch", "trace"];

function toPascalCase(value) {
  return value
    .replace(/[^a-zA-Z0-9]+/g, " ")
    .split(" ")
    .filter(Boolean)
    .map((part) => part.charAt(0).toUpperCase() + part.slice(1))
    .join("");
}

function getUniqueSchemaName(schemas, baseName) {
  if (!schemas[baseName]) return baseName;

  let index = 2;
  let nextName = `${baseName}${index}`;
  while (schemas[nextName]) {
    index += 1;
    nextName = `${baseName}${index}`;
  }

  return nextName;
}

function hoistInlineSchemaToComponent(schemas, baseName, schema) {
  if (!schema || typeof schema !== "object" || Array.isArray(schema) || "$ref" in schema) {
    return schema;
  }

  const schemaName = getUniqueSchemaName(schemas, baseName);
  schemas[schemaName] = schema;
  return { $ref: `#/components/schemas/${schemaName}` };
}

function normalizeOpenApiForRootTypes(document) {
  if (!document || typeof document !== "object") return document;

  const components = (document.components ??= {});
  const schemas = (components.schemas ??= {});
  const paths = document.paths ?? {};

  for (const [routePath, pathItem] of Object.entries(paths)) {
    if (!pathItem || typeof pathItem !== "object") continue;

    for (const method of METHODS) {
      const operation = pathItem[method];
      if (!operation || typeof operation !== "object") continue;

      const operationBaseName = toPascalCase(
        operation.operationId ?? `${method}_${routePath}`,
      );

      const requestBody = operation.requestBody;
      if (requestBody && typeof requestBody === "object" && requestBody.content) {
        for (const [contentType, mediaType] of Object.entries(requestBody.content)) {
          if (!mediaType || typeof mediaType !== "object" || !("schema" in mediaType)) {
            continue;
          }

          mediaType.schema = hoistInlineSchemaToComponent(
            schemas,
            `${operationBaseName}Request${toPascalCase(contentType)}`,
            mediaType.schema,
          );
        }
      }

      const responses = operation.responses;
      if (!responses || typeof responses !== "object") continue;

      for (const [statusCode, response] of Object.entries(responses)) {
        if (!response || typeof response !== "object" || !response.content) continue;

        for (const [contentType, mediaType] of Object.entries(response.content)) {
          if (!mediaType || typeof mediaType !== "object" || !("schema" in mediaType)) {
            continue;
          }

          mediaType.schema = hoistInlineSchemaToComponent(
            schemas,
            `${operationBaseName}Response${statusCode}${toPascalCase(contentType)}`,
            mediaType.schema,
          );
        }
      }
    }
  }

  return document;
}

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

    const openApiDocument = await fetch(openApiUrl).then((response) => {
      if (!response.ok) {
        throw new Error(`Failed to fetch OpenAPI schema from ${openApiUrl}`);
      }
      return response.json();
    });

    normalizeOpenApiForRootTypes(openApiDocument);
    await writeFile(preprocessedOpenApiPath, JSON.stringify(openApiDocument), "utf8");

    await runCommand(
      "npx",
      [
        "openapi-typescript",
        preprocessedOpenApiPath,
        "-o",
        schemaPath,
        "--root-types",
        "--root-types-no-schema-prefix",
      ],
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
