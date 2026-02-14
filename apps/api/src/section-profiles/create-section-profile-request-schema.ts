import { z } from "zod";

export const createSectionProfileRequestSchema = z.object({
  id: z.string().trim().min(1),
  name: z.string().trim().min(1),
});
