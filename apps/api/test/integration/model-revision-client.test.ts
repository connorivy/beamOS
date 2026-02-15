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

    const sectionProfileKeepId = randomUUID();
    const sectionProfileDeleteId = randomUUID();

    const createRevisionResponse = await client.POST(
      "/api/models/{modelId}/branches/{branchName}/revisions",
      {
        params: {
          path: { modelId, branchName },
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
                  rx: true,
                },
              },
              {
                location: {
                  type: "internal",
                  element1dId: Bun.randomUUIDv7(),
                  ratioAlongElement1d: 0.5,
                },
              },
            ],
            update: [],
            delete: [],
          },
          materials: {
            create: [
              {
                name: "Material Keep",
                pressureE: { value: 101, unit: PressureUnits.Pascals },
                pressureG: { value: 202, unit: PressureUnits.Pascals },
              },
              {
                name: "Material Delete",
                pressureE: { value: 303, unit: PressureUnits.Pascals },
                pressureG: { value: 404, unit: PressureUnits.Pascals },
              },
            ],
            update: [],
            delete: [],
          },
          sectionProfiles: {
            create: [
              {
                id: sectionProfileKeepId,
                name: "Section Keep",
              },
              {
                id: sectionProfileDeleteId,
                name: "Section Delete",
              },
            ],
            update: [],
            delete: [],
          },
          element1ds: {
            create: [
              {
                startNodeId: Bun.randomUUIDv7(),
                endNodeId: Bun.randomUUIDv7(),
                materialId: Bun.randomUUIDv7(),
                sectionProfileId: Bun.randomUUIDv7(),
              },
            ],
            update: [],
            delete: [],
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

    const initialModelRevision = createRevisionResponse.data.modelRevision;
    const externalNodeId = initialModelRevision.nodes.find(
      (node) => node.nodeTypeDescriminator === "external",
    )?.id;
    const internalNodeId = initialModelRevision.nodes.find(
      (node) => node.nodeTypeDescriminator === "internal",
    )?.id;
    const materialKeepId = initialModelRevision.materials.find(
      (material) => material.pressureE.value === 101,
    )?.id;
    const materialDeleteId = initialModelRevision.materials.find(
      (material) => material.pressureE.value === 303,
    )?.id;
    const initialElementId = initialModelRevision.element1ds[0]?.id;

    if (
      !externalNodeId ||
      !internalNodeId ||
      !materialKeepId ||
      !materialDeleteId ||
      !initialElementId
    ) {
      throw new Error("Expected initial model revision entities");
    }

    const secondRevisionResponse = await client.POST(
      "/api/models/{modelId}/branches/{branchName}/revisions",
      {
        params: {
          path: { modelId, branchName },
        },
        body: {
          nodes: {
            create: [
              {
                location: {
                  type: "spatial",
                  point: { x: 7, y: 8, z: 9 },
                },
              },
            ],
            update: [],
            delete: [internalNodeId],
          },
          materials: {
            create: [
              {
                name: "Material Created",
                pressureE: { value: 505, unit: PressureUnits.Pascals },
                pressureG: { value: 606, unit: PressureUnits.Pascals },
              },
            ],
            update: [],
            delete: [materialDeleteId],
          },
          sectionProfiles: {
            create: [{ id: randomUUID(), name: "Section Created" }],
            update: [{ id: sectionProfileKeepId, name: "Section Keep Updated" }],
            delete: [sectionProfileDeleteId],
          },
          element1ds: {
            create: [
              {
                startNodeId: externalNodeId,
                endNodeId: externalNodeId,
                materialId: materialKeepId,
                sectionProfileId: sectionProfileKeepId,
              },
            ],
            update: [],
            delete: [initialElementId],
          },
        },
      },
    );

    expect(secondRevisionResponse.error).toBeUndefined();
    expect(secondRevisionResponse.response.status).toBe(200);
    expect(secondRevisionResponse.data).toBeDefined();

    if (!secondRevisionResponse.data) {
      throw new Error("Expected second model revision response");
    }

    const finalModelRevision = secondRevisionResponse.data.modelRevision;
    expect({
      nodes: finalModelRevision.nodes
        .map((node) => node.nodeTypeDescriminator)
        .sort(),
      materials: finalModelRevision.materials
        .map((material) => ({
          pressureE: material.pressureE.value,
          pressureG: material.pressureG.value,
        }))
        .sort((a, b) => a.pressureE - b.pressureE),
      sectionProfiles: finalModelRevision.sectionProfiles
        .map((sectionProfile) => sectionProfile.name)
        .sort(),
      element1ds: finalModelRevision.element1ds.map((element1d) => ({
        startNodeInModel: finalModelRevision.nodes.some(
          (node) => node.id === element1d.startNodeId,
        ),
        endNodeInModel: finalModelRevision.nodes.some(
          (node) => node.id === element1d.endNodeId,
        ),
        materialInModel: finalModelRevision.materials.some(
          (material) => material.id === element1d.materialId,
        ),
        sectionProfileInModel: finalModelRevision.sectionProfiles.some(
          (sectionProfile) => sectionProfile.id === element1d.sectionProfileId,
        ),
      })),
    }).toMatchSnapshot();
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
              pressureE: { value: 1.1, unit: PressureUnits.Bars },
              pressureG: { value: 75, unit: PressureUnits.Kilopascals },
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
              pressureE: { value: 2.5, unit: PressureUnits.Bars },
              pressureG: { value: 120, unit: PressureUnits.Kilopascals },
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
