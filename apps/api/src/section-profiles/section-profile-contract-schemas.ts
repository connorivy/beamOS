import {
  AreaMomentOfInertiaUnits,
  AreaUnits,
  VolumeUnits,
  WarpingMomentOfInertiaUnits,
} from "unitsnet-js";
import { z } from "zod";
import { uuidV7Schema } from "../common/uuid";
import { SECTION_PROFILE_DISCRIMINATORS } from "./section-profile-entity";

export const sectionPropertiesInputSchema = z
  .object({
    area: z.number().finite(),
    strongAxisMomentOfInertia: z.number().finite(),
    weakAxisMomentOfInertia: z.number().finite(),
    torsionalConstant: z.number().finite(),
    warpingConstant: z.number().finite(),
    strongAxisPlasticSectionModulus: z.number().finite(),
    weakAxisPlasticSectionModulus: z.number().finite(),
    strongAxisElasticSectionModulus: z.number().finite(),
    weakAxisElasticSectionModulus: z.number().finite(),
    units: z.object({
      area: z.enum(AreaUnits),
      areaMomentOfInertia: z.enum(AreaMomentOfInertiaUnits),
      warpingMomentOfInertia: z.enum(WarpingMomentOfInertiaUnits),
      volume: z.enum(VolumeUnits),
    }),
  })
  .meta({ id: "SectionPropertiesInput" });

const sectionProfileBasePropertiesSchema = z.object({
  name: z.string().trim().min(1),
  ...sectionPropertiesInputSchema.shape,
});

export const createSectionProfileRequestSchema =
  sectionProfileBasePropertiesSchema
    .extend({
      strongAxisShearArea: z.number().finite().optional(),
      weakAxisShearArea: z.number().finite().optional(),
    })
    .meta({ id: "CreateSectionProfileRequest" });

export const putSectionProfileRequestSchema =
  createSectionProfileRequestSchema
    .extend({
      newName: z.string().trim().min(1).optional(),
    })
    .meta({ id: "PutSectionProfileRequest" });

export const deleteSectionProfileRequestSchema = z
  .string()
  .trim()
  .min(1)
  .meta({ id: "DeleteSectionProfileRequest" });

export const sectionProfileResponseSchema = z
  .object({
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
  })
  .meta({ id: "SectionProfile" });
