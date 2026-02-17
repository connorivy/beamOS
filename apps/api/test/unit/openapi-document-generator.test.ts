import { describe, expect, it } from "bun:test";
import { mkdtemp, readFile } from "node:fs/promises";
import os from "node:os";
import path from "node:path";

describe("openapi document generator", () => {
  it("writes an openapi document from aggregated endpoints", async () => {
    const tempDir = await mkdtemp(path.join(os.tmpdir(), "beamos-openapi-"));
    const outputPath = path.join(tempDir, "openapi.json");
    const apiDir = path.resolve(import.meta.dir, "..", "..");

    const command = Bun.spawnSync(
      ["bun", "run", "scripts/generate-openapi-document.mjs", outputPath],
      {
        cwd: apiDir,
        stdout: "pipe",
        stderr: "pipe",
      },
    );

    expect(command.exitCode).toBe(0);

    const document = JSON.parse(await readFile(outputPath, "utf8")) as {
      paths: Record<string, Record<string, unknown>>;
      components?: {
        schemas?: Record<string, unknown>;
      };
    };

    expect(document.paths["/api/models"]?.post).toBeDefined();
    expect(document.paths["/api/models"]?.get).toBeDefined();
    expect(
      document.paths["/api/models/{modelId}/branches/{branchName}/nodes/batch"]
        ?.post,
    ).toBeDefined();
    expect(Object.keys(document.components?.schemas ?? {}).length).toBeGreaterThan(
      0,
    );

    const postModels = document.paths["/api/models"]?.post as
      | {
          requestBody?: {
            content?: {
              "application/json"?: {
                schema?: {
                  $ref?: string;
                };
              };
            };
          };
          responses?: {
            "200"?: {
              content?: {
                "application/json"?: {
                  schema?: {
                    $ref?: string;
                  };
                };
              };
            };
          };
        }
      | undefined;

    expect(
      postModels?.requestBody?.content?.["application/json"]?.schema?.$ref,
    ).toBeString();
    expect(
      postModels?.responses?.["200"]?.content?.["application/json"]?.schema
        ?.$ref,
    ).toBeString();
  });
});
