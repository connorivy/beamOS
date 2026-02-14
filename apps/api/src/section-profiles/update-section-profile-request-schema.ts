import { z } from "zod";

export const updateSectionProfileRequestSchema = z
  .object({
    id: z.string().trim().min(1),
    name: z.string().trim().min(1).optional(),
  })
  .refine((value) => value.name !== undefined, {
    message: "At least one updatable field is required",
  });
