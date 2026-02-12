import {
  defineEndpoint,
  patchNodeReqSchema,
  patchNodeResSchema,
} from "@beamos/contracts";
import {
  httpError,
  loadModelForTarget,
  toVersionRef,
} from "src/endpoints/model-version-target";
import type { AppContext } from "../services/types";

export const patchNode = defineEndpoint({
  method: "PATCH",
  path: "/api/models/:modelId/nodes/:nodeId",
  req: patchNodeReqSchema,
  res: patchNodeResSchema,
  async handler(req, ctx: AppContext) {
    const model = await loadModelForTarget({
      modelId: req.params.modelId,
      target: req.body.target,
      ctx,
    });

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
