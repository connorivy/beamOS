import {
  defineEndpoint,
  patchNodeReqSchema,
  patchNodeResSchema,
} from "@beamos/contracts";
import { httpError } from "../common/http-utils";
import { toVersionRef } from "../common/version-utils";
import type { AppContext } from "../common/types";

export const patchNode = defineEndpoint({
  method: "PATCH",
  path: "/api/models/:modelId/nodes/:nodeId",
  req: patchNodeReqSchema,
  res: patchNodeResSchema,
  async handler(req, ctx: AppContext) {
    const model = await ctx.services.modelRepository.getById({
      modelId: req.params.modelId,
      revisionId: req.body.target.revisionId,
      draftId: req.body.target.draftId,
    });
    if (!model) {
      throw httpError("Model or target revision/draft not found", 404);
    }

    try {
      model.updateNode({
        nodeId: req.params.nodeId,
        name: req.body.name,
      });
    } catch (error) {
      if (error instanceof Error) {
        throw httpError(error.message, 400);
      }
      throw error;
    }

    await ctx.services.modelRepository.save(model);

    const patchedNode = model.nodes.find(
      (node) => node.id === req.params.nodeId,
    );
    if (!patchedNode) {
      throw httpError("Node not found after patch", 500);
    }

    return {
      node: {
        id: patchedNode.id,
        modelId: patchedNode.modelId,
        name: patchedNode.name,
      },
      version: {
        modelId: req.params.modelId,
        ...toVersionRef(req.body.target),
      },
    };
  },
});
