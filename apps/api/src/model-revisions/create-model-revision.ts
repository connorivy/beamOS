import { defineEndpoint } from "../contracts/endpoint";
import {
    createModelRevisionReqSchema,
    modelRevisionResSchema,
} from "./create-model-revision-request-schema";
import { AppContext } from "src/common/types";
import { httpError } from "src/common/http-utils";
import { getDb } from "src/db/client";
import { modelBranchHeads } from "src/db/schema";
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
import { ModelRevisionAggregate } from "./model-revision-aggregate";

export const createModelRevision = defineEndpoint({
    method: "POST",
    path: "/api/projects/:projectId/branches/:branchName/revisions",
    req: createModelRevisionReqSchema,
    res: modelRevisionResSchema,
    async handler(req, ctx: AppContext) {
        const headRevision = await ctx.services.modelRevisionRepository.load(
            req.params.projectId,
            req.params.branchName,
        );
        if (!headRevision) {
            throw httpError(
                `Could not find branch ${req.params.branchName} for project ${req.params.projectId}`,
                404,
            );
        }
        const snapshot = headRevision.toSnapshot();

        const now = new Date();
        const revision = ModelRevisionAggregate.create({
            ...snapshot,
            id: Bun.randomUUIDv7(),
            parentRevisionId: headRevision.id,
            secondParentRevisionId: null,
            createdAt: now,
        });

        applyModelRevisionOperations({ req, revision });

        const modelRevision = await ctx.services.modelRevisionRepository.save(revision);
        await getDb()
            .insert(modelBranchHeads)
            .values({
                projectId: req.params.projectId,
                branchName: req.params.branchName,
                headRevisionId: modelRevision.id,
                updatedAt: now,
            })
            .onConflictDoUpdate({
                target: [modelBranchHeads.projectId, modelBranchHeads.branchName],
                set: {
                    headRevisionId: modelRevision.id,
                    updatedAt: now,
                },
            });

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
                location: node.location,
                restraint: node.restraint,
            })),
            materials: modelRevision.materials.map((material) => ({
                id: material.id,
                revisionId: material.revisionId,
                name: material.name,
                modulusOfElasticity: material.modulusOfElasticity.Pascals,
                modulusOfRigidity: material.modulusOfRigidity.Pascals,
                units: {
                    pressure: PressureUnits.Pascals as const,
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

const applyModelRevisionOperations = (input: {
    req: z.infer<typeof createModelRevisionReqSchema>;
    revision: ModelRevisionAggregate;
}) => {
    const { req, revision } = input;

    try {
        if (req.body.nodes) {
            revision.applyNodeChanges(req.body.nodes);
        }
        if (req.body.materials) {
            revision.applyMaterialChanges(req.body.materials);
        }
        if (req.body.sectionProfiles) {
            revision.applySectionProfileChanges(req.body.sectionProfiles);
        }
        if (req.body.element1ds) {
            revision.applyElement1dChanges(req.body.element1ds);
        }
        if (req.body.loadCases) {
            revision.applyLoadCaseChanges(req.body.loadCases);
        }
        if (req.body.loadCombinations) {
            revision.applyLoadCombinationChanges(req.body.loadCombinations);
        }
        if (req.body.pointLoads) {
            revision.applyPointLoadChanges(req.body.pointLoads);
        }
        if (req.body.modelSettings) {
            revision.applyModelSettingsUpdate(req.body.modelSettings);
        }
    } catch (error) {
        if (error instanceof Error) {
            throw httpError(error.message, 400);
        }
        throw error;
    }
};
