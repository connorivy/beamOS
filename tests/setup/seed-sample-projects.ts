import { createApiClient } from "@beamos/openapi-client";
import { sampleProjectFixtures } from "../fixtures/Kassimali_MatrixAnalysisOfStructures2ndEd";

export const seedSampleProjects = async (apiBaseUrl: string) => {
    const client = createApiClient(apiBaseUrl);

    for (const sampleProjectSeed of sampleProjectFixtures) {
        const exists = await hasMainBranchRevision({
            client,
            projectId: sampleProjectSeed.project.id,
        });
        if (exists) {
            continue;
        }

        const { error } = await client.POST("/api/projects", {
            body: sampleProjectSeed.project,
        });

        if (error) {
            throw new Error(
                `Failed to seed sample project ${sampleProjectSeed.project.id}: ${JSON.stringify(error)}`,
            );
        }

        if (sampleProjectSeed.model) {
            const { error: modelError } = await client.POST(
                "/api/projects/{projectId}/branches/{branchName}/revisions",
                {
                    params: {
                        path: { projectId: sampleProjectSeed.project.id, branchName: "main" },
                    },
                    body: sampleProjectSeed.model,
                },
            );

            if (modelError) {
                throw new Error(
                    `Failed to seed sample project model revision ${sampleProjectSeed.project.id}: ${JSON.stringify(modelError)}`,
                );
            }
        }
    }
};

const hasMainBranchRevision = async (input: {
    client: ReturnType<typeof createApiClient>;
    projectId: string;
}): Promise<boolean> => {
    const response = await input.client.GET("/api/projects/{projectId}/branches/{branchName}", {
        params: {
            path: {
                projectId: input.projectId,
                branchName: "main",
            },
        },
    });

    if (response.data) {
        return true;
    }

    if (response.response.status === 404) {
        return false;
    }

    throw new Error(
        `Failed to check project ${input.projectId} existence: status=${response.response.status}, error=${JSON.stringify(response.error)}`,
    );
};
