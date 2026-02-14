import { defineEndpoint } from "@beamos/contracts";
import {
  AreaMomentOfInertiaUnits,
  AreaUnits,
  VolumeUnits,
  WarpingMomentOfInertiaUnits,
} from "unitsnet-js";
import { z } from "zod";
import type { AppContext } from "../common/types";
import { httpError } from "../common/http-utils";
import { isUuidV7 } from "../common/uuid";
import { SECTION_PROFILE_DISCRIMINATORS } from "./section-profile-aggregate";

const uuidV7Schema = z
  .uuid()
  .refine((value) => isUuidV7(value), "Must be a valid UUIDv7");

export const getSectionProfileReqSchema = z.object({
  params: z.object({
    sectionProfileId: uuidV7Schema,
  }),
});

export const getSectionProfileResSchema = z.object({
  sectionProfile: z.object({
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
  }),
});

export const getSectionProfile = defineEndpoint({
  method: "GET",
  path: "/api/section-profiles/:sectionProfileId",
  req: getSectionProfileReqSchema,
  res: getSectionProfileResSchema,
  async handler(req, ctx: AppContext) {
    const sectionProfile = await ctx.services.sectionProfileRepository.getById(
      req.params.sectionProfileId,
    );

    if (!sectionProfile) {
      throw httpError("Section profile not found", 404);
    }

    return {
      sectionProfile: {
        id: sectionProfile.id,
        revisionId: sectionProfile.revisionId,
        name: sectionProfile.name,
        discriminator: sectionProfile.discriminator,
        area: {
          value: sectionProfile.area.SquareMeters,
          unit: AreaUnits.SquareMeters as const,
        },
        strongAxisMomentOfInertia: {
          value: sectionProfile.strongAxisMomentOfInertia.MetersToTheFourth,
          unit: AreaMomentOfInertiaUnits.MetersToTheFourth as const,
        },
        weakAxisMomentOfInertia: {
          value: sectionProfile.weakAxisMomentOfInertia.MetersToTheFourth,
          unit: AreaMomentOfInertiaUnits.MetersToTheFourth as const,
        },
        torsionalConstant: {
          value: sectionProfile.torsionalConstant.MetersToTheFourth,
          unit: AreaMomentOfInertiaUnits.MetersToTheFourth as const,
        },
        warpingConstant: {
          value: sectionProfile.warpingConstant.MetersToTheSixth,
          unit: WarpingMomentOfInertiaUnits.MetersToTheSixth as const,
        },
        strongAxisPlasticSectionModulus: {
          value: sectionProfile.strongAxisPlasticSectionModulus.CubicMeters,
          unit: VolumeUnits.CubicMeters as const,
        },
        weakAxisPlasticSectionModulus: {
          value: sectionProfile.weakAxisPlasticSectionModulus.CubicMeters,
          unit: VolumeUnits.CubicMeters as const,
        },
        strongAxisElasticSectionModulus: {
          value: sectionProfile.strongAxisElasticSectionModulus.CubicMeters,
          unit: VolumeUnits.CubicMeters as const,
        },
        weakAxisElasticSectionModulus: {
          value: sectionProfile.weakAxisElasticSectionModulus.CubicMeters,
          unit: VolumeUnits.CubicMeters as const,
        },
        ...(sectionProfile.strongAxisShearArea
          ? {
              strongAxisShearArea: {
                value: sectionProfile.strongAxisShearArea.SquareMeters,
                unit: AreaUnits.SquareMeters as const,
              },
            }
          : {}),
        ...(sectionProfile.weakAxisShearArea
          ? {
              weakAxisShearArea: {
                value: sectionProfile.weakAxisShearArea.SquareMeters,
                unit: AreaUnits.SquareMeters as const,
              },
            }
          : {}),
      },
    };
  },
});
