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


describe("typed openapi client integration", () => {
    it("creates a model", async () => {
        const client = createApiClient(getIntegrationBaseUrl());

        const requestBody = {
            name: "Integration Test Model",
            description: "Create a model through typed OpenAPI client",
            modelSettings: defaultModelSettings,
        };

        const { data, error, response } = await client.POST("/api/projects", {
            body: requestBody,
        });

        expect(error).toBeUndefined();
        expect(response.status).toBe(200);
        expect(data).toBeDefined();

        if (!data) {
            throw new Error("Expected response body from create model API");
        }

        expect(data.name).toBe(requestBody.name);
        expect(data.id).toMatch(/^[0-9a-f-]{36}$/i);
        expect(data.description).toBe(requestBody.description);

        const mainBranchRevisionResponse = await client.GET(
            "/api/projects/{projectId}/branches/{branchName}",
            {
                params: {
                    path: {
                        projectId: data.id,
                        branchName: "main",
                    },
                },
            },
        );

        expect(mainBranchRevisionResponse.error).toBeUndefined();
        expect(mainBranchRevisionResponse.response.status).toBe(200);
        expect(mainBranchRevisionResponse.data).toBeDefined();

        if (!mainBranchRevisionResponse.data) {
            throw new Error("Expected response body from get model revision API");
        }

        expect(mainBranchRevisionResponse.data.id).toMatch(/^[0-9a-f-]{36}$/i);
        expect(mainBranchRevisionResponse.data.parentRevisionId).toBeNull();
        expect(mainBranchRevisionResponse.data.nodes).toHaveLength(0);
        expect(mainBranchRevisionResponse.data.materials).toHaveLength(0);
        expect(mainBranchRevisionResponse.data.modelSettings).toEqual({
            id: expect.any(String),
            revisionId: expect.any(String),
            units: defaultModelSettings.units,
            yAxisUp: defaultModelSettings.yAxisUp,
        });
        expect(mainBranchRevisionResponse.data.sectionProfiles).toHaveLength(0);
        expect(mainBranchRevisionResponse.data.element1ds).toHaveLength(0);

        const missingBranchRevisionResponse = await client.GET(
            "/api/projects/{projectId}/branches/{branchName}",
            {
                params: {
                    path: {
                        projectId: data.id,
                        branchName: "missing",
                    },
                },
            },
        );

        expect(missingBranchRevisionResponse.data).toBeUndefined();
        expect(missingBranchRevisionResponse.response.status).toBe(404);
    });

    it("lists models with last modified timestamp and role", async () => {
        const client = createApiClient(getIntegrationBaseUrl());
        const requestBody = {
            name: "List Models Integration Test",
            description: "Create a model for list endpoint",
            modelSettings: defaultModelSettings,
        };

        const createResponse = await client.POST("/api/projects", {
            body: requestBody,
        });

        expect(createResponse.error).toBeUndefined();
        expect(createResponse.response.status).toBe(200);
        expect(createResponse.data).toBeDefined();

        if (!createResponse.data) {
            throw new Error("Expected response body from create model API");
        }

        const revisionResponse = await client.GET(
            "/api/projects/{projectId}/branches/{branchName}",
            {
                params: {
                    path: {
                        projectId: createResponse.data.id,
                        branchName: "main",
                    },
                },
            },
        );

        expect(revisionResponse.error).toBeUndefined();
        expect(revisionResponse.response.status).toBe(200);
        expect(revisionResponse.data).toBeDefined();

        if (!revisionResponse.data) {
            throw new Error("Expected response body from get model revision API");
        }

        const listResponse = await client.GET("/api/projects");

        expect(listResponse.error).toBeUndefined();
        expect(listResponse.response.status).toBe(200);
        expect(listResponse.data).toBeDefined();

        const listedProjects = listResponse.data?.find(
            (model) => model.id === createResponse.data?.id,
        );

        expect(listedProjects).toBeDefined();

        if (!listedProjects) {
            throw new Error("Expected created model to be returned by get models API");
        }

        expect(listedProjects.name).toBe(requestBody.name);
        expect(listedProjects.description).toBe(requestBody.description);
        expect(listedProjects.lastModified).toBe(revisionResponse.data.createdAt);
        expect(listedProjects.role).toBe("Owner");
    });
});
