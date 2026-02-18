import { defineEndpoint } from "../contracts/endpoint";
import {
  patchModelReqSchema,
  patchModelResSchema,
} from "../contracts/model-version-editing";
import { httpError } from "../common/http-utils";
import { toVersionRef } from "../common/version-utils";
import type { AppContext } from "../common/types";

export const patchModel = defineEndpoint({
  method: "PATCH",
  path: "/api/projects/:projectId",
  req: patchModelReqSchema,
  res: patchModelResSchema,
  async handler(req, ctx: AppContext) {
    const model = await ctx.services.modelRepository.getById({
      projectId: req.params.projectId,
    });
    if (!model) {
      throw httpError("Model or target revision not found", 404);
    }

    model.rename(req.body.name);
    const savedModel = await ctx.services.modelRepository.update({
      model,
    });

    return {
      model: {
        id: savedModel.id,
        name: savedModel.name,
        description: savedModel.description,
      },
      version: {
        projectId: req.params.projectId,
        ...toVersionRef({ revisionId: req.body.revisionId }),
      },
    };
  },
});
