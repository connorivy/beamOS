import { afterAll, beforeAll, describe, expect, it } from "bun:test";
import { randomUUID } from "node:crypto";
import { createApiClient } from "@beamos/openapi-client";
import {
  AreaMomentOfInertiaUnits,
  AreaUnits,
  PressureUnits,
  VolumeUnits,
  WarpingMomentOfInertiaUnits,
} from "unitsnet-js";
import { setupIntegrationApp, teardownIntegrationApp } from "./shared-test-app";

let baseUrl = "";

beforeAll(async () => {
  baseUrl = await setupIntegrationApp();
}, 10_000);

afterAll(async () => {
  await teardownIntegrationApp();
}, 10_000);

describe("typed element1d api client integration", () => {
  it("batch creates element1ds and snapshots get responses", async () => {
    const client = createApiClient(baseUrl);
    const tempIds = ["el-01", "el-02"];

    const createModelResponse = await client.POST("/api/models", {
      body: {
        name: "Element1d Integration Model",
        authorId: randomUUID(),
        message: "Create model for element1d test",
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

    const materialBatchCreateResponse = await client.POST(
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
              tempId: "mat-for-element",
              name: "Material for Element1d",
              pressureE: { value: 1.25, unit: PressureUnits.Bars },
              pressureG: { value: 85, unit: PressureUnits.Kilopascals },
            },
          ],
        },
      },
    );

    expect(materialBatchCreateResponse.error).toBeUndefined();
    expect(materialBatchCreateResponse.response.status).toBe(200);
    expect(materialBatchCreateResponse.data?.materials).toHaveLength(1);

    if (!materialBatchCreateResponse.data) {
      throw new Error("Expected material batch response");
    }

    const materialId =
      materialBatchCreateResponse.data.tempIdToId["mat-for-element"];

    const sectionProfileBatchCreateResponse = await client.POST(
      "/api/models/{modelId}/branches/{branchName}/section-profiles/batch",
      {
        params: {
          path: {
            modelId,
            branchName,
          },
        },
        body: {
          units: {
            area: AreaUnits.SquareInches,
            areaMomentOfInertia: AreaMomentOfInertiaUnits.InchesToTheFourth,
            warpingMomentOfInertia:
              WarpingMomentOfInertiaUnits.InchesToTheSixth,
            volume: VolumeUnits.CubicInches,
          },
          sectionProfiles: [
            {
              tempId: "sp-for-element",
              name: "W12x26",
              discriminator: "STANDARD",
              area: 7.65,
              strongAxisMomentOfInertia: 204,
              weakAxisMomentOfInertia: 17.3,
              torsionalConstant: 0.346,
              warpingConstant: 337,
              strongAxisPlasticSectionModulus: 38.4,
              weakAxisPlasticSectionModulus: 8.94,
              strongAxisElasticSectionModulus: 34,
              weakAxisElasticSectionModulus: 5.77,
            },
          ],
        },
      },
    );

    expect(sectionProfileBatchCreateResponse.error).toBeUndefined();
    expect(sectionProfileBatchCreateResponse.response.status).toBe(200);
    expect(
      sectionProfileBatchCreateResponse.data?.sectionProfiles,
    ).toHaveLength(1);

    if (!sectionProfileBatchCreateResponse.data) {
      throw new Error("Expected section profile batch response");
    }

    const sectionProfileId =
      sectionProfileBatchCreateResponse.data.tempIdToId["sp-for-element"];

    const batchCreateResponse = await client.POST(
      "/api/models/{modelId}/branches/{branchName}/element1ds/batch",
      {
        params: {
          path: {
            modelId,
            branchName,
          },
        },
        body: {
          element1ds: [
            {
              tempId: tempIds[0],
              startNodeId: Bun.randomUUIDv7(),
              endNodeId: Bun.randomUUIDv7(),
              materialId,
              sectionProfileId,
            },
            {
              tempId: tempIds[1],
              startNodeId: Bun.randomUUIDv7(),
              endNodeId: Bun.randomUUIDv7(),
              materialId,
              sectionProfileId,
            },
          ],
        },
      },
    );

    expect(batchCreateResponse.error).toBeUndefined();
    expect(batchCreateResponse.response.status).toBe(200);
    expect(batchCreateResponse.data).toBeDefined();
    expect(batchCreateResponse.data?.element1ds).toHaveLength(tempIds.length);
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
      getRevisionResponse.data?.modelRevision.element1ds.map((element1d) => element1d.id),
    ).toEqual(
      expect.arrayContaining(
        Object.values(batchCreateResponse.data.tempIdToId),
      ),
    );

    for (const tempId of tempIds) {
      const element1dId = batchCreateResponse.data.tempIdToId[tempId];
      expect(element1dId).toBeDefined();

      if (!element1dId) {
        throw new Error(`Expected element1d ID for tempId ${tempId}`);
      }

      const getResponse = await client.GET("/api/element1ds/{element1dId}", {
        params: {
          path: { element1dId },
        },
      });

      expect(getResponse.error).toBeUndefined();
      expect(getResponse.response.status).toBe(200);
      expect(getResponse.data).toBeDefined();

      if (!getResponse.data) {
        throw new Error(`Expected element1d response for ${element1dId}`);
      }

      expect({
        ...getResponse.data.element1d,
        id: "<db-id>",
        revisionId: "<revision-id>",
        startNodeId: "<start-node-id>",
        endNodeId: "<end-node-id>",
        materialId: "<material-id>",
        sectionProfileId: "<section-profile-id>",
      }).toMatchSnapshot();
    }
  });

  it("rejects duplicate temp ids in batch create", async () => {
    const client = createApiClient(baseUrl);

    const createModelResponse = await client.POST("/api/models", {
      body: {
        name: "Duplicate Element1d TempId Model",
        authorId: randomUUID(),
        message: "Create model for duplicate element1d tempId test",
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
      "/api/models/{modelId}/branches/{branchName}/element1ds/batch",
      {
        params: {
          path: {
            modelId,
            branchName,
          },
        },
        body: {
          element1ds: [
            {
              tempId: "dup-1",
              startNodeId: Bun.randomUUIDv7(),
              endNodeId: Bun.randomUUIDv7(),
              materialId: Bun.randomUUIDv7(),
              sectionProfileId: Bun.randomUUIDv7(),
            },
            {
              tempId: "dup-1",
              startNodeId: Bun.randomUUIDv7(),
              endNodeId: Bun.randomUUIDv7(),
              materialId: Bun.randomUUIDv7(),
              sectionProfileId: Bun.randomUUIDv7(),
            },
          ],
        },
      },
    );

    expect(batchCreateResponse.data).toBeUndefined();
    expect(batchCreateResponse.response.status).toBe(400);
  });
});
