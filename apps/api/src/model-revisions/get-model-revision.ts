import { defineEndpoint } from "@beamos/contracts";
import {
  AreaMomentOfInertiaUnits,
  AreaUnits,
  PressureUnits,
  VolumeUnits,
  WarpingMomentOfInertiaUnits,
} from "unitsnet-js";
import { z } from "zod";
import type { AppContext } from "../common/types";
import { httpError } from "../common/http-utils";
import { isUuidV7 } from "../common/uuid";
import { SECTION_PROFILE_DISCRIMINATORS } from "../section-profiles/section-profile-aggregate";

const uuidV7Schema = z
  .uuid()
  .refine((value) => isUuidV7(value), "Must be a valid UUIDv7");

export const getModelRevisionReqSchema = z.object({
  params: z.object({
    modelId: uuidV7Schema,
    branchName: z.string().trim().min(1),
  }),
});

const nodeResSchema = z.object({
  id: uuidV7Schema,
  modelId: uuidV7Schema,
  name: z.string().min(1),
});

const materialResSchema = z.object({
  id: uuidV7Schema,
  revisionId: uuidV7Schema,
  pressureE: z.object({
    value: z.number().finite(),
    unit: z.literal(PressureUnits.Pascals),
  }),
  pressureG: z.object({
    value: z.number().finite(),
    unit: z.literal(PressureUnits.Pascals),
  }),
});

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

const element1dResSchema = z.object({
  id: uuidV7Schema,
  revisionId: uuidV7Schema,
  startNodeId: uuidV7Schema,
  endNodeId: uuidV7Schema,
  materialId: uuidV7Schema,
  sectionProfileId: uuidV7Schema,
});

export const getModelRevisionResSchema = z.object({
  modelRevision: z.object({
    id: uuidV7Schema,
    modelId: uuidV7Schema,
    name: z.string().min(1),
    parentRevisionId: uuidV7Schema.nullable(),
    secondParentRevisionId: uuidV7Schema.nullable(),
    authorId: z.uuid(),
    message: z.string().min(1),
    createdAt: z.date(),
    nodes: z.array(nodeResSchema),
    materials: z.array(materialResSchema),
    sectionProfiles: z.array(sectionProfileResSchema),
    element1ds: z.array(element1dResSchema),
  }),
});

export const getModelRevision = defineEndpoint({
  method: "GET",
  path: "/api/models/:modelId/branches/:branchName/revision",
  req: getModelRevisionReqSchema,
  res: getModelRevisionResSchema,
  async handler(req, ctx: AppContext) {
    const branch = await ctx.services.modelRevisionRepository.getBranchHead(
      req.params.modelId,
      req.params.branchName,
    );

    if (!branch) {
      throw httpError("Model branch not found", 404);
    }

    const modelRevision = await ctx.services.modelRevisionRepository.getRevisionById(
      branch.headRevisionId,
    );

    if (!modelRevision) {
      throw httpError("Model revision not found", 404);
    }

    return {
      modelRevision: {
        id: modelRevision.id,
        modelId: modelRevision.modelId,
        name: modelRevision.name,
        parentRevisionId: modelRevision.parentRevisionId,
        secondParentRevisionId: modelRevision.secondParentRevisionId,
        authorId: modelRevision.authorId,
        message: modelRevision.message,
        createdAt: modelRevision.createdAt,
        nodes: modelRevision.nodes.map((node) => ({
          id: node.id,
          modelId: node.modelId,
          name: node.name,
        })),
        materials: modelRevision.materials.map((material) => ({
          id: material.id,
          revisionId: material.revisionId,
          pressureE: {
            value: material.pressureE.Pascals,
            unit: PressureUnits.Pascals as const,
          },
          pressureG: {
            value: material.pressureG.Pascals,
            unit: PressureUnits.Pascals as const,
          },
        })),
        sectionProfiles: modelRevision.sectionProfiles.map((sectionProfile) => ({
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
        })),
        element1ds: modelRevision.element1ds.map((element1d) => ({
          id: element1d.id,
          revisionId: element1d.revisionId,
          startNodeId: element1d.startNodeId,
          endNodeId: element1d.endNodeId,
          materialId: element1d.materialId,
          sectionProfileId: element1d.sectionProfileId,
        })),
      },
    };
  },
});
