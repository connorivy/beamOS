import {
  AreaMomentOfInertiaUnits,
  AreaUnits,
  PressureUnits,
  VolumeUnits,
  WarpingMomentOfInertiaUnits,
} from "unitsnet-js";
import { z } from "zod";
import { uuidV7Schema } from "../common/uuid";

export const modelSettingsUnitsSchema = z.object({
  pressure: z.enum(PressureUnits),
  area: z.enum(AreaUnits),
  areaMomentOfInertia: z.enum(AreaMomentOfInertiaUnits),
  warpingMomentOfInertia: z.enum(WarpingMomentOfInertiaUnits),
  volume: z.enum(VolumeUnits),
});

export const modelSettingsPropertiesSchema = z.object({
  units: modelSettingsUnitsSchema,
  yAxisUp: z.boolean(),
});

export const modelSettingsResponseSchema = modelSettingsPropertiesSchema.extend({
  id: uuidV7Schema,
  revisionId: uuidV7Schema,
});
