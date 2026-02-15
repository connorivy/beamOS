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
  it("batch creates nodes and verifies persistence in revision", async () => {
    const client = createApiClient(baseUrl);
    const tempIds = ["node-01", "node-02", "node-03"];

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
            {
              tempId: tempIds[2],
              restraint: {
                canTranslateAlongX: false,
                canTranslateAlongY: false,
                canTranslateAlongZ: false,
                canRotateAboutX: false,
                canRotateAboutY: false,
                canRotateAboutZ: false,
              },
              location: {
                type: "spatial",
                point: { x: 10, y: 20, z: 30 },
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
    expect(batchCreateResponse.data?.tempIdToId).toBeDefined();

    if (!batchCreateResponse.data) {
      throw new Error("Expected batch create response");
    }

    const getRevisionResponse = await client.GET(
      "/api/models/{modelId}/branches/{branchName}/revision",
      {
        params: {
          path: { modelId, branchName },
        },
      },
    );

    expect(getRevisionResponse.error).toBeUndefined();
    expect(getRevisionResponse.response.status).toBe(200);
    expect(getRevisionResponse.data).toBeDefined();
    expect(
      getRevisionResponse.data?.modelRevision.nodes.map((node) => node.id),
    ).toEqual(
      expect.arrayContaining(
        Object.values(batchCreateResponse.data.tempIdToId),
      ),
    );

    // Verify all created nodes are in the revision
    for (const tempId of tempIds) {
      const nodeId = batchCreateResponse.data.tempIdToId[tempId];
      expect(nodeId).toBeDefined();

      if (!nodeId) {
        throw new Error(`Expected node ID for tempId ${tempId}`);
      }

      const nodeInRevision = getRevisionResponse.data?.modelRevision.nodes.find(
        (n) => n.id === nodeId,
      );
      expect(nodeInRevision).toBeDefined();
      expect(nodeInRevision?.id).toBe(nodeId);
      expect(nodeInRevision?.modelId).toBe(modelId);
      expect(nodeInRevision?.nodeTypeDescriminator).toBe("external");
    }

    // Snapshot the nodes data structure
    const nodesSnapshot = getRevisionResponse.data?.modelRevision.nodes.map(
      (node) => ({
        ...node,
        id: "<db-id>",
        modelId: "<model-id>",
      }),
    );
    expect(nodesSnapshot).toMatchSnapshot();
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
