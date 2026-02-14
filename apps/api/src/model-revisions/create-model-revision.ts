import { defineEndpoint } from "../contracts/endpoint";
import { modelRevisionResponseSchema } from "./model-revision-response-schema";
import { createModelRevisionReqSchema } from "./create-model-revision-request-schema";
import { AppContext } from "src/common/types";
import { httpError } from "src/common/http-utils";
import { DbTransaction, getDb } from "src/db/client";
import { modelBranchHeads, modelRevisions } from "src/db/schema";

export const createModelRevision = defineEndpoint({
  method: "POST",
  path: "/api/models/:modelId/branches/:branchName/revisions",
  req: createModelRevisionReqSchema,
  res: modelRevisionResponseSchema,
  async handler(req, ctx: AppContext) {
    return await getDb().transaction(async (tx) => {
      const revisionId = await createNewRevisionHandler(req, ctx, tx);
      // todo: implement createModelRevision to create a new revision based on the parent revision and the operations in the request body
      // the new revision should be created with the same modelId and branchName as the parent revision, and the parentRevisionId should be set to the id of the parent revision
      // the new revision should also include the operations from the request body (create, update, delete for nodes, materials, section profiles, and element1ds)
      // after creating the new revision, it should be returned in the response
      return null;
    });
  },
});

export async function createNewRevisionHandler(
  req: {
    params: { modelId: string; branchName: string };
  },
  ctx: AppContext,
  tx: DbTransaction,
) {
  const { modelId, branchName } = req.params;
  const branch = await ctx.services.modelRevisionRepository.getBranchHead(
    modelId,
    branchName,
  );
  if (!branch) {
    throw httpError(
      `Could not find branch ${branchName} on model with ID ${modelId}`,
      404,
    );
  }

  const parentRevision =
    await ctx.services.modelRevisionRepository.getRevisionById(
      branch.headRevisionId,
    );
  if (!parentRevision) {
    throw httpError(
      `Could not find parent revision ${branch.headRevisionId} for branch ${branchName}`,
      404,
    );
  }

  const revisionId = Bun.randomUUIDv7();
  const now = new Date();
  await tx.insert(modelRevisions).values({
    id: revisionId,
    modelId,
    modelName: parentRevision.name,
    parentRevisionId: parentRevision.id,
    secondParentRevisionId: null,
    authorId: parentRevision.authorId,
    message: "Batch create materials",
    createdAt: now,
  });

  await tx
    .insert(modelBranchHeads)
    .values({
      modelId,
      branchName,
      headRevisionId: revisionId,
      updatedAt: now,
    })
    .onConflictDoUpdate({
      target: [modelBranchHeads.modelId, modelBranchHeads.branchName],
      set: {
        headRevisionId: revisionId,
        updatedAt: now,
      },
    });
  return revisionId;
}
