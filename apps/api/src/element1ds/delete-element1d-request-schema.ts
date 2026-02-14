import { z } from "zod";

export const deleteElement1dRequestSchema = z.string().trim().min(1);
