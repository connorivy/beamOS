import { defineEndpoint } from "../contracts/endpoint";
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
import { getDb } from "../db/client";
import { SectionProfileAggregate } from "./section-profile-aggregate";
import { sectionProfileResponseSchema } from "./section-profile-response-schema";
import { uuidV7Schema } from "src/common/uuid";
import { createNewRevisionAggregateHandler } from "src/model-revisions/create-model-revision";
import { ModelRevisionAggregate } from "src/model-revisions/model-revision-aggregate";
import {
  createSectionProfileRequestSchema,
  sectionPropertiesInputSchema,
} from "./create-section-profile-request-schema";

const sectionProfileUnitsInputSchema = z.object({
  area: z.enum(AreaUnits),
  areaMomentOfInertia: z.enum(AreaMomentOfInertiaUnits),
  warpingMomentOfInertia: z.enum(WarpingMomentOfInertiaUnits),
  volume: z.enum(VolumeUnits),
});

export const batchCreateSectionProfileReqSchema = z.object({
  params: z.object({
    modelId: uuidV7Schema,
    branchName: z.string().trim().min(1),
  }),
  body: z.object({
    units: sectionProfileUnitsInputSchema,
    sectionProfiles: z.array(createSectionProfileRequestSchema).min(1),
  }),
});

export const batchCreateSectionProfileResSchema = z.object({
  sectionProfiles: z.array(sectionProfileResponseSchema),
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
    const revision = await createNewRevisionAggregateHandler(
      req,
      ctx,
      "Batch create section profiles",
    );
    return await batchCreateSectionProfileHandler(req, ctx, revision);
  },
});
async function batchCreateSectionProfileHandler(
  req: {
    params: { modelId: string; branchName: string };
    body: {
      units: {
        area: AreaUnits;
        areaMomentOfInertia: AreaMomentOfInertiaUnits;
        warpingMomentOfInertia: WarpingMomentOfInertiaUnits;
        volume: VolumeUnits;
      };
      sectionProfiles: (
        | {
            area: number;
            strongAxisMomentOfInertia: number;
            weakAxisMomentOfInertia: number;
            torsionalConstant: number;
            warpingConstant: number;
            strongAxisPlasticSectionModulus: number;
            weakAxisPlasticSectionModulus: number;
            strongAxisElasticSectionModulus: number;
            weakAxisElasticSectionModulus: number;
            name: string;
            discriminator: "STANDARD";
            tempId?: string | undefined;
          }
        | {
            area: number;
            strongAxisMomentOfInertia: number;
            weakAxisMomentOfInertia: number;
            torsionalConstant: number;
            warpingConstant: number;
            strongAxisPlasticSectionModulus: number;
            weakAxisPlasticSectionModulus: number;
            strongAxisElasticSectionModulus: number;
            weakAxisElasticSectionModulus: number;
            name: string;
            discriminator: "WITH_SHEAR_AREAS";
            strongAxisShearArea: number;
            weakAxisShearArea: number;
            tempId?: string | undefined;
          }
      )[];
    };
  },
  ctx: AppContext,
  revision: ModelRevisionAggregate,
) {
  const seenTempIds = new Set<string>();

  for (const sectionProfile of req.body.sectionProfiles) {
    if (!sectionProfile.tempId) {
      continue;
    }

    if (seenTempIds.has(sectionProfile.tempId)) {
      throw httpError(`Duplicate tempId "${sectionProfile.tempId}"`, 400);
    }

    seenTempIds.add(sectionProfile.tempId);
  }

  const tempIdToId: Record<string, string> = {};

  const entities = req.body.sectionProfiles.map((sectionProfile) => {
    const id = Bun.randomUUIDv7();

    if (sectionProfile.tempId) {
      tempIdToId[sectionProfile.tempId] = id;
    }

    const converted = toDomainProperties(sectionProfile, req.body.units);
    const shearAreas =
      sectionProfile.discriminator === "WITH_SHEAR_AREAS"
        ? {
            strongAxisShearArea: new Area(
              sectionProfile.strongAxisShearArea,
              req.body.units.area,
            ),
            weakAxisShearArea: new Area(
              sectionProfile.weakAxisShearArea,
              req.body.units.area,
            ),
          }
        : {
            strongAxisShearArea: undefined,
            weakAxisShearArea: undefined,
          };

    return SectionProfileAggregate.create({
      id,
      revisionId: revision.id,
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
      strongAxisShearArea: shearAreas.strongAxisShearArea,
      weakAxisShearArea: shearAreas.weakAxisShearArea,
    });
  });

  for (const sectionProfile of entities) {
    revision.addSectionProfile(sectionProfile.toSnapshot());
  }

  await getDb().transaction(async (tx) => {
    await ctx.services.modelRevisionRepository.save({
      revision,
      newRevision: true,
      tx,
    });
  });

  return {
    sectionProfiles: entities.map((sectionProfile) =>
      toResponseSectionProfile(sectionProfile),
    ),
    tempIdToId,
  };
}
