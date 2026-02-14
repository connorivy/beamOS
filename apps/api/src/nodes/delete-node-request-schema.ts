import { z } from "zod";

export const deleteNodeRequestSchema = z.string().trim().min(1);
