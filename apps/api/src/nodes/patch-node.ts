import {
  defineEndpoint,
  patchNodeReqSchema,
} from "@beamos/contracts";
import { z } from "zod";
import { httpError } from "../common/http-utils";
import { toVersionRef } from "../common/version-utils";
import type { AppContext } from "../common/types";
import { nodeResponseSchema } from "./node-response-schema";

const uuidSchema = z.uuid();
const modelVersionRefSchema = z.object({
  modelId: uuidSchema,
  revisionId: uuidSchema.nullable(),
  draftId: uuidSchema.nullable(),
});

export const patchNodeResSchema = z.object({
  node: nodeResponseSchema,
  version: modelVersionRefSchema,
});

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
        modelId: req.params.modelId,
      },
      version: {
        modelId: req.params.modelId,
        ...toVersionRef(req.body.target),
      },
    };
  },
});
