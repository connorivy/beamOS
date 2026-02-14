import { defineEndpoint } from "@beamos/contracts";
import {
  Area,
  AreaMomentOfInertia,
  AreaMomentOfInertiaUnits,
  AreaUnits,
  Volume,
  VolumeUnits,
  WarpingMomentOfInertia,
  WarpingMomentOfInertiaUnits,
} from "unitsnet-js";
import { z } from "zod";
import type { AppContext } from "../common/types";
import { httpError } from "../common/http-utils";
import { isUuidV7 } from "../common/uuid";
import {
  SECTION_PROFILE_DISCRIMINATORS,
  SectionProfileAggregate,
} from "./section-profile-aggregate";

const uuidV7Schema = z
  .uuid()
  .refine((value) => isUuidV7(value), "Must be a valid UUIDv7");

const sectionProfileUnitsInputSchema = z.object({
  area: z.enum(AreaUnits),
  areaMomentOfInertia: z.enum(AreaMomentOfInertiaUnits),
  warpingMomentOfInertia: z.enum(WarpingMomentOfInertiaUnits),
  volume: z.enum(VolumeUnits),
});

const sectionPropertiesInputSchema = z.object({
  area: z.number().finite(),
  strongAxisMomentOfInertia: z.number().finite(),
  weakAxisMomentOfInertia: z.number().finite(),
  torsionalConstant: z.number().finite(),
  warpingConstant: z.number().finite(),
  strongAxisPlasticSectionModulus: z.number().finite(),
  weakAxisPlasticSectionModulus: z.number().finite(),
  strongAxisElasticSectionModulus: z.number().finite(),
  weakAxisElasticSectionModulus: z.number().finite(),
});

const baseSectionInputSchema = z.object({
  tempId: z.string().trim().min(1).optional(),
  name: z.string().trim().min(1),
  ...sectionPropertiesInputSchema.shape,
});

const standardSectionInputSchema = baseSectionInputSchema.extend({
  discriminator: z.literal("STANDARD"),
});

const withShearAreasSectionInputSchema = baseSectionInputSchema.extend({
  discriminator: z.literal("WITH_SHEAR_AREAS"),
  strongAxisShearArea: z.number().finite(),
  weakAxisShearArea: z.number().finite(),
});

const sectionInputSchema = z.discriminatedUnion("discriminator", [
  standardSectionInputSchema,
  withShearAreasSectionInputSchema,
]);

const sectionProfileResSchema = z.object({
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

export const batchCreateSectionProfileReqSchema = z.object({
  params: z.object({
    modelId: uuidV7Schema,
    branchName: z.string().trim().min(1),
  }),
  body: z.object({
    units: sectionProfileUnitsInputSchema,
    sectionProfiles: z.array(sectionInputSchema).min(1),
  }),
});

export const batchCreateSectionProfileResSchema = z.object({
  sectionProfiles: z.array(sectionProfileResSchema),
  tempIdToId: z.record(z.string(), uuidV7Schema),
});

const toDomainProperties = (
  input: z.infer<typeof sectionPropertiesInputSchema>,
  units: z.infer<typeof sectionProfileUnitsInputSchema>,
) => {
  return {
    area: new Area(input.area, units.area),
    strongAxisMomentOfInertia: new AreaMomentOfInertia(
      input.strongAxisMomentOfInertia,
      units.areaMomentOfInertia,
    ),
    weakAxisMomentOfInertia: new AreaMomentOfInertia(
      input.weakAxisMomentOfInertia,
      units.areaMomentOfInertia,
    ),
    torsionalConstant: new AreaMomentOfInertia(
      input.torsionalConstant,
      units.areaMomentOfInertia,
    ),
    warpingConstant: new WarpingMomentOfInertia(
      input.warpingConstant,
      units.warpingMomentOfInertia,
    ),
    strongAxisPlasticSectionModulus: new Volume(
      input.strongAxisPlasticSectionModulus,
      units.volume,
    ),
    weakAxisPlasticSectionModulus: new Volume(
      input.weakAxisPlasticSectionModulus,
      units.volume,
    ),
    strongAxisElasticSectionModulus: new Volume(
      input.strongAxisElasticSectionModulus,
      units.volume,
    ),
    weakAxisElasticSectionModulus: new Volume(
      input.weakAxisElasticSectionModulus,
      units.volume,
    ),
  };
};

const toResponseSectionProfile = (sectionProfile: SectionProfileAggregate) => ({
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
});

export const batchCreateSectionProfile = defineEndpoint({
  method: "POST",
  path: "/api/models/:modelId/branches/:branchName/section-profiles/batch",
  req: batchCreateSectionProfileReqSchema,
  res: batchCreateSectionProfileResSchema,
  async handler(req, ctx: AppContext) {
    const seenTempIds = new Set<string>();

    for (const sectionProfile of req.body.sectionProfiles) {
      if (!sectionProfile.tempId) {
        continue;
      }

      if (seenTempIds.has(sectionProfile.tempId)) {
        throw httpError(`Duplicate tempId \"${sectionProfile.tempId}\"`, 400);
      }

      seenTempIds.add(sectionProfile.tempId);
    }

    const tempIdToId: Record<string, string> = {};
    const { modelId, branchName } = req.params;
    
    // Create a new revision and update branch head
    const newRevisionId = await ctx.services.modelRevisionRepository.createRevisionAndUpdateBranchHead({
      modelId,
      branchName,
      authorId: Bun.randomUUIDv7(),
      message: "Add section profiles",
    });

    const entities = req.body.sectionProfiles.map((sectionProfile) => {
      const id = Bun.randomUUIDv7();

      if (sectionProfile.tempId) {
        tempIdToId[sectionProfile.tempId] = id;
      }

      const converted = toDomainProperties(
        sectionProfile,
        req.body.units,
      );

      return SectionProfileAggregate.create({
        id,
        revisionId: newRevisionId,
        name: sectionProfile.name,
        discriminator: sectionProfile.discriminator,
        area: converted.area,
        strongAxisMomentOfInertia: converted.strongAxisMomentOfInertia,
        weakAxisMomentOfInertia: converted.weakAxisMomentOfInertia,
        torsionalConstant: converted.torsionalConstant,
        warpingConstant: converted.warpingConstant,
        strongAxisPlasticSectionModulus:
          converted.strongAxisPlasticSectionModulus,
        weakAxisPlasticSectionModulus: converted.weakAxisPlasticSectionModulus,
        strongAxisElasticSectionModulus:
          converted.strongAxisElasticSectionModulus,
        weakAxisElasticSectionModulus: converted.weakAxisElasticSectionModulus,
        strongAxisShearArea: sectionProfile.strongAxisShearArea
          ? new Area(sectionProfile.strongAxisShearArea, req.body.units.area)
          : undefined,
        weakAxisShearArea: sectionProfile.weakAxisShearArea
          ? new Area(sectionProfile.weakAxisShearArea, req.body.units.area)
          : undefined,
      });
    });

    const saved =
      await ctx.services.sectionProfileRepository.batchCreate(entities);

    return {
      sectionProfiles: saved.map((sectionProfile) =>
        toResponseSectionProfile(sectionProfile),
      ),
      tempIdToId,
    };
  },
});
