import { PressureUnits } from "unitsnet-js";
import { z } from "zod";
import { uuidV7Schema } from "../common/uuid";

const pressureDtoSchema = z.object({
  value: z.number().finite(),
  unit: z.enum(PressureUnits),
});

export const putMaterialReqSchema = z.object({
  id: uuidV7Schema,
  pressureE: pressureDtoSchema,
  pressureG: pressureDtoSchema,
});
