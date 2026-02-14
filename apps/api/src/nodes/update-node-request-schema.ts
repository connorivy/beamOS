import { z } from "zod";

export const updateNodeRequestSchema = z
  .object({
    id: z.string().trim().min(1),
    nodeTypeDescriminator: z.enum(["external", "internal"]).optional(),
  })
  .refine((value) => value.nodeTypeDescriminator !== undefined, {
    message: "At least one updatable field is required",
  });
