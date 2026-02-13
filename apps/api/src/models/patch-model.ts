import {
  defineEndpoint,
  patchModelReqSchema,
  patchModelResSchema,
} from "@beamos/contracts";
import { httpError } from "../common/http-utils";
import { toVersionRef } from "../common/version-utils";
import type { AppContext } from "../common/types";

export const patchModel = defineEndpoint({
  method: "PATCH",
  path: "/api/models/:modelId",
  req: patchModelReqSchema,
  res: patchModelResSchema,
  async handler(req, ctx: AppContext) {
    const model = await ctx.services.modelRepository.getById({
      modelId: req.params.modelId,
      revisionId: req.body.target.revisionId,
      draftId: req.body.target.draftId,
    });
    if (!model) {
      throw httpError("Model or target revision/draft not found", 404);
    }

    model.rename(req.body.name);
    const savedModel = await ctx.services.modelRepository.save(model);

    return {
      model: {
        id: savedModel.id,
        name: savedModel.name,
      },
      version: {
        modelId: req.params.modelId,
        ...toVersionRef(req.body.target),
      },
    };
  },
});
