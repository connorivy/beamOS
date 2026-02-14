import { z } from "zod";

const uuidSchema = z.uuid();

export const nodeResponseSchema = z.object({
  id: uuidSchema,
  modelId: uuidSchema,
  name: z.string(),
});

export const revisionNodeResponseSchema = z.object({
  id: uuidSchema,
  modelId: uuidSchema,
  nodeTypeDescriminator: z.enum(["external", "internal"]),
});
