import { z } from "zod";

export const deleteMaterialRequestSchema = z.string().trim().min(1);
