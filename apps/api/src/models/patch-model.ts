import {
  defineEndpoint,
  patchModelReqSchema,
  patchModelResSchema,
} from "@beamos/contracts";
import {
  loadModelForTarget,
  toVersionRef,
} from "../endpoints/model-version-target";
import type { AppContext } from "../services/types";

export const patchModel = defineEndpoint({
  method: "PATCH",
  path: "/api/models/:modelId",
  req: patchModelReqSchema,
  res: patchModelResSchema,
  async handler(req, ctx: AppContext) {
    const model = await loadModelForTarget({
      modelId: req.params.modelId,
      target: req.body.target,
      ctx,
    });

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
