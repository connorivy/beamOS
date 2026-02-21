import { defineEndpoint } from "../contracts/endpoint";
import type { AppContext } from "../common/types";
import { z } from "zod";
import { uuidV7Schema } from "src/common/uuid";
import { projectResponseSchema } from "./create-project";
import { getDb } from "src/db/client";
import { modelBranchHeads, projects } from "src/db/schema";
import { httpError } from "src/common/http-utils";

const forkProjectReqSchema = z
    .object({
        params: z.object({ projectId: uuidV7Schema }),
        body: z
            .object({
                name: z.string().min(1).optional(),
            })
            .optional()
            .meta({ id: "ForkProjectRequest" }),
    })
    .meta({ id: "ForkProjectEndpointRequest" });

export const forkProject = defineEndpoint({
    method: "POST",
    path: "/api/projects/:projectId/fork",
    req: forkProjectReqSchema,
    res: projectResponseSchema,
    async handler(req, ctx: AppContext) {
        const sourceProject = await ctx.services.modelRepository.getById({
            projectId: req.params.projectId,
            loadModelBranchHeadAggregates: true,
        });

        if (!sourceProject) {
            throw httpError("Project not found", 404);
        }

        const sourceBranchHeads = sourceProject.modelBranchHeads ?? [];
        if (sourceBranchHeads.length === 0) {
            throw httpError("Source project has no branches", 400);
        }

        const newProjectId = Bun.randomUUIDv7();
        const newName = req.body?.name ?? `${sourceProject.name} (fork)`;
        const now = new Date();

        await getDb().transaction(async (tx) => {
            await tx.insert(projects).values({
                id: newProjectId,
                name: newName,
                description: sourceProject.description,
            });

            await tx.insert(modelBranchHeads).values(
                sourceBranchHeads.map((branch) => ({
                    projectId: newProjectId,
                    branchName: branch.branchName,
                    headRevisionId: branch.headRevisionId,
                    updatedAt: now,
                })),
            );
        });

        return {
            id: newProjectId,
            name: newName,
            description: sourceProject.description,
            lastModified: now.toISOString(),
            role: "Owner" as const,
        };
    },
});
