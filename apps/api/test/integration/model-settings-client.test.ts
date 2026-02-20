import { describe, expect, it } from "bun:test";
import { createApiClient } from "@beamos/openapi-client";
import { getIntegrationBaseUrl } from "./shared-test-app";


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


describe("model settings integration", () => {
    it("stores latest model settings in model revision aggregate", async () => {
        const client = createApiClient(getIntegrationBaseUrl());

        const createModelResponse = await client.POST("/api/projects", {
            body: {
                name: "Model Settings Model",
                description: "Create model for settings test",
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

        const firstResponse = await client.POST(
            "/api/projects/{projectId}/branches/{branchName}/revisions",
            {
                params: {
                    path: { projectId, branchName },
                },
                body: {
                    modelSettings: {
                        units: {
                            pressure: "Pascal",
                            area: "SquareMeter",
                            areaMomentOfInertia: "MeterToTheFourth",
                            warpingMomentOfInertia: "MeterToTheSixth",
                            volume: "CubicMeter",
                        },
                        yAxisUp: true,
                    },
                },
            },
        );

        expect(firstResponse.error).toBeUndefined();
        expect(firstResponse.response.status).toBe(200);

        const secondResponse = await client.POST(
            "/api/projects/{projectId}/branches/{branchName}/revisions",
            {
                params: {
                    path: { projectId, branchName },
                },
                body: {
                    modelSettings: {
                        units: {
                            pressure: "Bar",
                            area: "SquareFoot",
                            areaMomentOfInertia: "FootToTheFourth",
                            warpingMomentOfInertia: "FootToTheSixth",
                            volume: "CubicFoot",
                        },
                        yAxisUp: false,
                    },
                },
            },
        );

        expect(secondResponse.error).toBeUndefined();
        expect(secondResponse.response.status).toBe(200);

        const createEntitiesResponse = await client.POST(
            "/api/projects/{projectId}/branches/{branchName}/revisions",
            {
                params: {
                    path: { projectId, branchName },
                },
                body: {
                    materials: {
                        create: [
                            {
                                name: "A36",
                                modulusOfElasticity: 2,
                                modulusOfRigidity: 1,
                                units: { pressure: "Bar" },
                            },
                        ],
                    },
                    sectionProfiles: {
                        create: [
                            {
                                name: "W12x26",
                                discriminator: "STANDARD",
                                area: 10,
                                strongAxisMomentOfInertia: 20,
                                weakAxisMomentOfInertia: 30,
                                torsionalConstant: 40,
                                warpingConstant: 50,
                                strongAxisPlasticSectionModulus: 60,
                                weakAxisPlasticSectionModulus: 70,
                                strongAxisElasticSectionModulus: 80,
                                weakAxisElasticSectionModulus: 90,
                                units: {
                                    area: "SquareFoot",
                                    areaMomentOfInertia: "FootToTheFourth",
                                    warpingMomentOfInertia: "FootToTheSixth",
                                    volume: "CubicFoot",
                                },
                            },
                        ],
                    },
                },
            },
        );

        expect(createEntitiesResponse.error).toBeUndefined();
        expect(createEntitiesResponse.response.status).toBe(200);
        expect(createEntitiesResponse.data?.materials[0]?.units.pressure).toBe("Pascal");
        expect(createEntitiesResponse.data?.sectionProfiles[0]?.area.unit).toBe("SquareMeter");

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

        if (!getRevisionResponse.data) {
            throw new Error("Expected model revision response");
        }

        const modelRevision = getRevisionResponse.data;

        expect(modelRevision.modelSettings).toEqual({
            id: expect.any(String),
            revisionId: expect.any(String),
            units: {
                pressure: "Bar",
                area: "SquareFoot",
                areaMomentOfInertia: "FootToTheFourth",
                warpingMomentOfInertia: "FootToTheSixth",
                volume: "CubicFoot",
            },
            yAxisUp: false,
        });
        expect(modelRevision.materials[0]?.units.pressure).toBe("Bar");
        expect(modelRevision.sectionProfiles[0]?.area.unit).toBe("SquareFoot");

        const getRevisionResponseWithSiUnits = await client.GET(
            "/api/projects/{projectId}/branches/{branchName}",
            {
                params: {
                    path: { projectId, branchName },
                    query: { units: "SI" },
                },
            },
        );

        expect(getRevisionResponseWithSiUnits.error).toBeUndefined();
        expect(getRevisionResponseWithSiUnits.response.status).toBe(200);
        expect(getRevisionResponseWithSiUnits.data).toBeDefined();

        if (!getRevisionResponseWithSiUnits.data) {
            throw new Error("Expected model revision response");
        }

        expect(getRevisionResponseWithSiUnits.data.modelSettings.units).toEqual({
            pressure: "Bar",
            area: "SquareFoot",
            areaMomentOfInertia: "FootToTheFourth",
            warpingMomentOfInertia: "FootToTheSixth",
            volume: "CubicFoot",
        });
        expect(getRevisionResponseWithSiUnits.data.materials[0]?.units.pressure).toBe("Pascal");
        expect(getRevisionResponseWithSiUnits.data.sectionProfiles[0]?.area.unit).toBe(
            "SquareMeter",
        );
    });
});
