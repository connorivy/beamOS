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
import type { ModelSettingsUnitsSnapshot } from "../model-settings/model-settings-entity";

const uuidV7Schema = z.uuid().refine((value) => isUuidV7(value), "Must be a valid UUIDv7");
const modelUnitsOverrideSchema = z.enum(["SI"]);
const siModelUnits: ModelSettingsUnitsSnapshot = {
    pressure: PressureUnits.Pascals,
    area: AreaUnits.SquareMeters,
    areaMomentOfInertia: AreaMomentOfInertiaUnits.MetersToTheFourth,
    warpingMomentOfInertia: WarpingMomentOfInertiaUnits.MetersToTheSixth,
    volume: VolumeUnits.CubicMeters,
};

export const getModelRevisionReqSchema = z
    .object({
        params: z.object({
            projectId: uuidV7Schema,
            branchName: z.string().trim().min(1),
        }),
        query: z
            .object({
                units: modelUnitsOverrideSchema.optional(),
            })
            .optional(),
    })
    .meta({ id: "GetModelRevisionRequest" });

export const getModelRevision = defineEndpoint({
    method: "GET",
    path: "/api/projects/:projectId/branches/:branchName",
    req: getModelRevisionReqSchema,
    res: modelRevisionResSchema,
    async handler(req, ctx: AppContext) {
        const modelRevision = await ctx.services.modelRevisionRepository.load(
            req.params.projectId,
            req.params.branchName,
        );

        if (!modelRevision) {
            throw httpError("Model revision not found", 404);
        }
        const responseUnits =
            req.query?.units === "SI" ? siModelUnits : modelRevision.modelSettings.units;

        return {
            id: modelRevision.id,
            parentRevisionId: modelRevision.parentRevisionId,
            secondParentRevisionId: modelRevision.secondParentRevisionId,
            authorId: modelRevision.authorId,
            message: modelRevision.message,
            createdAt: modelRevision.createdAt.toISOString(),
            nodes: modelRevision.nodes.map((node) => ({
                id: node.id,
                location: node.location,
                restraint: node.restraint,
                ...(node.applicationId ? { applicationId: node.applicationId } : {}),
            })),
            materials: modelRevision.materials.map((material) => ({
                id: material.id,
                revisionId: material.revisionId,
                name: material.name,
                modulusOfElasticity: material.modulusOfElasticity.convert(responseUnits.pressure),
                modulusOfRigidity: material.modulusOfRigidity.convert(responseUnits.pressure),
                applicationId: material.applicationId,
                units: {
                    pressure: responseUnits.pressure,
                },
            })),
            modelSettings: {
                id: modelRevision.modelSettings.id,
                revisionId: modelRevision.modelSettings.revisionId,
                units: modelRevision.modelSettings.units,
                yAxisUp: modelRevision.modelSettings.yAxisUp,
            },
            sectionProfiles: modelRevision.sectionProfiles.map((sectionProfile) => ({
                id: sectionProfile.id,
                revisionId: sectionProfile.revisionId,
                name: sectionProfile.name,
                discriminator: sectionProfile.discriminator,
                area: {
                    value: sectionProfile.area.convert(responseUnits.area),
                    unit: responseUnits.area,
                },
                strongAxisMomentOfInertia: {
                    value: sectionProfile.strongAxisMomentOfInertia.convert(
                        responseUnits.areaMomentOfInertia,
                    ),
                    unit: responseUnits.areaMomentOfInertia,
                },
                weakAxisMomentOfInertia: {
                    value: sectionProfile.weakAxisMomentOfInertia.convert(
                        responseUnits.areaMomentOfInertia,
                    ),
                    unit: responseUnits.areaMomentOfInertia,
                },
                torsionalConstant: {
                    value: sectionProfile.torsionalConstant.convert(
                        responseUnits.areaMomentOfInertia,
                    ),
                    unit: responseUnits.areaMomentOfInertia,
                },
                warpingConstant: {
                    value: sectionProfile.warpingConstant.convert(
                        responseUnits.warpingMomentOfInertia,
                    ),
                    unit: responseUnits.warpingMomentOfInertia,
                },
                strongAxisPlasticSectionModulus: {
                    value: sectionProfile.strongAxisPlasticSectionModulus.convert(
                        responseUnits.volume,
                    ),
                    unit: responseUnits.volume,
                },
                weakAxisPlasticSectionModulus: {
                    value: sectionProfile.weakAxisPlasticSectionModulus.convert(
                        responseUnits.volume,
                    ),
                    unit: responseUnits.volume,
                },
                strongAxisElasticSectionModulus: {
                    value: sectionProfile.strongAxisElasticSectionModulus.convert(
                        responseUnits.volume,
                    ),
                    unit: responseUnits.volume,
                },
                weakAxisElasticSectionModulus: {
                    value: sectionProfile.weakAxisElasticSectionModulus.convert(
                        responseUnits.volume,
                    ),
                    unit: responseUnits.volume,
                },
                ...(sectionProfile.strongAxisShearArea
                    ? {
                          strongAxisShearArea: {
                              value: sectionProfile.strongAxisShearArea.convert(responseUnits.area),
                              unit: responseUnits.area,
                          },
                      }
                    : {}),
                ...(sectionProfile.weakAxisShearArea
                    ? {
                          weakAxisShearArea: {
                              value: sectionProfile.weakAxisShearArea.convert(responseUnits.area),
                              unit: responseUnits.area,
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
