import { afterAll, beforeAll, describe, expect, it } from "bun:test";
import { randomUUID } from "node:crypto";
import { createApiClient } from "@beamos/openapi-client";
import {
  setupIntegrationApp,
  teardownIntegrationApp,
} from "./shared-test-app";

let baseUrl = "";

beforeAll(async () => {
  baseUrl = await setupIntegrationApp();
}, 30_000);

afterAll(async () => {
  await teardownIntegrationApp();
}, 30_000);

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

    const mainBranchRevisionResponse = await client.GET(
      "/api/models/{modelId}/branches/{branchName}/revision",
      {
        params: {
          path: {
            modelId: data.model.id,
            branchName: "main",
          },
        },
      },
    );

    expect(mainBranchRevisionResponse.error).toBeUndefined();
    expect(mainBranchRevisionResponse.response.status).toBe(200);
    expect(mainBranchRevisionResponse.data).toBeDefined();

    if (!mainBranchRevisionResponse.data) {
      throw new Error("Expected response body from get model revision API");
    }

    expect(mainBranchRevisionResponse.data.modelRevision.id).toBe(
      data.version.revisionId,
    );
    expect(mainBranchRevisionResponse.data.modelRevision.modelId).toBe(
      data.model.id,
    );
    expect(mainBranchRevisionResponse.data.modelRevision.name).toBe(
      requestBody.name,
    );
    expect(mainBranchRevisionResponse.data.modelRevision.parentRevisionId).toBeNull();
    expect(mainBranchRevisionResponse.data.modelRevision.nodes).toHaveLength(0);
    expect(mainBranchRevisionResponse.data.modelRevision.materials).toHaveLength(0);
    expect(mainBranchRevisionResponse.data.modelRevision.sectionProfiles).toHaveLength(0);
    expect(mainBranchRevisionResponse.data.modelRevision.element1ds).toHaveLength(0);

    const missingBranchRevisionResponse = await client.GET(
      "/api/models/{modelId}/branches/{branchName}/revision",
      {
        params: {
          path: {
            modelId: data.model.id,
            branchName: "missing",
          },
        },
      },
    );

    expect(missingBranchRevisionResponse.data).toBeUndefined();
    expect(missingBranchRevisionResponse.response.status).toBe(404);
  });

  it("lists models with last modified timestamp and role", async () => {
    const client = createApiClient(baseUrl);
    const requestBody = {
      name: "List Models Integration Test",
      authorId: randomUUID(),
      message: "Create a model for list endpoint",
    };

    const createResponse = await client.POST("/api/models", {
      body: requestBody,
    });

    expect(createResponse.error).toBeUndefined();
    expect(createResponse.response.status).toBe(200);
    expect(createResponse.data).toBeDefined();

    if (!createResponse.data) {
      throw new Error("Expected response body from create model API");
    }

    const revisionResponse = await client.GET(
      "/api/models/{modelId}/branches/{branchName}/revision",
      {
        params: {
          path: {
            modelId: createResponse.data.model.id,
            branchName: "main",
          },
        },
      },
    );

    expect(revisionResponse.error).toBeUndefined();
    expect(revisionResponse.response.status).toBe(200);
    expect(revisionResponse.data).toBeDefined();

    if (!revisionResponse.data) {
      throw new Error("Expected response body from get model revision API");
    }

    const listResponse = await fetch(`${baseUrl}/api/models`);

    expect(listResponse.status).toBe(200);

    const body = (await listResponse.json()) as {
      models: Array<{
        id: string;
        name: string;
        description: string;
        lastModified: string | null;
        role: "Owner" | "Contributor" | "Reviewer";
      }>;
    };

    const listedModel = body.models.find(
      (model) => model.id === createResponse.data?.model.id,
    );

    expect(listedModel).toBeDefined();

    if (!listedModel) {
      throw new Error("Expected created model to be returned by get models API");
    }

    expect(listedModel.name).toBe(requestBody.name);
    expect(listedModel.description).toBe("");
    expect(listedModel.lastModified).toBe(
      revisionResponse.data.modelRevision.createdAt,
    );
    expect(listedModel.role).toBe("Owner");
  });
});
