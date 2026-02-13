import {
  createNodeReqSchema,
  createNodeResSchema,
  defineEndpoint,
} from "@beamos/contracts";
import type { AppContext } from "../common/types";
import { httpError } from "../common/http-utils";
import { toVersionRef } from "../common/version-utils";

export const createNode = defineEndpoint({
  method: "POST",
  path: "/api/models/:modelId/nodes",
  req: createNodeReqSchema,
  res: createNodeResSchema,
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
