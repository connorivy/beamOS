import { defineEndpoint } from "../contracts/endpoint";
import type { AppContext } from "../common/types";
import { z } from "zod";

const getModelsReqSchema = z.object({});

const getModelsResSchema = z.object({
  models: z.array(
    z.object({
      id: z.uuid(),
      name: z.string(),
      description: z.string(),
      lastModified: z.string().datetime().nullable(),
      role: z.enum(["Owner", "Contributor", "Reviewer"]),
    }),
  ),
});

export const getModels = defineEndpoint({
  method: "GET",
  path: "/api/models",
  req: getModelsReqSchema,
  res: getModelsResSchema,
  async handler(_req, ctx: AppContext) {
    const models = await ctx.services.modelRepository.getUserModels();
    return {
      models: models.map((model) => ({
        id: model.id,
        name: model.name,
        description: model.description,
        lastModified: model.lastModified?.toISOString() ?? null,
        role: model.role,
      })),
    };
  },
});
