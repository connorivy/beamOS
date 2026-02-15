import {
  AreaMomentOfInertiaUnits,
  AreaUnits,
  VolumeUnits,
  WarpingMomentOfInertiaUnits,
} from "unitsnet-js";
import { z } from "zod";
import { uuidV7Schema } from "../common/uuid";
import { SECTION_PROFILE_DISCRIMINATORS } from "./section-profile-entity";

export const sectionProfileResponseSchema = z.object({
  id: uuidV7Schema,
  revisionId: uuidV7Schema,
  name: z.string().min(1),
  discriminator: z.enum(SECTION_PROFILE_DISCRIMINATORS),
  area: z.object({
    value: z.number().finite(),
    unit: z.literal(AreaUnits.SquareMeters),
  }),
  strongAxisMomentOfInertia: z.object({
    value: z.number().finite(),
    unit: z.literal(AreaMomentOfInertiaUnits.MetersToTheFourth),
  }),
  weakAxisMomentOfInertia: z.object({
    value: z.number().finite(),
    unit: z.literal(AreaMomentOfInertiaUnits.MetersToTheFourth),
  }),
  torsionalConstant: z.object({
    value: z.number().finite(),
    unit: z.literal(AreaMomentOfInertiaUnits.MetersToTheFourth),
  }),
  warpingConstant: z.object({
    value: z.number().finite(),
    unit: z.literal(WarpingMomentOfInertiaUnits.MetersToTheSixth),
  }),
  strongAxisPlasticSectionModulus: z.object({
    value: z.number().finite(),
    unit: z.literal(VolumeUnits.CubicMeters),
  }),
  weakAxisPlasticSectionModulus: z.object({
    value: z.number().finite(),
    unit: z.literal(VolumeUnits.CubicMeters),
  }),
  strongAxisElasticSectionModulus: z.object({
    value: z.number().finite(),
    unit: z.literal(VolumeUnits.CubicMeters),
  }),
  weakAxisElasticSectionModulus: z.object({
    value: z.number().finite(),
    unit: z.literal(VolumeUnits.CubicMeters),
  }),
  strongAxisShearArea: z
    .object({
      value: z.number().finite(),
      unit: z.literal(AreaUnits.SquareMeters),
    })
    .optional(),
  weakAxisShearArea: z
    .object({
      value: z.number().finite(),
      unit: z.literal(AreaUnits.SquareMeters),
    })
    .optional(),
});
