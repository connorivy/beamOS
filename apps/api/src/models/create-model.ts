import { defineEndpoint } from "../contracts/endpoint";
import { ModelAggregate } from "../models/model-aggregate";
import type { AppContext } from "../common/types";
import { z } from "zod";

const uuidSchema = z.uuid();
export const createModelReqSchema = z.object({
  body: z.object({
    name: z.string().min(1),
    authorId: uuidSchema,
    message: z.string().min(1),
  }),
});
export const createModelResSchema = z
  .object({
    model: z.object({
      id: uuidSchema,
      name: z.string(),
      description: z.string(),
    }),
    version: z.object({
      modelId: uuidSchema,
      branchName: z.string(),
      revisionId: uuidSchema,
      revisionsAhead: z.number().min(0).meta({
        description:
          "Number of revisions ahead of the parent branch. In progress revisions are not included in the number",
      }),
      revisionsBehind: z.number().min(0),
    }),
  })
  .meta({ id: "Model" });

export const createModel = defineEndpoint({
  method: "POST",
  path: "/api/models",
  req: createModelReqSchema,
  res: createModelResSchema,
  async handler(req, ctx: AppContext) {
    const model = ModelAggregate.create({
      name: req.body.name,
    });
    const createdModel = await ctx.services.modelRepository.create({
      model,
      authorId: req.body.authorId,
      message: req.body.message,
    });
    return {
      model: {
        id: createdModel.model.id,
        name: createdModel.model.name,
        description: createdModel.model.description,
      },
      version: {
        modelId: createdModel.model.id,
        branchName: "main",
        revisionId: createdModel.revisionId,
        revisionsAhead: 0,
        revisionsBehind: 0,
      },
    };
  },
});
