import {
  AreaMomentOfInertiaUnits,
  AreaUnits,
  VolumeUnits,
  WarpingMomentOfInertiaUnits,
} from "unitsnet-js";
import { z } from "zod";
import { uuidV7Schema } from "../common/uuid";
import { SECTION_PROFILE_DISCRIMINATORS } from "./section-profile-entity";

export const sectionPropertiesInputSchema = z.object({
  area: z.number().finite(),
  strongAxisMomentOfInertia: z.number().finite(),
  weakAxisMomentOfInertia: z.number().finite(),
  torsionalConstant: z.number().finite(),
  warpingConstant: z.number().finite(),
  strongAxisPlasticSectionModulus: z.number().finite(),
  weakAxisPlasticSectionModulus: z.number().finite(),
  strongAxisElasticSectionModulus: z.number().finite(),
  weakAxisElasticSectionModulus: z.number().finite(),
}).meta({ id: "SectionPropertiesInput" });

const sectionProfileBasePropertiesSchema = z.object({
  name: z.string().trim().min(1),
  ...sectionPropertiesInputSchema.shape,
});

const standardSectionProfilePropertiesSchema = sectionProfileBasePropertiesSchema.extend({
  discriminator: z.literal("STANDARD"),
});

const withShearAreasSectionProfilePropertiesSchema =
  sectionProfileBasePropertiesSchema.extend({
    discriminator: z.literal("WITH_SHEAR_AREAS"),
    strongAxisShearArea: z.number().finite(),
    weakAxisShearArea: z.number().finite(),
  });

export const sectionProfilePropertiesSchema = z.discriminatedUnion("discriminator", [
  standardSectionProfilePropertiesSchema,
  withShearAreasSectionProfilePropertiesSchema,
]).meta({ id: "SectionProfileProperties" });

export const createSectionProfileRequestSchema = z.discriminatedUnion("discriminator", [
  standardSectionProfilePropertiesSchema.extend({
    tempId: z.string().trim().min(1).optional(),
  }),
  withShearAreasSectionProfilePropertiesSchema.extend({
    tempId: z.string().trim().min(1).optional(),
  }),
]).meta({ id: "CreateSectionProfileRequest" });

export const putSectionProfileRequestSchema = z.discriminatedUnion("discriminator", [
  standardSectionProfilePropertiesSchema.extend({
    id: uuidV7Schema,
  }),
  withShearAreasSectionProfilePropertiesSchema.extend({
    id: uuidV7Schema,
  }),
]).meta({ id: "PutSectionProfileRequest" });

export const deleteSectionProfileRequestSchema = z
  .string()
  .trim()
  .min(1)
  .meta({ id: "DeleteSectionProfileRequest" });

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
}).meta({ id: "SectionProfile" });
