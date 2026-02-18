import { defineEndpoint } from "../contracts/endpoint";
import type { AppContext } from "../common/types";
import { z } from "zod";
import { modelResponseSchema } from "./create-model";
import { modelMapper } from "./model-mapper";

const getModelsReqSchema = z.object({});

const getModelsResSchema = z
  .array(modelResponseSchema)
  .meta({ id: "ModelsArray" });

export const getModels = defineEndpoint({
  method: "GET",
  path: "/api/models",
  req: getModelsReqSchema,
  res: getModelsResSchema,
  async handler(_req, ctx: AppContext) {
    const models = await ctx.services.modelRepository.getUserModels();
    return models.map((model) => modelMapper.toResponse(model));
  },
});
