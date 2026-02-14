import { z } from "zod";
import { uuidV7Schema } from "../common/uuid";
import { Ratio } from "unitsnet-js";

const restraintSchema = z.record(z.string(), z.boolean());

const spatialNodeLocationSchema = z.object({
  type: z.literal("spatial"),
  point: z.object({
    x: z.number().finite(),
    y: z.number().finite(),
    z: z.number().finite(),
  }),
});

const internalNodeLocationSchema = z.object({
  type: z.literal("internal"),
  element1dId: uuidV7Schema,
  ratioAlongElement1d: z.instanceof(Ratio),
});

export const createNodeRequestSchema = z.object({
  tempId: z.string().trim().min(1).optional(),
  restraint: restraintSchema.default({}),
  location: z.discriminatedUnion("type", [
    spatialNodeLocationSchema,
    internalNodeLocationSchema,
  ]),
});
