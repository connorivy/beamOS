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
export const createModelResSchema = z.object({
  model: z.object({
    id: uuidSchema,
    name: z.string(),
    branchNames: z.array(z.string().trim().min(1)),
    lastModified: z.iso.datetime(),
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
    const lastModified = new Date().toISOString();
    const model = ModelAggregate.create({
      name: req.body.name,
    });
    const savedModel = await ctx.services.modelRepository.save(model);
    const initialRevision =
      await ctx.services.modelRevisionRepository.commitRevision({
        id: Bun.randomUUIDv7(),
        modelId: savedModel.id,
        branchName: "main",
        name: savedModel.name,
        authorId: req.body.authorId,
        message: req.body.message,
        nodes: [],
        includeModelChange: true,
      });

    return {
      model: {
        id: savedModel.id,
        name: savedModel.name,
        branchNames: [...savedModel.branchNames],
        lastModified,
      },
      version: {
        modelId: savedModel.id,
        branchName: "main",
        revisionId: initialRevision.id,
        revisionsAhead: 0,
        revisionsBehind: 0,
        inProgressRevisionId: initialRevision.id,
      },
    };
  },
});
