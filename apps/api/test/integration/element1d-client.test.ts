import { afterAll, beforeAll, describe, expect, it } from "bun:test";
import { createApiClient } from "@beamos/openapi-client";
import { PressureUnits } from "unitsnet-js";
import { setupIntegrationApp, teardownIntegrationApp } from "./shared-test-app";

let baseUrl = "";

const defaultModelSettings = {
    units: {
        pressure: "Pascal",
        area: "SquareMeter",
        areaMomentOfInertia: "MeterToTheFourth",
        warpingMomentOfInertia: "MeterToTheSixth",
        volume: "CubicMeter",
    },
    yAxisUp: true,
} as const;

beforeAll(async () => {
    baseUrl = await setupIntegrationApp();
}, 30_000);

afterAll(async () => {
    await teardownIntegrationApp();
}, 30_000);

describe("typed element1d api client integration", () => {
    it("batch creates element1ds and snapshots get responses", async () => {
        const client = createApiClient(baseUrl);
        const tempIds = ["el-01", "el-02"];

        const createModelResponse = await client.POST("/api/projects", {
            body: {
                name: "Element1d Integration Model",
                description: "Create model for element1d test",
                modelSettings: defaultModelSettings,
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

        const materialBatchCreateResponse = await client.POST(
            "/api/projects/{projectId}/branches/{branchName}/revisions",
            {
                params: {
                    path: {
                        projectId,
                        branchName,
                    },
                },
                body: {
                    materials: {
                        create: [
                            {
                                name: "Material for Element1d",
                                modulusOfElasticity: 125000, // 1.25 Bars
                                modulusOfRigidity: 85000, // 85 Kilopascals
                                units: { pressure: PressureUnits.Pascals },
                            },
                        ],
                    },
                },
            },
        );

        expect(materialBatchCreateResponse.error).toBeUndefined();
        expect(materialBatchCreateResponse.response.status).toBe(200);
        expect(materialBatchCreateResponse.data?.materials).toHaveLength(1);

        if (!materialBatchCreateResponse.data) {
            throw new Error("Expected material batch response");
        }

        const materialName = "Material for Element1d";

        const sectionProfileBatchCreateResponse = await client.POST(
            "/api/projects/{projectId}/branches/{branchName}/revisions",
            {
                params: {
                    path: {
                        projectId,
                        branchName,
                    },
                },
                body: {
                    sectionProfiles: {
                        create: [
                            {
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
                                units: {
                                    area: "SquareInch",
                                    areaMomentOfInertia: "InchToTheFourth",
                                    warpingMomentOfInertia: "InchToTheSixth",
                                    volume: "CubicInch",
                                },
                            },
                        ],
                    },
                },
            },
        );

        expect(sectionProfileBatchCreateResponse.error).toBeUndefined();
        expect(sectionProfileBatchCreateResponse.response.status).toBe(200);
        expect(sectionProfileBatchCreateResponse.data?.sectionProfiles).toHaveLength(1);

        if (!sectionProfileBatchCreateResponse.data) {
            throw new Error("Expected section profile batch response");
        }

        const sectionProfileName = "W12x26";

        const batchCreateResponse = await client.POST(
            "/api/projects/{projectId}/branches/{branchName}/revisions",
            {
                params: {
                    path: {
                        projectId,
                        branchName,
                    },
                },
                body: {
                    element1ds: {
                        create: [
                            {
                                tempId: tempIds[0],
                                startNodeId: Bun.randomUUIDv7(),
                                endNodeId: Bun.randomUUIDv7(),
                                materialName,
                                sectionProfileName,
                            },
                            {
                                tempId: tempIds[1],
                                startNodeId: Bun.randomUUIDv7(),
                                endNodeId: Bun.randomUUIDv7(),
                                materialName,
                                sectionProfileName,
                            },
                        ],
                    },
                },
            },
        );

        expect(batchCreateResponse.error).toBeUndefined();
        expect(batchCreateResponse.response.status).toBe(200);
        expect(batchCreateResponse.data).toBeDefined();
        expect(batchCreateResponse.data?.element1ds).toHaveLength(tempIds.length);

        if (!batchCreateResponse.data) {
            throw new Error("Expected batch create response");
        }

        const createdElement1dIds = batchCreateResponse.data.element1ds.map(
            (element1d) => element1d.id,
        );

        const getRevisionResponse = await client.GET(
            "/api/projects/{projectId}/branches/{branchName}",
            {
                params: {
                    path: { projectId, branchName },
                },
            },
        );

        expect(getRevisionResponse.error).toBeUndefined();
        expect(getRevisionResponse.response.status).toBe(200);
        expect(getRevisionResponse.data).toBeDefined();
        expect(getRevisionResponse.data?.element1ds.map((element1d) => element1d.id)).toEqual(
            expect.arrayContaining(createdElement1dIds),
        );

        for (const element1dId of createdElement1dIds) {
            expect(element1dId).toBeDefined();

            if (!element1dId) {
                throw new Error("Expected element1d ID");
            }

            const getResponse = await client.GET(
                "/api/projects/{projectId}/branches/{branchName}/element1ds/{element1dId}",
                {
                    params: {
                        path: { projectId, branchName, element1dId },
                    },
                },
            );

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

        const createModelResponse = await client.POST("/api/projects", {
            body: {
                name: "Duplicate Element1d TempId Model",
                description: "Create model for duplicate element1d tempId test",
                modelSettings: defaultModelSettings,
            },
        });

        expect(createModelResponse.response.status).toBe(200);
        expect(createModelResponse.data).toBeDefined();

        if (!createModelResponse.data) {
            throw new Error("Expected model response");
        }

        const projectId = createModelResponse.data.id;
        const branchName = "main";

        const createDependenciesResponse = await client.POST(
            "/api/projects/{projectId}/branches/{branchName}/revisions",
            {
                params: {
                    path: {
                        projectId,
                        branchName,
                    },
                },
                body: {
                    materials: {
                        create: [
                            {
                                name: "Material A",
                                modulusOfElasticity: 1,
                                modulusOfRigidity: 1,
                                units: { pressure: PressureUnits.Pascals },
                            },
                            {
                                name: "Material B",
                                modulusOfElasticity: 2,
                                modulusOfRigidity: 2,
                                units: { pressure: PressureUnits.Pascals },
                            },
                        ],
                    },
                    sectionProfiles: {
                        create: [
                            {
                                name: "Section A",
                                area: 5,
                                strongAxisMomentOfInertia: 10,
                                weakAxisMomentOfInertia: 4,
                                torsionalConstant: 1,
                                warpingConstant: 20,
                                strongAxisPlasticSectionModulus: 3.5,
                                weakAxisPlasticSectionModulus: 2.1,
                                strongAxisElasticSectionModulus: 3,
                                weakAxisElasticSectionModulus: 2,
                                units: {
                                    area: "SquareMeter",
                                    areaMomentOfInertia: "MeterToTheFourth",
                                    warpingMomentOfInertia: "MeterToTheSixth",
                                    volume: "CubicMeter",
                                },
                            },
                            {
                                name: "Section B",
                                area: 6,
                                strongAxisMomentOfInertia: 11,
                                weakAxisMomentOfInertia: 5,
                                torsionalConstant: 1.2,
                                warpingConstant: 22,
                                strongAxisPlasticSectionModulus: 4.5,
                                weakAxisPlasticSectionModulus: 3.1,
                                strongAxisElasticSectionModulus: 4,
                                weakAxisElasticSectionModulus: 3,
                                units: {
                                    area: "SquareMeter",
                                    areaMomentOfInertia: "MeterToTheFourth",
                                    warpingMomentOfInertia: "MeterToTheSixth",
                                    volume: "CubicMeter",
                                },
                            },
                        ],
                    },
                },
            },
        );

        expect(createDependenciesResponse.error).toBeUndefined();
        expect(createDependenciesResponse.response.status).toBe(200);

        const batchCreateResponse = await client.POST(
            "/api/projects/{projectId}/branches/{branchName}/revisions",
            {
                params: {
                    path: {
                        projectId,
                        branchName,
                    },
                },
                body: {
                    element1ds: {
                        create: [
                            {
                                tempId: "dup-1",
                                startNodeId: Bun.randomUUIDv7(),
                                endNodeId: Bun.randomUUIDv7(),
                                materialName: "Material A",
                                sectionProfileName: "Section A",
                            },
                            {
                                tempId: "dup-1",
                                startNodeId: Bun.randomUUIDv7(),
                                endNodeId: Bun.randomUUIDv7(),
                                materialName: "Material B",
                                sectionProfileName: "Section B",
                            },
                        ],
                    },
                },
            },
        );

        expect(batchCreateResponse.data).toBeUndefined();
        expect(batchCreateResponse.response.status).toBe(400);
    });
});
