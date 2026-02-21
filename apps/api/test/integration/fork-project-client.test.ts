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

describe("fork project integration", () => {
    it("forks a project and the forked project has the same branches", async () => {
        const client = createApiClient(getIntegrationBaseUrl());

        // Create source project
        const createResponse = await client.POST("/api/projects", {
            body: {
                name: "Fork Source Project",
                description: "Project to be forked",
                modelSettings: defaultModelSettings,
            },
        });

        expect(createResponse.error).toBeUndefined();
        expect(createResponse.response.status).toBe(200);
        expect(createResponse.data).toBeDefined();

        if (!createResponse.data) {
            throw new Error("Expected response body from create project API");
        }

        const sourceProjectId = createResponse.data.id;

        // Verify source project has a main branch
        const sourceBranchResponse = await client.GET(
            "/api/projects/{projectId}/branches/{branchName}",
            { params: { path: { projectId: sourceProjectId, branchName: "main" } } },
        );
        expect(sourceBranchResponse.response.status).toBe(200);
        expect(sourceBranchResponse.data).toBeDefined();

        // Fork the project
        const forkResponse = await client.POST("/api/projects/{projectId}/fork", {
            params: { path: { projectId: sourceProjectId } },
            body: { name: "Fork Source Project (fork)" },
        });

        expect(forkResponse.error).toBeUndefined();
        expect(forkResponse.response.status).toBe(200);
        expect(forkResponse.data).toBeDefined();

        if (!forkResponse.data) {
            throw new Error("Expected response body from fork project API");
        }

        const forkedProjectId = forkResponse.data.id;

        // Forked project should have a different id
        expect(forkedProjectId).not.toBe(sourceProjectId);
        expect(forkResponse.data.name).toBe("Fork Source Project (fork)");
        expect(forkResponse.data.description).toBe("Project to be forked");

        // Forked project should have the same main branch pointing to the same revision
        const forkedBranchResponse = await client.GET(
            "/api/projects/{projectId}/branches/{branchName}",
            { params: { path: { projectId: forkedProjectId, branchName: "main" } } },
        );

        expect(forkedBranchResponse.error).toBeUndefined();
        expect(forkedBranchResponse.response.status).toBe(200);
        expect(forkedBranchResponse.data).toBeDefined();

        if (!forkedBranchResponse.data || !sourceBranchResponse.data) {
            throw new Error("Expected branch data for both projects");
        }

        // Both branches should point to the same head revision
        expect(forkedBranchResponse.data.id).toBe(sourceBranchResponse.data.id);
    });

    it("forks a project with commits and preserves the revision history", async () => {
        const client = createApiClient(getIntegrationBaseUrl());

        // Create source project
        const createResponse = await client.POST("/api/projects", {
            body: {
                name: "Fork History Source",
                description: "Project with commits to fork",
                modelSettings: defaultModelSettings,
            },
        });

        expect(createResponse.data).toBeDefined();
        if (!createResponse.data) throw new Error("Expected project");

        const sourceProjectId = createResponse.data.id;

        // Create a revision with a node
        const revisionResponse = await client.POST(
            "/api/projects/{projectId}/branches/{branchName}/revisions",
            {
                params: { path: { projectId: sourceProjectId, branchName: "main" } },
                body: {
                    nodes: {
                        create: [{ location: { type: "spatial", point: { x: 1, y: 2, z: 3 } } }],
                    },
                },
            },
        );

        expect(revisionResponse.response.status).toBe(200);
        expect(revisionResponse.data?.nodes).toHaveLength(1);

        // Fork the project
        const forkResponse = await client.POST("/api/projects/{projectId}/fork", {
            params: { path: { projectId: sourceProjectId } },
        });

        expect(forkResponse.response.status).toBe(200);
        expect(forkResponse.data).toBeDefined();

        if (!forkResponse.data) throw new Error("Expected fork data");

        const forkedProjectId = forkResponse.data.id;

        // Forked project uses the default name when none is provided
        expect(forkResponse.data.name).toBe("Fork History Source (fork)");

        // Forked project's branch should contain the node from the commit
        const forkedBranchResponse = await client.GET(
            "/api/projects/{projectId}/branches/{branchName}",
            { params: { path: { projectId: forkedProjectId, branchName: "main" } } },
        );

        expect(forkedBranchResponse.response.status).toBe(200);
        expect(forkedBranchResponse.data?.nodes).toHaveLength(1);
        expect(forkedBranchResponse.data?.nodes[0]?.location).toEqual(
            revisionResponse.data?.nodes[0]?.location,
        );
    });

    it("returns 404 when forking a non-existent project", async () => {
        const client = createApiClient(getIntegrationBaseUrl());

        const nonExistentId = Bun.randomUUIDv7();
        const forkResponse = await client.POST("/api/projects/{projectId}/fork", {
            params: { path: { projectId: nonExistentId } },
        });

        expect(forkResponse.response.status).toBe(404);
    });
});
