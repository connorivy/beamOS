import { PressureUnits } from "unitsnet-js";
import { z } from "zod";

const pressureDtoSchema = z.object({
  value: z.number().finite(),
  unit: z.enum(PressureUnits),
});

export const createMaterialRequestSchema = z.object({
  name: z.string().trim().min(1),
  pressureE: pressureDtoSchema,
  pressureG: pressureDtoSchema,
  tempId: z.string().trim().min(1).optional(),
});
