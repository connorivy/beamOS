import { defineEndpoint } from "@beamos/contracts";
import { ModelAggregate } from "../models/model-aggregate";
import type { AppContext } from "../services/types";
import { z } from "zod";

const uuidSchema = z.uuid();
export const createModelReqSchema = z.object({
  body: z.object({
    name: z.string().min(1),
    authorId: uuidSchema,
    message: z.string().min(1),
  }),
});
export const createModelResSchema = z.object({
  model: z.object({
    id: uuidSchema,
    name: z.string(),
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
    inProgressRevisionId: uuidSchema,
  }),
});

export const createModel = defineEndpoint({
  method: "POST",
  path: "/api/models",
  req: createModelReqSchema,
  res: createModelResSchema,
  async handler(req, ctx: AppContext) {
    const model = ModelAggregate.create({
      name: req.body.name,
    });
    const savedModel = await ctx.services.modelRepository.save(model);

    return {
      model: {
        id: savedModel.id,
        name: savedModel.name,
      },
      version: {
        modelId: savedModel.id,
        branchName: "hello",
        revisionId: "",
        revisionsAhead: 0,
        revisionsBehind: 0,
        inProgressRevisionId: Bun.randomUUIDv7(),
      },
    };
  },
});
