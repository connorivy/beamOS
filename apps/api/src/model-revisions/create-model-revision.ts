import { defineEndpoint } from "@beamos/contracts";
import { modelRevisionResponseSchema } from "./model-revision-response-schema";
import { createModelRevisionReqSchema } from "./create-model-revision-request-schema";
import { AppContext } from "src/common/types";
import { httpError } from "src/common/http-utils";

export const createModelRevision = defineEndpoint({
  method: "POST",
  path: "/api/models/:modelId/branches/:branchName/revision",
  req: createModelRevisionReqSchema,
  res: modelRevisionResponseSchema,
  async handler(req, ctx: AppContext) {
    const branch = await ctx.services.modelRevisionRepository.getBranchHead(
      req.params.modelId,
      req.params.branchName,
    );

    if (!branch) {
      throw httpError("Model branch not found", 404);
    }

    const modelRevision =
      await ctx.services.modelRevisionRepository.getRevisionById(
        branch.headRevisionId,
      );

    if (!modelRevision) {
      throw httpError("Model revision not found", 404);
    }

    // todo: implement createModelRevision to create a new revision based on the parent revision and the operations in the request body
    // the new revision should be created with the same modelId and branchName as the parent revision, and the parentRevisionId should be set to the id of the parent revision
    // the new revision should also include the operations from the request body (create, update, delete for nodes, materials, section profiles, and element1ds)
    // after creating the new revision, it should be returned in the response
  },
});
