import { z } from "zod";
import { uuidV7Schema } from "../common/uuid";
import { materialPropertiesSchema } from "./material-properties-schema";

export const updateMaterialRequestSchema = materialPropertiesSchema.extend({
  id: uuidV7Schema,
});
