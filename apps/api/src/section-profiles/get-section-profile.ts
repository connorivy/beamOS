import { defineEndpoint } from "../contracts/endpoint";
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
import { sectionProfileResponseSchema } from "./section-profile-contract-schemas";

const uuidV7Schema = z
  .uuid()
  .refine((value) => isUuidV7(value), "Must be a valid UUIDv7");

export const getSectionProfileReqSchema = z.object({
  params: z.object({
    sectionProfileId: uuidV7Schema,
  }),
});

export const getSectionProfileResSchema = z.object({
  sectionProfile: sectionProfileResponseSchema,
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
