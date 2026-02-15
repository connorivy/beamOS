import { PressureUnits } from "unitsnet-js";
import { z } from "zod";

export const pressureDtoSchema = z.object({
  value: z.number().finite(),
  unit: z.enum(PressureUnits),
});

export const materialPropertiesSchema = z.object({
  pressureE: pressureDtoSchema,
  pressureG: pressureDtoSchema,
});
