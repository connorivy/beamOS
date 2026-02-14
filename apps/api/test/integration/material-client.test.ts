import { afterAll, beforeAll, describe, expect, it } from "bun:test";
import { randomUUID } from "node:crypto";
import { createApiClient } from "@beamos/openapi-client";
import { PressureUnits } from "unitsnet-js";
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
              name: "Material 1",
              pressureE: { value: 1.25, unit: PressureUnits.Bars },
              pressureG: { value: 85, unit: PressureUnits.Kilopascals },
            },
            {
              tempId: tempIds[1],
              name: "Material 2",
              pressureE: {
                value: 14.6959,
                unit: PressureUnits.PoundsForcePerSquareInch,
              },
              pressureG: { value: 1013.25, unit: PressureUnits.Millibars },
            },
            {
              tempId: tempIds[2],
              name: "Material 3",
              pressureE: { value: 0.95, unit: PressureUnits.Atmospheres },
              pressureG: { value: 950, unit: PressureUnits.Hectopascals },
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
              name: "Material 1",
              pressureE: { value: 1, unit: PressureUnits.Bars },
              pressureG: { value: 1, unit: PressureUnits.Bars },
            },
            {
              tempId: "dup-1",
              name: "Material 2",
              pressureE: { value: 2, unit: PressureUnits.Bars },
              pressureG: { value: 2, unit: PressureUnits.Bars },
            },
          ],
        },
      },
    );

    expect(batchCreateResponse.data).toBeUndefined();
    expect(batchCreateResponse.response.status).toBe(400);
  });
});
