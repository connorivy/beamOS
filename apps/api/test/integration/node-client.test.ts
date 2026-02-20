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

describe("typed node api client integration", () => {
    it("batch creates nodes and verifies persistence in revision", async () => {
        const client = createApiClient(getIntegrationBaseUrl());
        const tempIds = ["node-01", "node-02", "node-03"];

        const createModelResponse = await client.POST("/api/projects", {
            body: {
                name: "Node Integration Model",
                description: "Create model for nodes test",
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
                    nodes: {
                        create: [
                            {
                                tempId: tempIds[0],
                                restraint: {
                                    canTranslateAlongX: false,
                                    canTranslateAlongY: true,
                                    canTranslateAlongZ: true,
                                    canRotateAboutX: true,
                                    canRotateAboutY: true,
                                    canRotateAboutZ: true,
                                },
                                location: {
                                    type: "spatial",
                                    point: { x: 0, y: 1, z: 2 },
                                },
                            },
                            {
                                tempId: tempIds[1],
                                restraint: {
                                    canTranslateAlongX: true,
                                    canTranslateAlongY: false,
                                    canTranslateAlongZ: true,
                                    canRotateAboutX: true,
                                    canRotateAboutY: true,
                                    canRotateAboutZ: true,
                                },
                                location: {
                                    type: "spatial",
                                    point: { x: 3, y: 4, z: 5 },
                                },
                            },
                            {
                                tempId: tempIds[2],
                                restraint: {
                                    canTranslateAlongX: false,
                                    canTranslateAlongY: false,
                                    canTranslateAlongZ: false,
                                    canRotateAboutX: false,
                                    canRotateAboutY: false,
                                    canRotateAboutZ: false,
                                },
                                location: {
                                    type: "spatial",
                                    point: { x: 10, y: 20, z: 30 },
                                },
                            },
                        ],
                    },
                },
            },
        );

        expect(batchCreateResponse.error).toBeUndefined();
        expect(batchCreateResponse.response.status).toBe(200);
        expect(batchCreateResponse.data).toBeDefined();
        expect(batchCreateResponse.data?.nodes).toHaveLength(tempIds.length);

        if (!batchCreateResponse.data) {
            throw new Error("Expected batch create response");
        }

        const tempIdToId = {
            [tempIds[0]]: batchCreateResponse.data.nodes[0]?.id,
            [tempIds[1]]: batchCreateResponse.data.nodes[1]?.id,
            [tempIds[2]]: batchCreateResponse.data.nodes[2]?.id,
        };
        const createdNodeIds = Object.values(tempIdToId).filter(
            (nodeId): nodeId is string => typeof nodeId === "string",
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
        expect(getRevisionResponse.data?.nodes.map((node) => node.id)).toEqual(
            expect.arrayContaining(createdNodeIds),
        );

        // Verify all created nodes are in the revision
        for (const tempId of tempIds) {
            const nodeId = tempIdToId[tempId];
            expect(nodeId).toBeDefined();

            if (!nodeId) {
                throw new Error(`Expected node ID for tempId ${tempId}`);
            }

            const nodeInRevision = getRevisionResponse.data?.nodes.find((n) => n.id === nodeId);
            expect(nodeInRevision).toBeDefined();
            expect(nodeInRevision?.id).toBe(nodeId);
            expect(nodeInRevision?.projectId).toBe(projectId);
        }

        // Snapshot the nodes data structure
        const nodesSnapshot = getRevisionResponse.data?.nodes
            .slice()
            .sort((a, b) => a.id.localeCompare(b.id))
            .map((node) => ({
                ...node,
                id: "<db-id>",
                projectId: "<project-id>",
            }));
        expect(nodesSnapshot).toMatchSnapshot();
    });

    it("rejects duplicate temp ids in batch create", async () => {
        const client = createApiClient(getIntegrationBaseUrl());

        const createModelResponse = await client.POST("/api/projects", {
            body: {
                name: "Duplicate Node TempId Model",
                description: "Create model for duplicate node tempId test",
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
                    nodes: {
                        create: [
                            {
                                tempId: "dup-1",
                                location: {
                                    type: "spatial",
                                    point: { x: 0, y: 0, z: 0 },
                                },
                            },
                            {
                                tempId: "dup-1",
                                location: {
                                    type: "spatial",
                                    point: { x: 1, y: 1, z: 1 },
                                },
                            },
                        ],
                    },
                },
            },
        );

        expect(batchCreateResponse.data).toBeUndefined();
        expect(batchCreateResponse.response.status).toBe(409);
    });
});
