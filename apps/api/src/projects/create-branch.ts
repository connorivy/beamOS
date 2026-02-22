import { defineEndpoint } from "../contracts/endpoint";
import type { AppContext } from "../common/types";
import { z } from "zod";
import { uuidV7Schema } from "src/common/uuid";
import { httpError } from "src/common/http-utils";
import { getDb } from "src/db/client";
import { modelBranchHeads } from "src/db/schema";

const createBranchReqSchema = z
  .object({
    params: z.object({
      projectId: uuidV7Schema,
    }),
    body: z
      .object({
        branchName: z.string().trim().min(1),
        baseBranchName: z.string().trim().min(1).default("main"),
      })
      .meta({ id: "CreateBranchRequest" }),
  })
  .meta({ id: "CreateBranchEndpointRequest" });

const createBranchResSchema = z
  .object({
    projectId: uuidV7Schema,
    branchName: z.string(),
    baseBranchName: z.string(),
    headRevisionId: uuidV7Schema,
  })
  .meta({ id: "CreateBranchResponse" });

export const createBranch = defineEndpoint({
  method: "POST",
  path: "/api/projects/:projectId/branches",
  req: createBranchReqSchema,
  res: createBranchResSchema,
  async handler(req, ctx: AppContext) {
    const baseRevision = await ctx.services.modelRevisionRepository.load(
      req.params.projectId,
      req.body.baseBranchName,
    );
    if (!baseRevision) {
      throw httpError(`Base branch '${req.body.baseBranchName}' not found`, 404);
    }

    const existingBranch = await ctx.services.modelRevisionRepository.load(
      req.params.projectId,
      req.body.branchName,
    );
    if (existingBranch) {
      throw httpError(`Branch '${req.body.branchName}' already exists`, 409);
    }

    const now = new Date();
    await getDb().insert(modelBranchHeads).values({
      projectId: req.params.projectId,
      branchName: req.body.branchName,
      headRevisionId: baseRevision.id,
      updatedAt: now,
    });

    return {
      projectId: req.params.projectId,
      branchName: req.body.branchName,
      baseBranchName: req.body.baseBranchName,
      headRevisionId: baseRevision.id,
    };
  },
});
