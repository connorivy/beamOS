import { afterAll, beforeAll, describe, expect, it } from "bun:test";
import { randomUUID } from "node:crypto";
import { createApiClient } from "@beamos/openapi-client";
import { setupIntegrationApp, teardownIntegrationApp } from "./shared-test-app";

let baseUrl = "";

beforeAll(async () => {
  baseUrl = await setupIntegrationApp();
}, 10_000);

afterAll(async () => {
  await teardownIntegrationApp();
}, 10_000);

describe("typed node api client integration", () => {
  it("batch creates nodes", async () => {
    const client = createApiClient(baseUrl);
    const tempIds = ["node-01", "node-02"];

    const createModelResponse = await client.POST("/api/models", {
      body: {
        name: "Node Integration Model",
        authorId: randomUUID(),
        message: "Create model for nodes test",
      },
    });

    expect(createModelResponse.error).toBeUndefined();
    expect(createModelResponse.response.status).toBe(200);
    expect(createModelResponse.data).toBeDefined();

    if (!createModelResponse.data) {
      throw new Error("Expected model response");
    }

    const modelId = createModelResponse.data.model.id;
    const branchName = createModelResponse.data.version.branchName;

    const batchCreateResponse = await client.POST(
      "/api/models/{modelId}/branches/{branchName}/nodes/batch",
      {
        params: {
          path: {
            modelId,
            branchName,
          },
        },
        body: {
          nodes: [
            {
              tempId: tempIds[0],
              restraint: {
                canTranslateAlongX: false,
                canTranslateAlongY: true,
                canTranslateAlongZ: true,
                canRotateAboutX: true,
                canRotateAboutY: true,
                canRotateAboutZ: true,
              },
              location: {
                type: "spatial",
                point: { x: 0, y: 1, z: 2 },
              },
            },
            {
              tempId: tempIds[1],
              restraint: {
                canTranslateAlongX: true,
                canTranslateAlongY: false,
                canTranslateAlongZ: true,
                canRotateAboutX: true,
                canRotateAboutY: true,
                canRotateAboutZ: true,
              },
              location: {
                type: "spatial",
                point: { x: 3, y: 4, z: 5 },
              },
            },
          ],
        },
      },
    );

    expect(batchCreateResponse.error).toBeUndefined();
    expect(batchCreateResponse.response.status).toBe(200);
    expect(batchCreateResponse.data).toBeDefined();
    expect(batchCreateResponse.data?.nodes).toHaveLength(tempIds.length);

    if (!batchCreateResponse.data) {
      throw new Error("Expected batch create response");
    }

    for (const tempId of tempIds) {
      expect(batchCreateResponse.data.tempIdToId[tempId]).toBeDefined();
    }

    const responseByTempId = new Map(
      batchCreateResponse.data.nodes.map((node) => [node.id, node]),
    );

    for (const tempId of tempIds) {
      const nodeId = batchCreateResponse.data.tempIdToId[tempId];
      const node = responseByTempId.get(nodeId);
      expect(node).toBeDefined();
      expect(node?.modelId).toBe(modelId);
    }

    const nodeTypes = batchCreateResponse.data.nodes.map(
      (node) => node.nodeTypeDescriminator,
    );
    expect(nodeTypes).toEqual(["external", "external"]);
  });

  it("rejects duplicate temp ids in batch create", async () => {
    const client = createApiClient(baseUrl);

    const createModelResponse = await client.POST("/api/models", {
      body: {
        name: "Duplicate Node TempId Model",
        authorId: randomUUID(),
        message: "Create model for duplicate node tempId test",
      },
    });

    expect(createModelResponse.response.status).toBe(200);
    expect(createModelResponse.data).toBeDefined();

    if (!createModelResponse.data) {
      throw new Error("Expected model response");
    }

    const modelId = createModelResponse.data.model.id;
    const branchName = createModelResponse.data.version.branchName;
    const batchCreateResponse = await client.POST(
      "/api/models/{modelId}/branches/{branchName}/nodes/batch",
      {
        params: {
          path: {
            modelId,
            branchName,
          },
        },
        body: {
          nodes: [
            {
              tempId: "dup-1",
              location: {
                type: "spatial",
                point: { x: 0, y: 0, z: 0 },
              },
            },
            {
              tempId: "dup-1",
              location: {
                type: "spatial",
                point: { x: 1, y: 1, z: 1 },
              },
            },
          ],
        },
      },
    );

    expect(batchCreateResponse.data).toBeUndefined();
    expect(batchCreateResponse.response.status).toBe(400);
  });
});
