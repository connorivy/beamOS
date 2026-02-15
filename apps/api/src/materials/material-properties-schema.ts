import { PressureUnits } from "unitsnet-js";
import { z } from "zod";

export const materialPropertiesSchema = z.object({
  modulusOfElasticity: z.number().finite(),
  modulusOfRigidity: z.number().finite(),
  units: z.object({
    pressure: z.enum(PressureUnits),
  }),
});
