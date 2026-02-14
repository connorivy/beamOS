import { z } from "zod";

export const deleteSectionProfileRequestSchema = z.string().trim().min(1);
