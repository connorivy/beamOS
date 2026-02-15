import { afterAll, beforeAll, describe, expect, it } from "bun:test";
import { randomUUID } from "node:crypto";
import { createApiClient } from "@beamos/openapi-client";
import { Pressure, PressureUnits } from "unitsnet-js";
import { setupIntegrationApp, teardownIntegrationApp } from "./shared-test-app";

let baseUrl = "";

beforeAll(async () => {
  baseUrl = await setupIntegrationApp();
}, 10_000);

afterAll(async () => {
  await teardownIntegrationApp();
}, 10_000);

describe("typed material api client integration", () => {
  it("batch creates materials and snapshots get responses", async () => {
    const client = createApiClient(baseUrl);
    const tempIds = ["mat-01", "mat-02", "mat-03"];

    const createModelResponse = await client.POST("/api/models", {
      body: {
        name: "Material Integration Model",
        authorId: randomUUID(),
        message: "Create model for materials test",
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
      "/api/models/{modelId}/branches/{branchName}/materials/batch",
      {
        params: {
          path: {
            modelId,
            branchName,
          },
        },
        body: {
          materials: [
            {
              tempId: tempIds[0],
              E: 125000,
              G: 85000,
              units: { pressure: PressureUnits.Pascals },
            },
            {
              tempId: tempIds[1],
              E: 101324.66370467292,
              G: 101325,
              units: { pressure: PressureUnits.Pascals },
            },
            {
              tempId: tempIds[2],
              E: 96258.75,
              G: 95000,
              units: { pressure: PressureUnits.Pascals },
            },
          ],
        },
      },
    );

    expect(batchCreateResponse.error).toBeUndefined();
    expect(batchCreateResponse.response.status).toBe(200);
    expect(batchCreateResponse.data).toBeDefined();
    expect(batchCreateResponse.data?.materials).toHaveLength(tempIds.length);
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
      getRevisionResponse.data?.modelRevision.materials.map((material) => material.id),
    ).toEqual(
      expect.arrayContaining(
        Object.values(batchCreateResponse.data.tempIdToId),
      ),
    );

    for (const tempId of tempIds) {
      const materialId = batchCreateResponse.data.tempIdToId[tempId];
      expect(materialId).toBeDefined();

      if (!materialId) {
        throw new Error(`Expected material ID for tempId ${tempId}`);
      }

      const getResponse = await client.GET("/api/materials/{materialId}", {
        params: {
          path: { materialId },
        },
      });

      expect(getResponse.error).toBeUndefined();
      expect(getResponse.response.status).toBe(200);
      expect(getResponse.data).toBeDefined();

      if (!getResponse.data) {
        throw new Error(`Expected material response for ${materialId}`);
      }

      expect({
        ...getResponse.data.material,
        id: "<db-id>",
        revisionId: "<revision-id>",
      }).toMatchSnapshot();
    }
  });

  it("rejects duplicate temp ids in batch create", async () => {
    const client = createApiClient(baseUrl);

    const createModelResponse = await client.POST("/api/models", {
      body: {
        name: "Duplicate TempId Model",
        authorId: randomUUID(),
        message: "Create model for duplicate tempId test",
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
      "/api/models/{modelId}/branches/{branchName}/materials/batch",
      {
        params: {
          path: {
            modelId,
            branchName,
          },
        },
        body: {
          materials: [
            {
              tempId: "dup-1",
              E: 1,
              G: 1,
              units: { pressure: PressureUnits.Bars },
            },
            {
              tempId: "dup-1",
              E: 2,
              G: 2,
              units: { pressure: PressureUnits.Bars },
            },
          ],
        },
      },
    );

    expect(batchCreateResponse.data).toBeUndefined();
    expect(batchCreateResponse.response.status).toBe(400);
  });

  it("batch puts materials and updates values", async () => {
    const client = createApiClient(baseUrl);

    const createModelResponse = await client.POST("/api/models", {
      body: {
        name: "Material Batch Put Model",
        authorId: randomUUID(),
        message: "Create model for batch put test",
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
      "/api/models/{modelId}/branches/{branchName}/materials/batch",
      {
        params: {
          path: {
            modelId,
            branchName,
          },
        },
        body: {
          materials: [
            {
              tempId: "mat-put-01",
              E: 100000,
              G: 200000,
              units: { pressure: PressureUnits.Pascals },
            },
          ],
        },
      },
    );

    expect(batchCreateResponse.response.status).toBe(200);
    expect(batchCreateResponse.data).toBeDefined();

    if (!batchCreateResponse.data) {
      throw new Error("Expected batch create response");
    }

    const materialId = batchCreateResponse.data.tempIdToId["mat-put-01"];
    if (!materialId) {
      throw new Error("Expected material ID for mat-put-01");
    }

    const batchPutResponse = await client.PUT(
      "/api/models/{modelId}/branches/{branchName}/materials/batch",
      {
        params: {
          path: {
            modelId,
            branchName,
          },
        },
        body: {
          materials: [
            {
              id: materialId,
              E: 250000,
              G: 206842.7185,
              units: { pressure: PressureUnits.Pascals },
            },
          ],
        },
      },
    );

    expect(batchPutResponse.error).toBeUndefined();
    expect(batchPutResponse.response.status).toBe(200);
    expect(batchPutResponse.data?.materials).toHaveLength(1);
    expect(batchPutResponse.data?.materials[0]?.id).toBe(materialId);

    const getResponse = await client.GET("/api/materials/{materialId}", {
      params: {
        path: { materialId },
      },
    });

    expect(getResponse.error).toBeUndefined();
    expect(getResponse.response.status).toBe(200);
    expect(getResponse.data).toBeDefined();

    if (!getResponse.data) {
      throw new Error(`Expected material response for ${materialId}`);
    }

    expect(getResponse.data.material.E).toBeCloseTo(250000, 6);
    expect(getResponse.data.material.G).toBeCloseTo(206842.7185, 6);
    expect(getResponse.data.material.units.pressure).toBe(PressureUnits.Pascals);
  });
});
