import { defineEndpoint } from "../contracts/endpoint";
import {
    createModelRevisionReqSchema,
    modelRevisionResSchema,
} from "./create-model-revision-request-schema";
import { AppContext } from "src/common/types";
import { httpError } from "src/common/http-utils";
import { DbTransaction, getDb } from "src/db/client";
import { modelBranchHeads, modelRevisions } from "src/db/schema";
import { ModelRevisionAggregate } from "./model-revision-aggregate";
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

export const createModelRevision = defineEndpoint({
    method: "POST",
    path: "/api/projects/:projectId/branches/:branchName/revisions",
    req: createModelRevisionReqSchema,
    res: modelRevisionResSchema,
    async handler(req, ctx: AppContext) {
        const revision = await createNewRevisionAggregateHandler(req, ctx, "Create model revision");

        applyModelRevisionOperations({ req, revision });

        const modelRevision = await getDb().transaction(async (tx) =>
            ctx.services.modelRevisionRepository.save({
                revision,
                branchName: req.params.branchName,
                tx,
            }),
        );

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
                modulusOfElasticity: material.modulusOfElasticity.Pascals,
                modulusOfRigidity: material.this.modulusOfRigidity.Pascals,
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
        revision.addNodeOperations(req.body.nodes);
        revision.addMaterialOperations(req.body.materials);
        revision.addSectionProfileOperations(req.body.sectionProfiles);
        revision.addElement1dOperations(req.body.element1ds);
        revision.addLoadCaseOperations(req.body.loadCases);
        revision.addLoadCombinationOperations(req.body.loadCombinations);
        revision.addPointLoadOperations(req.body.pointLoads);
    } catch (error) {
        if (error instanceof Error) {
            throw httpError(error.message, 400);
        }
        throw error;
    }
};

export async function createNewRevisionHandler(
    req: {
        params: { projectId: string; branchName: string };
    },
    ctx: AppContext,
    tx: DbTransaction,
) {
    const { projectId, branchName } = req.params;
    const branch = await ctx.services.modelRevisionRepository.getBranchHead(projectId, branchName);
    if (!branch) {
        throw httpError(`Could not find branch ${branchName} on model with ID ${projectId}`, 404);
    }

    const parentRevision = await ctx.services.modelRevisionRepository.getRevisionById(
        branch.headRevisionId,
    );
    if (!parentRevision) {
        throw httpError(
            `Could not find parent revision ${branch.headRevisionId} for branch ${branchName}`,
            404,
        );
    }

    const revisionId = Bun.randomUUIDv7();
    const now = new Date();
    await tx.insert(modelRevisions).values({
        id: revisionId,
        projectId,
        parentRevisionId: parentRevision.id,
        secondParentRevisionId: null,
        authorId: parentRevision.authorId,
        message: "Batch create materials",
        createdAt: now,
    });

    await tx
        .insert(modelBranchHeads)
        .values({
            projectId,
            branchName,
            headRevisionId: revisionId,
            updatedAt: now,
        })
        .onConflictDoUpdate({
            target: [modelBranchHeads.projectId, modelBranchHeads.branchName],
            set: {
                headRevisionId: revisionId,
                updatedAt: now,
            },
        });
    return revisionId;
}

export async function createNewRevisionAggregateHandler(
    req: {
        params: { projectId: string; branchName: string };
    },
    ctx: AppContext,
    message = "Batch create materials",
) {
    const { projectId, branchName } = req.params;
    const branch = await ctx.services.modelRevisionRepository.getBranchHead(projectId, branchName);
    if (!branch) {
        throw httpError(`Could not find branch ${branchName} on model with ID ${projectId}`, 404);
    }

    const parentRevision = await ctx.services.modelRevisionRepository.getRevisionById(
        branch.headRevisionId,
    );
    if (!parentRevision) {
        throw httpError(
            `Could not find parent revision ${branch.headRevisionId} for branch ${branchName}`,
            404,
        );
    }

    return ModelRevisionAggregate.create({
        projectId,
        parentRevisionId: parentRevision.id,
        secondParentRevisionId: null,
        authorId: parentRevision.authorId,
        message,
        createdAt: new Date(),
        nodes: parentRevision.nodes.map((node) => node.toSnapshot()),
        materials: parentRevision.materials.map((material) => material.toSnapshot()),
        modelSettings: parentRevision.modelSettings.toSnapshot(),
        sectionProfiles: parentRevision.sectionProfiles.map((sectionProfile) =>
            sectionProfile.toSnapshot(),
        ),
        element1ds: parentRevision.element1ds.map((element1d) => element1d.toSnapshot()),
        loadCases: parentRevision.loadCases.map((loadCase) => loadCase.toSnapshot()),
        loadCombinations: parentRevision.loadCombinations.map((loadCombination) =>
            loadCombination.toSnapshot(),
        ),
        pointLoads: parentRevision.pointLoads.map((pointLoad) => pointLoad.toSnapshot()),
    });
}
