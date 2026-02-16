import { afterAll, beforeAll, describe, expect, it } from "bun:test";
import { randomUUID } from "node:crypto";
import { createApiClient } from "@beamos/openapi-client";
import { drizzleModelRepository } from "../../src/models/model-repository";
import { drizzleModelVersionRepository } from "../../src/model-revisions/model-revision-repository";
import {
  setupIntegrationApp,
  teardownIntegrationApp,
} from "./shared-test-app";

let baseUrl = "";

beforeAll(async () => {
  baseUrl = await setupIntegrationApp();
}, 10_000);

afterAll(async () => {
  await teardownIntegrationApp();
}, 10_000);

describe("typed openapi client integration", () => {
  it("creates a model", async () => {
    const client = createApiClient(baseUrl);

    const requestBody = {
      name: "Integration Test Model",
      authorId: randomUUID(),
      message: "Create a model through typed OpenAPI client",
    };

    const { data, error, response } = await client.POST("/api/models", {
      body: requestBody,
    });

    expect(error).toBeUndefined();
    expect(response.status).toBe(200);
    expect(data).toBeDefined();

    if (!data) {
      throw new Error("Expected response body from create model API");
    }

    expect(data.model.name).toBe(requestBody.name);
    expect(data.model.id).toMatch(/^[0-9a-f-]{36}$/i);
    expect(data.model.description).toBe("");

    const storedModelWithoutBranchHeads = await drizzleModelRepository.getById({
      modelId: data.model.id,
    });
    expect(storedModelWithoutBranchHeads).toBeDefined();
    expect(storedModelWithoutBranchHeads?.modelBranchHeads).toBeNull();

    const storedModelWithBranchHeads = await drizzleModelRepository.getById({
      modelId: data.model.id,
      loadModelBranchHeadAggregates: true,
    });
    expect(storedModelWithBranchHeads).toBeDefined();
    expect(storedModelWithBranchHeads?.modelBranchHeads).not.toBeNull();
    expect(
      storedModelWithBranchHeads?.modelBranchHeads?.map((branch) => branch.branchName),
    ).toContain("main");
    const mainBranchHead = storedModelWithBranchHeads?.modelBranchHeads?.find(
      (branch) => branch.branchName === "main",
    );
    expect(mainBranchHead?.headRevisionId).toBe(data.version.revisionId);

    const initialRevision = await drizzleModelVersionRepository.getRevisionById(
      data.version.revisionId,
    );
    expect(initialRevision).toBeDefined();
    expect(initialRevision?.modelId).toBe(data.model.id);
  });
});
