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

const createSectionProfileInput = (tempId?: string) => ({
  ...(tempId ? { tempId } : {}),
  name: "W12x26",
  discriminator: "STANDARD" as const,
  area: 7.65,
  strongAxisMomentOfInertia: 204,
  weakAxisMomentOfInertia: 17.3,
  torsionalConstant: 0.346,
  warpingConstant: 337,
  strongAxisPlasticSectionModulus: 38.4,
  weakAxisPlasticSectionModulus: 8.94,
  strongAxisElasticSectionModulus: 34,
  weakAxisElasticSectionModulus: 5.77,
});

beforeAll(async () => {
  baseUrl = await setupIntegrationApp();
}, 30_000);

afterAll(async () => {
  await teardownIntegrationApp();
}, 30_000);

describe("model revision integration", () => {
  it("creates a model revision from batched entity operations", async () => {
    const client = createApiClient(baseUrl);

    const createModelResponse = await client.POST("/api/models", {
      body: {
        name: "Create Revision Operations Model",
        authorId: randomUUID(),
        message: "Create base model",
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

    // Initial setup operation: create multiple entities of all types
    const setupRevisionResponse = await client.POST(
      "/api/models/{modelId}/branches/{branchName}/revisions",
      {
        params: {
          path: { modelId, branchName },
        },
        body: {
          nodes: {
            create: [
              {
                tempId: "node-1",
                location: {
                  type: "spatial",
                  point: { x: 0, y: 0, z: 0 },
                },
                restraint: {
                  canTranslateAlongX: false,
                  canTranslateAlongY: false,
                  canTranslateAlongZ: false,
                  canRotateAboutX: false,
                  canRotateAboutY: false,
                  canRotateAboutZ: false,
                },
              },
              {
                tempId: "node-2",
                location: {
                  type: "spatial",
                  point: { x: 10, y: 0, z: 0 },
                },
                restraint: {
                  canTranslateAlongX: true,
                  canTranslateAlongY: true,
                  canTranslateAlongZ: true,
                  canRotateAboutX: true,
                  canRotateAboutY: true,
                  canRotateAboutZ: true,
                },
              },
              {
                tempId: "node-3",
                location: {
                  type: "spatial",
                  point: { x: 20, y: 0, z: 0 },
                },
                restraint: {
                  canTranslateAlongX: true,
                  canTranslateAlongY: true,
                  canTranslateAlongZ: true,
                  canRotateAboutX: true,
                  canRotateAboutY: true,
                  canRotateAboutZ: true,
                },
              },
            ],
            update: [],
            delete: [],
          },
          materials: {
            create: [
              {
                tempId: "mat-1",
                name: "Material 1",
                pressureE: { value: 200, unit: PressureUnits.Gigapascals },
                pressureG: { value: 80, unit: PressureUnits.Gigapascals },
              },
              {
                tempId: "mat-2",
                name: "Material 2",
                pressureE: { value: 210, unit: PressureUnits.Gigapascals },
                pressureG: { value: 85, unit: PressureUnits.Gigapascals },
              },
            ],
            update: [],
            delete: [],
          },
          sectionProfiles: {
            create: [
              createSectionProfileInput("sp-1"),
              {
                tempId: "sp-2",
                name: "W14x30",
                discriminator: "STANDARD" as const,
                area: 8.85,
                strongAxisMomentOfInertia: 291,
                weakAxisMomentOfInertia: 19.6,
                torsionalConstant: 0.462,
                warpingConstant: 450,
                strongAxisPlasticSectionModulus: 47.3,
                weakAxisPlasticSectionModulus: 10.3,
                strongAxisElasticSectionModulus: 41.8,
                weakAxisElasticSectionModulus: 6.55,
              },
            ],
            update: [],
            delete: [],
          },
          element1ds: {
            create: [],
            update: [],
            delete: [],
          },
        },
      },
    );

    expect(setupRevisionResponse.error).toBeUndefined();
    expect(setupRevisionResponse.response.status).toBe(200);
    expect(setupRevisionResponse.data).toBeDefined();

    if (!setupRevisionResponse.data) {
      throw new Error("Expected setup model revision response");
    }

    expect(setupRevisionResponse.data.modelRevision.nodes).toHaveLength(3);
    expect(setupRevisionResponse.data.modelRevision.materials).toHaveLength(2);
    expect(setupRevisionResponse.data.modelRevision.sectionProfiles).toHaveLength(2);

    // Get entity IDs from the setup revision
    // Since we can't access detailed properties from the revision response,
    // we'll use the order they were created
    const nodes = setupRevisionResponse.data.modelRevision.nodes;
    const node1Id = nodes[0]?.id;
    const node2Id = nodes[1]?.id;
    const node3Id = nodes[2]?.id;
    
    const materials = setupRevisionResponse.data.modelRevision.materials;
    const mat1Id = materials[0]?.id;
    const mat2Id = materials[1]?.id;
    
    const sectionProfiles = setupRevisionResponse.data.modelRevision.sectionProfiles;
    const sp1Id = sectionProfiles[0]?.id;
    const sp2Id = sectionProfiles[1]?.id;

    if (!node1Id || !node2Id || !node3Id || !mat1Id || !mat2Id || !sp1Id || !sp2Id) {
      throw new Error("Expected all entity IDs");
    }

    // Second operation: create, update, and delete entities
    const updateRevisionResponse = await client.POST(
      "/api/models/{modelId}/branches/{branchName}/revisions",
      {
        params: {
          path: { modelId, branchName },
        },
        body: {
          nodes: {
            create: [
              {
                tempId: "node-4",
                location: {
                  type: "spatial",
                  point: { x: 30, y: 0, z: 0 },
                },
                restraint: {
                  canTranslateAlongX: true,
                  canTranslateAlongY: true,
                  canTranslateAlongZ: true,
                  canRotateAboutX: true,
                  canRotateAboutY: true,
                  canRotateAboutZ: true,
                },
              },
            ],
            update: [
              {
                id: node3Id,
                nodeTypeDescriminator: "internal" as const,
              },
            ],
            delete: [],
          },
          materials: {
            create: [
              {
                tempId: "mat-3",
                name: "Material 3",
                pressureE: { value: 190, unit: PressureUnits.Gigapascals },
                pressureG: { value: 75, unit: PressureUnits.Gigapascals },
              },
            ],
            update: [
              {
                id: mat2Id,
                name: "Material 2 Updated",
              },
            ],
            delete: [],
          },
          sectionProfiles: {
            create: [
              {
                tempId: "sp-3",
                name: "W16x26",
                discriminator: "STANDARD" as const,
                area: 7.68,
                strongAxisMomentOfInertia: 301,
                weakAxisMomentOfInertia: 9.59,
                torsionalConstant: 0.262,
                warpingConstant: 341,
                strongAxisPlasticSectionModulus: 43.1,
                weakAxisPlasticSectionModulus: 6.31,
                strongAxisElasticSectionModulus: 37.7,
                weakAxisElasticSectionModulus: 3.98,
              },
            ],
            update: [
              {
                id: sp1Id,
                name: "W12x26 Updated",
              },
            ],
            delete: [sp2Id],
          },
          element1ds: {
            create: [
              {
                tempId: "elem-1",
                startNodeId: node1Id,
                endNodeId: node2Id,
                materialId: mat1Id,
                sectionProfileId: sp1Id,
              },
              {
                tempId: "elem-2",
                startNodeId: node2Id,
                endNodeId: node3Id,
                materialId: mat2Id,
                sectionProfileId: sp1Id,
              },
            ],
            update: [],
            delete: [],
          },
        },
      },
    );

    expect(updateRevisionResponse.error).toBeUndefined();
    expect(updateRevisionResponse.response.status).toBe(200);
    expect(updateRevisionResponse.data).toBeDefined();

    if (!updateRevisionResponse.data) {
      throw new Error("Expected update model revision response");
    }

    expect(updateRevisionResponse.data.modelRevision.parentRevisionId).toBe(
      setupRevisionResponse.data.modelRevision.id,
    );

    // Get the final model revision state for snapshot verification
    const finalRevisionResponse = await client.GET(
      "/api/models/{modelId}/branches/{branchName}/revision",
      {
        params: {
          path: { modelId, branchName },
        },
      },
    );

    expect(finalRevisionResponse.error).toBeUndefined();
    expect(finalRevisionResponse.response.status).toBe(200);
    expect(finalRevisionResponse.data).toBeDefined();

    if (!finalRevisionResponse.data) {
      throw new Error("Expected final model revision response");
    }

    // Create snapshot with normalized IDs for consistent testing
    const modelSnapshot = {
      nodes: finalRevisionResponse.data.modelRevision.nodes.map((node) => ({
        ...node,
        id: "<node-id>",
        modelId: "<model-id>",
      })),
      materials: finalRevisionResponse.data.modelRevision.materials.map((material) => ({
        ...material,
        id: "<material-id>",
        modelId: "<model-id>",
        revisionId: "<revision-id>",
      })),
      sectionProfiles: finalRevisionResponse.data.modelRevision.sectionProfiles.map(
        (sp) => ({
          ...sp,
          id: "<section-profile-id>",
          modelId: "<model-id>",
          revisionId: "<revision-id>",
        }),
      ),
      element1ds: finalRevisionResponse.data.modelRevision.element1ds.map((elem) => ({
        ...elem,
        id: "<element-id>",
        modelId: "<model-id>",
        startNodeId: "<node-id>",
        endNodeId: "<node-id>",
        materialId: "<material-id>",
        sectionProfileId: "<section-profile-id>",
        revisionId: "<revision-id>",
      })),
    };

    expect(modelSnapshot).toMatchSnapshot();
  });

  it("gets a branch model revision built by stacking revisions", async () => {
    const client = createApiClient(baseUrl);

    const createModelResponse = await client.POST("/api/models", {
      body: {
        name: "Stacked Revision Model",
        authorId: randomUUID(),
        message: "Create base model",
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

    const materialRev1Response = await client.POST(
      "/api/models/{modelId}/branches/{branchName}/materials/batch",
      {
        params: {
          path: { modelId, branchName },
        },
        body: {
          materials: [
            {
              tempId: "mat-rev1",
              name: "Material Revision 1",
              modulusOfElasticity: 110000, // 1.1 Bars
              modulusOfRigidity: 75000, // 75 Kilopascals
              units: { pressure: PressureUnits.Pascals },
            },
          ],
        },
      },
    );

    expect(materialRev1Response.error).toBeUndefined();
    expect(materialRev1Response.response.status).toBe(200);
    expect(materialRev1Response.data).toBeDefined();

    if (!materialRev1Response.data) {
      throw new Error("Expected material batch response for revision 1");
    }

    const materialRev1Id = materialRev1Response.data.tempIdToId["mat-rev1"];

    const sectionProfileRev1Response = await client.POST(
      "/api/models/{modelId}/branches/{branchName}/section-profiles/batch",
      {
        params: {
          path: { modelId, branchName },
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
              tempId: "sp-rev1",
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

    expect(sectionProfileRev1Response.error).toBeUndefined();
    expect(sectionProfileRev1Response.response.status).toBe(200);
    expect(sectionProfileRev1Response.data).toBeDefined();

    if (!sectionProfileRev1Response.data) {
      throw new Error("Expected section profile batch response for revision 1");
    }

    const sectionProfileRev1Id =
      sectionProfileRev1Response.data.tempIdToId["sp-rev1"];

    const elementRev1Response = await client.POST(
      "/api/models/{modelId}/branches/{branchName}/element1ds/batch",
      {
        params: {
          path: { modelId, branchName },
        },
        body: {
          element1ds: [
            {
              tempId: "el-rev1",
              startNodeId: Bun.randomUUIDv7(),
              endNodeId: Bun.randomUUIDv7(),
              materialId: materialRev1Id,
              sectionProfileId: sectionProfileRev1Id,
            },
          ],
        },
      },
    );

    expect(elementRev1Response.error).toBeUndefined();
    expect(elementRev1Response.response.status).toBe(200);
    expect(elementRev1Response.data).toBeDefined();

    if (!elementRev1Response.data) {
      throw new Error("Expected element1d batch response for revision 1");
    }

    const elementRev1RevisionId = elementRev1Response.data.element1ds[0]?.revisionId;
    expect(elementRev1RevisionId).toBeDefined();

    const materialRev2Response = await client.POST(
      "/api/models/{modelId}/branches/{branchName}/materials/batch",
      {
        params: {
          path: { modelId, branchName },
        },
        body: {
          materials: [
            {
              tempId: "mat-rev2",
              name: "Material Revision 2",
              modulusOfElasticity: 250000, // 2.5 Bars
              modulusOfRigidity: 120000, // 120 Kilopascals
              units: { pressure: PressureUnits.Pascals },
            },
          ],
        },
      },
    );

    expect(materialRev2Response.error).toBeUndefined();
    expect(materialRev2Response.response.status).toBe(200);
    expect(materialRev2Response.data).toBeDefined();

    if (!materialRev2Response.data) {
      throw new Error("Expected material batch response for revision 2");
    }

    const materialRev2Id = materialRev2Response.data.tempIdToId["mat-rev2"];

    const sectionProfileRev2Response = await client.POST(
      "/api/models/{modelId}/branches/{branchName}/section-profiles/batch",
      {
        params: {
          path: { modelId, branchName },
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
              tempId: "sp-rev2",
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

    expect(sectionProfileRev2Response.error).toBeUndefined();
    expect(sectionProfileRev2Response.response.status).toBe(200);
    expect(sectionProfileRev2Response.data).toBeDefined();

    if (!sectionProfileRev2Response.data) {
      throw new Error("Expected section profile batch response for revision 2");
    }

    const sectionProfileRev2Id =
      sectionProfileRev2Response.data.tempIdToId["sp-rev2"];

    const elementRev2Response = await client.POST(
      "/api/models/{modelId}/branches/{branchName}/element1ds/batch",
      {
        params: {
          path: { modelId, branchName },
        },
        body: {
          element1ds: [
            {
              tempId: "el-rev2",
              startNodeId: Bun.randomUUIDv7(),
              endNodeId: Bun.randomUUIDv7(),
              materialId: materialRev2Id,
              sectionProfileId: sectionProfileRev2Id,
            },
          ],
        },
      },
    );

    expect(elementRev2Response.error).toBeUndefined();
    expect(elementRev2Response.response.status).toBe(200);
    expect(elementRev2Response.data).toBeDefined();

    if (!elementRev2Response.data) {
      throw new Error("Expected element1d batch response for revision 2");
    }

    const elementRev2RevisionId = elementRev2Response.data.element1ds[0]?.revisionId;
    expect(elementRev2RevisionId).toBeDefined();

    const getModelRevisionResponse = await client.GET(
      "/api/models/{modelId}/branches/{branchName}/revision",
      {
        params: {
          path: {
            modelId,
            branchName,
          },
        },
      },
    );

    expect(getModelRevisionResponse.error).toBeUndefined();
    expect(getModelRevisionResponse.response.status).toBe(200);
    expect(getModelRevisionResponse.data).toBeDefined();

    if (!getModelRevisionResponse.data) {
      throw new Error("Expected get model revision response");
    }

    expect(getModelRevisionResponse.data.modelRevision.id).toBe(
      elementRev2RevisionId,
    );
    expect(getModelRevisionResponse.data.modelRevision.nodes).toHaveLength(0);
    expect(
      getModelRevisionResponse.data.modelRevision.materials.map(
        (material) => material.id,
      ),
    ).toEqual(expect.arrayContaining([materialRev1Id, materialRev2Id]));
    expect(
      getModelRevisionResponse.data.modelRevision.sectionProfiles.map(
        (sectionProfile) => sectionProfile.id,
      ),
    ).toEqual(
      expect.arrayContaining([sectionProfileRev1Id, sectionProfileRev2Id]),
    );
    expect(
      getModelRevisionResponse.data.modelRevision.element1ds.map(
        (element1d) => element1d.revisionId,
      ),
    ).toEqual(
      expect.arrayContaining([elementRev1RevisionId, elementRev2RevisionId]),
    );

    expect(getModelRevisionResponse.data.modelRevision.materials).toHaveLength(
      2,
    );
    expect(
      getModelRevisionResponse.data.modelRevision.sectionProfiles,
    ).toHaveLength(2);
    expect(getModelRevisionResponse.data.modelRevision.element1ds).toHaveLength(
      2,
    );
  });
});
