import { afterAll, beforeAll, describe, expect, it } from "bun:test";
import { randomUUID } from "node:crypto";
import { createApiClient } from "@beamos/openapi-client";
import {
  AreaMomentOfInertiaUnits,
  AreaUnits,
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

describe("typed section profile api client integration", () => {
  it("batch creates section profiles and snapshots get responses", async () => {
    const client = createApiClient(baseUrl);
    const imperialTempId = "sp-01";
    const metricTempId = "sp-02";

    const createModelResponse = await client.POST("/api/models", {
      body: {
        name: "Section Profile Integration Model",
        authorId: randomUUID(),
        message: "Create model for section profile test",
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

    const imperialBatchCreateResponse = await client.POST(
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
              tempId: imperialTempId,
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

    const metricBatchCreateResponse = await client.POST(
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
            area: AreaUnits.SquareCentimeters,
            areaMomentOfInertia:
              AreaMomentOfInertiaUnits.CentimetersToTheFourth,
            warpingMomentOfInertia:
              WarpingMomentOfInertiaUnits.CentimetersToTheSixth,
            volume: VolumeUnits.CubicCentimeters,
          },
          sectionProfiles: [
            {
              tempId: metricTempId,
              name: "IPE 200",
              discriminator: "WITH_SHEAR_AREAS",
              area: 33.4,
              strongAxisMomentOfInertia: 1940,
              weakAxisMomentOfInertia: 142,
              torsionalConstant: 14,
              warpingConstant: 11700,
              strongAxisPlasticSectionModulus: 220,
              weakAxisPlasticSectionModulus: 44.6,
              strongAxisElasticSectionModulus: 194,
              weakAxisElasticSectionModulus: 28.5,
              strongAxisShearArea: 19.8,
              weakAxisShearArea: 13.2,
            },
          ],
        },
      },
    );

    expect(imperialBatchCreateResponse.error).toBeUndefined();
    expect(imperialBatchCreateResponse.response.status).toBe(200);
    expect(imperialBatchCreateResponse.data).toBeDefined();
    expect(imperialBatchCreateResponse.data?.sectionProfiles).toHaveLength(1);
    expect(imperialBatchCreateResponse.data?.tempIdToId).toBeDefined();

    expect(metricBatchCreateResponse.error).toBeUndefined();
    expect(metricBatchCreateResponse.response.status).toBe(200);
    expect(metricBatchCreateResponse.data).toBeDefined();
    expect(metricBatchCreateResponse.data?.sectionProfiles).toHaveLength(1);
    expect(metricBatchCreateResponse.data?.tempIdToId).toBeDefined();

    if (!imperialBatchCreateResponse.data || !metricBatchCreateResponse.data) {
      throw new Error("Expected batch create responses");
    }

    const tempIdToId = {
      ...imperialBatchCreateResponse.data.tempIdToId,
      ...metricBatchCreateResponse.data.tempIdToId,
    };

    for (const tempId of [imperialTempId, metricTempId]) {
      const sectionProfileId = tempIdToId[tempId];
      expect(sectionProfileId).toBeDefined();

      if (!sectionProfileId) {
        throw new Error(`Expected section profile ID for tempId ${tempId}`);
      }

      const getResponse = await client.GET(
        "/api/section-profiles/{sectionProfileId}",
        {
          params: {
            path: { sectionProfileId },
          },
        },
      );

      expect(getResponse.error).toBeUndefined();
      expect(getResponse.response.status).toBe(200);
      expect(getResponse.data).toBeDefined();

      if (!getResponse.data) {
        throw new Error(`Expected section profile response for ${sectionProfileId}`);
      }

      expect({
        ...getResponse.data.sectionProfile,
        id: "<db-id>",
        revisionId: "<revision-id>",
      }).toMatchSnapshot();
    }
  });

  it("rejects duplicate temp ids in batch create", async () => {
    const client = createApiClient(baseUrl);

    const createModelResponse = await client.POST("/api/models", {
      body: {
        name: "Duplicate Section Profile TempId Model",
        authorId: randomUUID(),
        message: "Create model for duplicate section profile tempId test",
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
              tempId: "dup-1",
              name: "HSS dup 1",
              discriminator: "STANDARD",
              area: 5,
              strongAxisMomentOfInertia: 10,
              weakAxisMomentOfInertia: 4,
              torsionalConstant: 1,
              warpingConstant: 20,
              strongAxisPlasticSectionModulus: 3.5,
              weakAxisPlasticSectionModulus: 2.1,
              strongAxisElasticSectionModulus: 3,
              weakAxisElasticSectionModulus: 2,
            },
            {
              tempId: "dup-1",
              name: "HSS dup 2",
              discriminator: "STANDARD",
              area: 6,
              strongAxisMomentOfInertia: 11,
              weakAxisMomentOfInertia: 5,
              torsionalConstant: 1.2,
              warpingConstant: 22,
              strongAxisPlasticSectionModulus: 4.5,
              weakAxisPlasticSectionModulus: 3.1,
              strongAxisElasticSectionModulus: 4,
              weakAxisElasticSectionModulus: 3,
            },
          ],
        },
      },
    );

    expect(batchCreateResponse.data).toBeUndefined();
    expect(batchCreateResponse.response.status).toBe(400);
  });
});
