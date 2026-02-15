import { PressureUnits } from "unitsnet-js";
import { z } from "zod";

export const materialPropertiesSchema = z.object({
  E: z.number().finite(),
  G: z.number().finite(),
  units: z.object({
    pressure: z.enum(PressureUnits),
  }),
});
