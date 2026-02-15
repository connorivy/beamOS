import { PressureUnits } from "unitsnet-js";
import { z } from "zod";
import { uuidV7Schema } from "../common/uuid";

export const materialPropertiesResponseSchema = z.object({
  E: z.number().finite(),
  G: z.number().finite(),
  units: z.object({
    pressure: z.literal(PressureUnits.Pascals),
  }),
});

export const materialResponseSchema = materialPropertiesResponseSchema.extend({
  id: uuidV7Schema,
  revisionId: uuidV7Schema,
});
