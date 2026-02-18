import { defineEndpoint } from "../contracts/endpoint";
import {
  AreaMomentOfInertiaUnits,
  AreaUnits,
  ForceUnits,
  PressureUnits,
  TorqueUnits,
  VolumeUnits,
  WarpingMomentOfInertiaUnits,
} from "unitsnet-js";
import { z } from "zod";
import type { AppContext } from "../common/types";
import { httpError } from "../common/http-utils";
import { isUuidV7 } from "../common/uuid";
import { modelRevisionResSchema } from "./create-model-revision-request-schema";

const uuidV7Schema = z
  .uuid()
  .refine((value) => isUuidV7(value), "Must be a valid UUIDv7");

export const getModelRevisionReqSchema = z
  .object({
    params: z.object({
      projectId: uuidV7Schema,
      branchName: z.string().trim().min(1),
    }),
  })
  .meta({ id: "GetModelRevisionRequest" });

export const getModelRevision = defineEndpoint({
  method: "GET",
  path: "/api/projects/:projectId/branches/:branchName/revisions",
  req: getModelRevisionReqSchema,
  res: modelRevisionResSchema,
  async handler(req, ctx: AppContext) {
    const branch = await ctx.services.modelRevisionRepository.getBranchHead(
      req.params.projectId,
      req.params.branchName,
    );

    if (!branch) {
      throw httpError("Model branch not found", 404);
    }

    const modelRevision =
      await ctx.services.modelRevisionRepository.getRevisionById(
        branch.headRevisionId,
      );

    if (!modelRevision) {
      throw httpError("Model revision not found", 404);
    }

    return {
      id: modelRevision.id,
      projectId: modelRevision.projectId,
      parentRevisionId: modelRevision.parentRevisionId,
      secondParentRevisionId: modelRevision.secondParentRevisionId,
      authorId: modelRevision.authorId,
      message: modelRevision.message,
      createdAt: modelRevision.createdAt.toISOString(),
      nodes: modelRevision.nodes.map((node) => ({
        id: node.id,
        projectId: modelRevision.projectId,
        nodeTypeDescriminator:
          node.toSnapshot().nodeTypeDescriminator ??
          (node.nodeType === "internalNode" ? "internal" : "external"),
      })),
      materials: modelRevision.materials.map((material) => ({
        id: material.id,
        revisionId: material.revisionId,
        name: material.name,
        modulusOfElasticity: material.pressureE.Pascals,
        modulusOfRigidity: material.pressureG.Pascals,
        units: {
          pressure: PressureUnits.Pascals as const,
        },
      })),
      modelSettings: modelRevision.modelSettings
        ? {
            id: modelRevision.modelSettings.id,
            revisionId: modelRevision.modelSettings.revisionId,
            units: modelRevision.modelSettings.units,
            yAxisUp: modelRevision.modelSettings.yAxisUp,
          }
        : null,
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
      loadCases: modelRevision.loadCases.map((loadCase) => ({
        id: loadCase.id,
        revisionId: loadCase.revisionId,
        name: loadCase.name,
      })),
      loadCombinations: modelRevision.loadCombinations.map((loadCombination) => ({
        id: loadCombination.id,
        revisionId: loadCombination.revisionId,
        loadCaseFactors: { ...loadCombination.loadCaseFactors },
      })),
      pointLoads: modelRevision.pointLoads.map((pointLoad) => ({
        id: pointLoad.id,
        revisionId: pointLoad.revisionId,
        nodeId: pointLoad.nodeId,
        loadCaseId: pointLoad.loadCaseId,
        force: {
          forceAlongX: pointLoad.force.forceAlongX.Newtons,
          forceAlongY: pointLoad.force.forceAlongY.Newtons,
          forceAlongZ: pointLoad.force.forceAlongZ.Newtons,
          momentAboutX: pointLoad.force.momentAboutX.NewtonMeters,
          momentAboutY: pointLoad.force.momentAboutY.NewtonMeters,
          momentAboutZ: pointLoad.force.momentAboutZ.NewtonMeters,
        },
        direction: pointLoad.direction,
        units: {
          force: ForceUnits.Newtons as const,
          torque: TorqueUnits.NewtonMeters as const,
        },
      })),
    };
  },
});
