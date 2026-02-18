import { defineEndpoint } from "../contracts/endpoint";
import { ModelAggregate } from "../models/model-aggregate";
import type { AppContext } from "../common/types";
import { z } from "zod";

const uuidSchema = z.uuid();
const createModelRequestBodySchema = z
  .object({
    name: z.string().min(1),
    authorId: uuidSchema,
    description: z.string().min(1),
  })
  .meta({ id: "CreateModelRequest" });

export const createModelReqSchema = z
  .object({
    body: createModelRequestBodySchema,
  })
  .meta({ id: "CreateModelEndpointRequest" });

export const modelResponseSchema = z
  .object({
    id: uuidSchema,
    name: z.string(),
    description: z.string(),
    lastModified: z.date(),
    role: z.string(),
  })
  .meta({ id: "Model" });

export const createModel = defineEndpoint({
  method: "POST",
  path: "/api/models",
  req: createModelReqSchema,
  res: modelResponseSchema,
  async handler(req, ctx: AppContext) {
    const model = ModelAggregate.create({
      name: req.body.name,
      description: req.body.description,
    });
    const createdModel = await ctx.services.modelRepository.create({
      model,
      authorId: req.body.authorId,
      message: req.body.description,
    });
    return {
      id: createdModel.id,
      name: createdModel.name,
      description: createdModel.description,
      lastModified: createdModel.modelBranchHeads
        ? createdModel.modelBranchHeads[0].updatedAt
        : new Date(),
      role: "Owner" as const,
    };
  },
});
