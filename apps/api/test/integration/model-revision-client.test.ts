import { afterAll, beforeAll, describe, expect, it } from "bun:test";
import { createApiClient } from "@beamos/openapi-client";
import {
  AreaMomentOfInertiaUnits,
  AreaUnits,
  ForceUnits,
  PressureUnits,
  TorqueUnits,
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

    const createModelResponse = await client.POST("/api/projects", {
      body: {
        name: "Create Revision Operations Model",
        description: "Create base model",
      },
    });

    expect(createModelResponse.error).toBeUndefined();
    expect(createModelResponse.response.status).toBe(200);
    expect(createModelResponse.data).toBeDefined();

    if (!createModelResponse.data) {
      throw new Error("Expected model response");
    }

    const projectId = createModelResponse.data.id;
    const branchName = "main";

    const createRevisionResponse = await client.POST(
      "/api/projects/{projectId}/branches/{branchName}/revisions",
      {
        params: {
          path: { projectId, branchName },
        },
        body: {
          nodes: {
            create: [
              {
                location: {
                  type: "spatial",
                  point: { x: 1, y: 2, z: 3 },
                },
                restraint: {
                  canTranslateAlongX: true,
                  canTranslateAlongY: true,
                  canTranslateAlongZ: true,
                  canRotateAboutX: false,
                  canRotateAboutY: true,
                  canRotateAboutZ: true,
                },
              },
            ],
          },
          sectionProfiles: {
            create: [createSectionProfileInput("sp-rev-create")],
          },
        },
      },
    );

    expect(createRevisionResponse.error).toBeUndefined();
    expect(createRevisionResponse.response.status).toBe(200);
    expect(createRevisionResponse.data).toBeDefined();

    if (!createRevisionResponse.data) {
      throw new Error("Expected create model revision response");
    }

    expect(createRevisionResponse.data.projectId).toBe(projectId);
    expect(createRevisionResponse.data.nodes).toHaveLength(1);
    expect(createRevisionResponse.data.sectionProfiles).toHaveLength(1);
    const createdNodeId = createRevisionResponse.data.nodes[0]?.id;
    expect(createdNodeId).toBeDefined();

    if (!createdNodeId) {
      throw new Error("Expected created node id");
    }

    const deleteRevisionResponse = await client.POST(
      "/api/projects/{projectId}/branches/{branchName}/revisions",
      {
        params: {
          path: { projectId, branchName },
        },
        body: {
          nodes: {
            delete: [createdNodeId],
          },
        },
      },
    );

    expect(deleteRevisionResponse.error).toBeUndefined();
    expect(deleteRevisionResponse.response.status).toBe(200);
    expect(deleteRevisionResponse.data).toBeDefined();

    if (!deleteRevisionResponse.data) {
      throw new Error("Expected delete model revision response");
    }

    expect(deleteRevisionResponse.data.nodes).toHaveLength(0);
    expect(deleteRevisionResponse.data.parentRevisionId).toBe(
      createRevisionResponse.data.id,
    );
  });

  it("gets a branch model revision built by stacking revisions", async () => {
    const client = createApiClient(baseUrl);

    const createModelResponse = await client.POST("/api/projects", {
      body: {
        name: "Stacked Revision Model",
        description: "Create base model",
      },
    });

    expect(createModelResponse.error).toBeUndefined();
    expect(createModelResponse.response.status).toBe(200);
    expect(createModelResponse.data).toBeDefined();

    if (!createModelResponse.data) {
      throw new Error("Expected model response");
    }

    const projectId = createModelResponse.data.id;
    const branchName = "main";

    const materialRev1Response = await client.POST(
      "/api/projects/{projectId}/branches/{branchName}/materials/batch",
      {
        params: {
          path: { projectId, branchName },
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
      "/api/projects/{projectId}/branches/{branchName}/section-profiles/batch",
      {
        params: {
          path: { projectId, branchName },
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
      "/api/projects/{projectId}/branches/{branchName}/element1ds/batch",
      {
        params: {
          path: { projectId, branchName },
        },
        body: {
          element1ds: [
            {
              tempId: "el-rev1",
              startNodeId: Bun.randomUUIDv7(),
              endNodeId: Bun.randomUUIDv7(),
              materialName: "Material Revision 1",
              sectionProfileName: "W12x26",
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

    const elementRev1RevisionId =
      elementRev1Response.data.element1ds[0]?.revisionId;
    expect(elementRev1RevisionId).toBeDefined();

    const materialRev2Response = await client.POST(
      "/api/projects/{projectId}/branches/{branchName}/materials/batch",
      {
        params: {
          path: { projectId, branchName },
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
      "/api/projects/{projectId}/branches/{branchName}/section-profiles/batch",
      {
        params: {
          path: { projectId, branchName },
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
      "/api/projects/{projectId}/branches/{branchName}/element1ds/batch",
      {
        params: {
          path: { projectId, branchName },
        },
        body: {
          element1ds: [
            {
              tempId: "el-rev2",
              startNodeId: Bun.randomUUIDv7(),
              endNodeId: Bun.randomUUIDv7(),
              materialName: "Material Revision 2",
              sectionProfileName: "IPE 200",
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

    const elementRev2RevisionId =
      elementRev2Response.data.element1ds[0]?.revisionId;
    expect(elementRev2RevisionId).toBeDefined();

    const getModelRevisionResponse = await client.GET(
      "/api/projects/{projectId}/branches/{branchName}/revisions",
      {
        params: {
          path: {
            projectId,
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

    expect(getModelRevisionResponse.data.id).toBe(elementRev2RevisionId);
    expect(getModelRevisionResponse.data.nodes).toHaveLength(0);
    expect(
      getModelRevisionResponse.data.materials.map((material) => material.id),
    ).toEqual(expect.arrayContaining([materialRev1Id, materialRev2Id]));
    expect(
      getModelRevisionResponse.data.sectionProfiles.map(
        (sectionProfile) => sectionProfile.id,
      ),
    ).toEqual(
      expect.arrayContaining([sectionProfileRev1Id, sectionProfileRev2Id]),
    );
    expect(
      getModelRevisionResponse.data.element1ds.map(
        (element1d) => element1d.revisionId,
      ),
    ).toEqual(
      expect.arrayContaining([elementRev1RevisionId, elementRev2RevisionId]),
    );

    expect(getModelRevisionResponse.data.materials).toHaveLength(2);
    expect(getModelRevisionResponse.data.sectionProfiles).toHaveLength(2);
    expect(getModelRevisionResponse.data.element1ds).toHaveLength(2);
  });

  it("creates and returns load entities in model revisions", async () => {
    const client = createApiClient(baseUrl);

    const createModelResponse = await client.POST("/api/projects", {
      body: {
        name: "Load Entities Model",
        description: "Create base model",
      },
    });

    expect(createModelResponse.error).toBeUndefined();
    expect(createModelResponse.response.status).toBe(200);
    expect(createModelResponse.data).toBeDefined();

    if (!createModelResponse.data) {
      throw new Error("Expected model response");
    }

    const projectId = createModelResponse.data.id;
    const branchName = "main";

    const createNodeRevisionResponse = await client.POST(
      "/api/projects/{projectId}/branches/{branchName}/revisions",
      {
        params: { path: { projectId, branchName } },
        body: {
          nodes: {
            create: [
              {
                location: { type: "spatial", point: { x: 0, y: 0, z: 0 } },
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
          },
        },
      },
    );

    expect(createNodeRevisionResponse.error).toBeUndefined();
    expect(createNodeRevisionResponse.response.status).toBe(200);
    expect(createNodeRevisionResponse.data?.nodes).toHaveLength(1);
    const nodeId = createNodeRevisionResponse.data?.nodes[0]?.id;
    expect(nodeId).toBeDefined();

    if (!nodeId) {
      throw new Error("Expected node id");
    }

    const createLoadCaseRevisionResponse = await client.POST(
      "/api/projects/{projectId}/branches/{branchName}/revisions",
      {
        params: { path: { projectId, branchName } },
        body: {
          loadCases: {
            create: [{ name: "LC1" }],
          },
        },
      },
    );

    expect(createLoadCaseRevisionResponse.error).toBeUndefined();
    expect(createLoadCaseRevisionResponse.response.status).toBe(200);
    expect(createLoadCaseRevisionResponse.data?.loadCases).toHaveLength(1);
    const loadCaseId = createLoadCaseRevisionResponse.data?.loadCases[0]?.id;
    expect(loadCaseId).toBeDefined();

    if (!loadCaseId) {
      throw new Error("Expected load case id");
    }

    const createLoadsRevisionResponse = await client.POST(
      "/api/projects/{projectId}/branches/{branchName}/revisions",
      {
        params: { path: { projectId, branchName } },
        body: {
          loadCombinations: {
            create: [{ loadCaseFactors: { [loadCaseId]: 1.5 } }],
          },
          pointLoads: {
            create: [
              {
                nodeId,
                loadCaseId,
                force: {
                  forceAlongX: 10,
                  forceAlongY: 20,
                  forceAlongZ: 30,
                  momentAboutX: 1,
                  momentAboutY: 2,
                  momentAboutZ: 3,
                },
                direction: { x: 0, y: 0, z: -1 },
                units: {
                  force: ForceUnits.Newtons,
                  torque: TorqueUnits.NewtonMeters,
                },
              },
            ],
          },
        },
      },
    );

    expect(createLoadsRevisionResponse.error).toBeUndefined();
    expect(createLoadsRevisionResponse.response.status).toBe(200);
    expect(createLoadsRevisionResponse.data?.loadCombinations).toHaveLength(1);
    expect(createLoadsRevisionResponse.data?.pointLoads).toHaveLength(1);
    expect(
      createLoadsRevisionResponse.data?.loadCombinations[0]?.loadCaseFactors[
        loadCaseId
      ],
    ).toBe(1.5);
    expect(
      createLoadsRevisionResponse.data?.pointLoads[0]?.force.forceAlongX,
    ).toBe(10);
    expect(createLoadsRevisionResponse.data?.pointLoads[0]?.units.force).toBe(
      ForceUnits.Newtons,
    );
  });
});
