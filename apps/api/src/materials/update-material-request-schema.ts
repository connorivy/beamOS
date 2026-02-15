import { z } from "zod";
import { materialPropertiesSchema } from "./material-properties-schema";

export const updateMaterialRequestSchema = materialPropertiesSchema.extend({
  id: z.string().trim().min(1),
});
