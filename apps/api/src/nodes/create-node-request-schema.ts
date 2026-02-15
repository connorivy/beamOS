import { z } from "zod";
import { uuidV7Schema } from "../common/uuid";

const restraintSchema = z.object({
  canTranslateAlongX: z.boolean(),
  canTranslateAlongY: z.boolean(),
  canTranslateAlongZ: z.boolean(),
  canRotateAboutX: z.boolean(),
  canRotateAboutY: z.boolean(),
  canRotateAboutZ: z.boolean(),
}).optional();

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
  ratioAlongElement1d: z.number().finite().min(0).max(1),
});

export const createNodeRequestSchema = z.object({
  tempId: z.string().trim().min(1).optional(),
  restraint: restraintSchema,
  location: z.discriminatedUnion("type", [
    spatialNodeLocationSchema,
    internalNodeLocationSchema,
  ]),
});
