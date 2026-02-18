import { defineEndpoint } from "../contracts/endpoint";
import { z } from "zod";
import { httpError } from "../common/http-utils";
import { toVersionRef } from "../common/version-utils";
import type { AppContext } from "../common/types";
import { nodeResponseSchema } from "./node-contract-schemas";
import { patchNodeReqSchema } from "src/contracts/model-version-editing";

const uuidSchema = z.uuid();
const modelVersionRefSchema = z.object({
  projectId: uuidSchema,
  revisionId: uuidSchema.nullable(),
});

export const patchNodeResSchema = z
  .object({
    node: nodeResponseSchema,
    version: modelVersionRefSchema,
  })
  .meta({ id: "PatchNodeResponse" });

export const patchNode = defineEndpoint({
  method: "PATCH",
  path: "/api/projects/:projectId/nodes/:nodeId",
  req: patchNodeReqSchema,
  res: patchNodeResSchema,
  async handler(req, ctx: AppContext) {
    const model = await ctx.services.modelRepository.getById({
      projectId: req.params.projectId,
    });
    if (!model) {
      throw httpError("Model or target revision not found", 404);
    }
    const revision = await ctx.services.modelRevisionRepository.getRevisionById(
      req.body.revisionId,
    );
    if (!revision) {
      throw httpError("Revision not found", 400);
    }
    if (revision.projectId !== req.params.projectId) {
      throw httpError("Revision does not belong to this model", 400);
    }
    if (!revision.nodes.some((node) => node.id === req.params.nodeId)) {
      throw httpError(
        `Node ${req.params.nodeId} does not exist in revision ${req.body.revisionId}`,
        400,
      );
    }

    await ctx.services.modelRepository.update({
      model,
    });

    return {
      node: {
        id: req.params.nodeId,
        projectId: req.params.projectId,
      },
      version: {
        projectId: req.params.projectId,
        ...toVersionRef({ revisionId: req.body.revisionId }),
      },
    };
  },
});
