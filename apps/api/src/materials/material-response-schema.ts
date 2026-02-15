import { PressureUnits } from "unitsnet-js";
import { z } from "zod";
import { uuidV7Schema } from "../common/uuid";

const pressureResponseSchema = z.object({
  value: z.number().finite(),
  unit: z.literal(PressureUnits.Pascals),
});

export const materialPropertiesResponseSchema = z.object({
  pressureE: pressureResponseSchema,
  pressureG: pressureResponseSchema,
});

export const materialResponseSchema = materialPropertiesResponseSchema.extend({
  id: uuidV7Schema,
  revisionId: uuidV7Schema,
});
