import { defineEndpoint } from "../contracts/endpoint";
import { ModelAggregate } from "../models/model-aggregate";
import type { AppContext } from "../common/types";
import { z } from "zod";
import { modelMapper } from "./model-mapper";
import { uuidV7Schema } from "src/common/uuid";

const createModelReqSchema = z.object({
  body: z
    .object({
      name: z.string().min(1),
      description: z.string().min(1),
    })
    .meta({ id: "CreateModelRequest" }),
});

export const modelResponseSchema = z
  .object({
    id: uuidV7Schema,
    name: z.string(),
    description: z.string(),
    lastModified: z.string().datetime(),
    role: z.enum(["Owner", "Contributor", "Reviewer"]),
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
      message: req.body.description,
    });
    return modelMapper.toResponse(createdModel);
  },
});
