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
    };

    expect(document.paths["/api/models"]?.post).toBeDefined();
    expect(document.paths["/api/models"]?.get).toBeDefined();
    expect(
      document.paths["/api/models/{modelId}/branches/{branchName}/nodes/batch"]
        ?.post,
    ).toBeDefined();
  });
});
