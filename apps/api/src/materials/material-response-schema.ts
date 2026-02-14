import { PressureUnits } from "unitsnet-js";
import { z } from "zod";
import { uuidV7Schema } from "../common/uuid";

export const materialResponseSchema = z.object({
  id: uuidV7Schema,
  revisionId: uuidV7Schema,
  pressureE: z.object({
    value: z.number().finite(),
    unit: z.literal(PressureUnits.Pascals),
  }),
  pressureG: z.object({
    value: z.number().finite(),
    unit: z.literal(PressureUnits.Pascals),
  }),
});
