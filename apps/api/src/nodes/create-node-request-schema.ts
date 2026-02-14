import { z } from "zod";

export const createNodeRequestSchema = z.object({
  id: z.string().trim().min(1),
  nodeTypeDescriminator: z.enum(["external", "internal"]).optional(),
});
