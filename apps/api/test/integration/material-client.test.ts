import { describe, expect, it } from "bun:test";
import { createApiClient } from "@beamos/openapi-client";
import { PressureUnits } from "unitsnet-js";
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


describe("typed material api client integration", () => {
    it("batch creates materials and snapshots get responses", async () => {
        const client = createApiClient(getIntegrationBaseUrl());
        const materialNames = ["Material 1", "Material 2", "Material 3"];

        const createModelResponse = await client.POST("/api/projects", {
            body: {
                name: "Material Integration Model",
                description: "Create model for materials test",
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
                    materials: {
                        create: [
                            {
                                name: materialNames[0],
                                modulusOfElasticity: 125000, // 1.25 Bars
                                modulusOfRigidity: 85000, // 85 Kilopascals
                                units: { pressure: PressureUnits.Pascals },
                            },
                            {
                                name: materialNames[1],
                                modulusOfElasticity: 101324.66370467292, // 14.6959 PoundsForcePerSquareInch
                                modulusOfRigidity: 101325, // 1013.25 Millibars
                                units: { pressure: PressureUnits.Pascals },
                            },
                            {
                                name: materialNames[2],
                                modulusOfElasticity: 96258.75, // 0.95 Atmospheres
                                modulusOfRigidity: 95000, // 950 Hectopascals
                                units: { pressure: PressureUnits.Pascals },
                            },
                        ],
                    },
                },
            },
        );

        expect(batchCreateResponse.error).toBeUndefined();
        expect(batchCreateResponse.response.status).toBe(200);
        expect(batchCreateResponse.data).toBeDefined();
        expect(batchCreateResponse.data?.materials).toHaveLength(materialNames.length);

        if (!batchCreateResponse.data) {
            throw new Error("Expected batch create response");
        }

        const materialIdByName = new Map(
            batchCreateResponse.data.materials.map(
                (material) => [material.name, material.id] as const,
            ),
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
        expect(getRevisionResponse.data?.materials.map((material) => material.id)).toEqual(
            expect.arrayContaining(Array.from(materialIdByName.values())),
        );

        for (const materialName of materialNames) {
            const materialId = materialIdByName.get(materialName);
            expect(materialId).toBeDefined();

            if (!materialId) {
                throw new Error(`Expected material ID for material name ${materialName}`);
            }

            const getResponse = await client.GET(
                "/api/projects/{projectId}/branches/{branchName}/materials/{materialId}",
                {
                    params: {
                        path: { projectId, branchName, materialId },
                    },
                },
            );

            expect(getResponse.error).toBeUndefined();
            expect(getResponse.response.status).toBe(200);
            expect(getResponse.data).toBeDefined();

            if (!getResponse.data) {
                throw new Error(`Expected material response for ${materialId}`);
            }

            expect({
                ...getResponse.data,
                id: "<db-id>",
                revisionId: "<revision-id>",
            }).toMatchSnapshot();
        }
    });

    it("rejects duplicate material names in batch create", async () => {
        const client = createApiClient(getIntegrationBaseUrl());

        const createModelResponse = await client.POST("/api/projects", {
            body: {
                name: "Duplicate Material Name Model",
                description: "Create model for duplicate material name test",
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
                    materials: {
                        create: [
                            {
                                name: "Duplicate Name",
                                modulusOfElasticity: 1,
                                modulusOfRigidity: 1,
                                units: { pressure: PressureUnits.Bars },
                            },
                            {
                                name: "Duplicate Name",
                                modulusOfElasticity: 2,
                                modulusOfRigidity: 2,
                                units: { pressure: PressureUnits.Bars },
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
