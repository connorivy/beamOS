import { z } from "zod";
import { materialPropertiesSchema } from "./material-properties-schema";

export const createMaterialRequestSchema = materialPropertiesSchema.extend({
  tempId: z.string().trim().min(1).optional(),
});
