import { afterAll, beforeAll, describe, expect, it } from "bun:test";
import { createApiClient } from "@beamos/openapi-client";
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

describe("typed section profile api client integration", () => {
    it("batch creates section profiles and snapshots get responses", async () => {
        const client = createApiClient(baseUrl);

        const createModelResponse = await client.POST("/api/projects", {
            body: {
                name: "Section Profile Integration Model",
                description: "Create model for section profile test",
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

        const imperialBatchCreateResponse = await client.POST(
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

        const metricBatchCreateResponse = await client.POST(
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
                                units: {
                                    area: "SquareCentimeter",
                                    areaMomentOfInertia: "CentimeterToTheFourth",
                                    warpingMomentOfInertia: "CentimeterToTheSixth",
                                    volume: "CubicCentimeter",
                                },
                            },
                        ],
                    },
                },
            },
        );

        expect(imperialBatchCreateResponse.error).toBeUndefined();
        expect(imperialBatchCreateResponse.response.status).toBe(200);
        expect(imperialBatchCreateResponse.data).toBeDefined();
        expect(imperialBatchCreateResponse.data?.sectionProfiles).toHaveLength(1);

        expect(metricBatchCreateResponse.error).toBeUndefined();
        expect(metricBatchCreateResponse.response.status).toBe(200);
        expect(metricBatchCreateResponse.data).toBeDefined();
        expect(metricBatchCreateResponse.data?.sectionProfiles).toHaveLength(2);

        if (!imperialBatchCreateResponse.data || !metricBatchCreateResponse.data) {
            throw new Error("Expected batch create responses");
        }

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

        const createdIds = [
            imperialBatchCreateResponse.data.sectionProfiles.find(
                (sectionProfile) => sectionProfile.name === "W12x26",
            )?.id,
            metricBatchCreateResponse.data.sectionProfiles.find(
                (sectionProfile) => sectionProfile.name === "IPE 200",
            )?.id,
        ];

        expect(createdIds[0]).toBeDefined();
        expect(createdIds[1]).toBeDefined();

        expect(
            getRevisionResponse.data?.sectionProfiles.map((sectionProfile) => sectionProfile.id),
        ).toEqual(expect.arrayContaining(createdIds as string[]));

        for (const sectionProfileId of createdIds) {
            if (!sectionProfileId) {
                throw new Error("Expected section profile id");
            }

            const getResponse = await client.GET(
                "/api/projects/{projectId}/branches/{branchName}/section-profiles/{sectionProfileId}",
                {
                    params: {
                        path: { projectId, branchName, sectionProfileId },
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

    it("rejects duplicate section profile names in batch create", async () => {
        const client = createApiClient(baseUrl);

        const createModelResponse = await client.POST("/api/projects", {
            body: {
                name: "Duplicate Section Profile Name Model",
                description: "Create model for duplicate section profile name test",
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
                    sectionProfiles: {
                        create: [
                            {
                                name: "Duplicate Section",
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
                                units: {
                                    area: "SquareMeter",
                                    areaMomentOfInertia: "MeterToTheFourth",
                                    warpingMomentOfInertia: "MeterToTheSixth",
                                    volume: "CubicMeter",
                                },
                            },
                            {
                                name: "Duplicate Section",
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

        expect(batchCreateResponse.data).toBeUndefined();
        expect(batchCreateResponse.response.status).toBe(400);
    });
});
