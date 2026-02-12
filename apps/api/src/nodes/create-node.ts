import {
  createNodeReqSchema,
  createNodeResSchema,
  defineEndpoint,
} from "@beamos/contracts";
import type { AppContext } from "../services/types";
import {
  httpError,
  loadModelForTarget,
  toVersionRef,
} from "src/endpoints/model-version-target";

export const createNode = defineEndpoint({
  method: "POST",
  path: "/api/models/:modelId/nodes",
  req: createNodeReqSchema,
  res: createNodeResSchema,
  async handler(req, ctx: AppContext) {
    const model = await loadModelForTarget({
      modelId: req.params.modelId,
      target: req.body.target,
      ctx,
    });

    try {
      model.addNode({
        id: req.body.nodeId,
        modelId: req.params.modelId,
        name: req.body.name,
      });
    } catch (error) {
      if (error instanceof Error) {
        throw httpError(error.message, 400);
      }
      throw error;
    }

    await ctx.services.modelRepository.save(model);

    return {
      node: {
        id: req.body.nodeId,
        modelId: req.params.modelId,
        name: req.body.name.trim(),
      },
      version: {
        modelId: req.params.modelId,
        ...toVersionRef(req.body.target),
      },
    };
  },
});
