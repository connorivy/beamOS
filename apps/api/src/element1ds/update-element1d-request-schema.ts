import { z } from "zod";

export const updateElement1dRequestSchema = z
  .object({
    id: z.string().trim().min(1),
    startNode: z.string().trim().min(1).optional(),
    endNode: z.string().trim().min(1).optional(),
    material: z.string().trim().min(1).optional(),
    section: z.string().trim().min(1).optional(),
  })
  .refine(
    (value) =>
      value.startNode !== undefined ||
      value.endNode !== undefined ||
      value.material !== undefined ||
      value.section !== undefined,
    {
      message:
        "At least one of startNode, endNode, material, or section is required",
    },
  );
