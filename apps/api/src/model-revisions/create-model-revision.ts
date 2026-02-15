import { defineEndpoint } from "../contracts/endpoint";
import { modelRevisionResponseSchema } from "./model-revision-response-schema";
import { createModelRevisionReqSchema } from "./create-model-revision-request-schema";
import { AppContext } from "src/common/types";
import { httpError } from "src/common/http-utils";
import { DbTransaction, getDb } from "src/db/client";
import { modelBranchHeads, modelRevisions } from "src/db/schema";
import { RevisionChangeEntity } from "src/revision-changes/revision-change-entity";
import { ModelRevisionAggregate } from "./model-revision-aggregate";
import {
  AreaMomentOfInertiaUnits,
  AreaUnits,
  Pressure,
  PressureUnits,
  VolumeUnits,
  WarpingMomentOfInertiaUnits,
} from "unitsnet-js";
import type { Element1dSnapshot } from "src/element1ds/element1d-entity";
import type { MaterialSnapshot } from "src/materials/material-entity";
import type { NodeSnapshot } from "src/nodes/node-entity";
import type { SectionProfileSnapshot } from "src/section-profiles/section-profile-entity";
import { z } from "zod";

export const createModelRevision = defineEndpoint({
  method: "POST",
  path: "/api/models/:modelId/branches/:branchName/revisions",
  req: createModelRevisionReqSchema,
  res: modelRevisionResponseSchema,
  async handler(req, ctx: AppContext) {
    const branch = await ctx.services.modelRevisionRepository.getBranchHead(
      req.params.modelId,
      req.params.branchName,
    );
    if (!branch) {
      throw httpError(
        `Could not find branch ${req.params.branchName} on model with ID ${req.params.modelId}`,
        404,
      );
    }

    const parentRevision = await ctx.services.modelRevisionRepository.getRevisionById(
      branch.headRevisionId,
    );
    if (!parentRevision) {
      throw httpError(
        `Could not find parent revision ${branch.headRevisionId} for branch ${req.params.branchName}`,
        404,
      );
    }

    const currentNodesById = new Map(
      parentRevision.nodes.map((node) => [node.id, node.toSnapshot()] as const),
    );
    const currentMaterialsById = new Map(
      parentRevision.materials.map((material) => [material.id, material.toSnapshot()] as const),
    );
    const currentSectionProfilesById = new Map(
      parentRevision.sectionProfiles.map((sectionProfile) => [sectionProfile.id, sectionProfile.toSnapshot()] as const),
    );
    const currentElement1dsById = new Map(
      parentRevision.element1ds.map((element1d) => [element1d.id, element1d.toSnapshot()] as const),
    );

    const revisionId = await getDb().transaction(async (tx) => {
      const revisionId = await createNewRevisionHandler(req, ctx, tx);
      const createdAt = new Date();

      const changes = buildRevisionChanges({
        req,
        revisionId,
        createdAt,
        currentNodesById,
        currentMaterialsById,
        currentSectionProfilesById,
        currentElement1dsById,
      });

      await ctx.services.revisionChangeRepository.batchCreate(tx, changes);
      return revisionId;
    });

    const modelRevision = await ctx.services.modelRevisionRepository.getRevisionById(
      revisionId,
    );
    if (!modelRevision) {
      throw httpError("Failed to load created model revision", 500);
    }

    return {
      modelRevision: {
        id: modelRevision.id,
        version: {
          modelId: modelRevision.modelId,
          branchName: req.params.branchName,
          revisionId: modelRevision.id,
          revisionsAhead: 0,
          revisionsBehind: 0,
          inProgressRevisionId: modelRevision.id,
        },
        modelId: modelRevision.modelId,
        name: modelRevision.name,
        parentRevisionId: modelRevision.parentRevisionId,
        secondParentRevisionId: modelRevision.secondParentRevisionId,
        authorId: modelRevision.authorId,
        message: modelRevision.message,
        createdAt: modelRevision.createdAt.toISOString(),
        nodes: modelRevision.nodes.map((node) => ({
          id: node.id,
          modelId: modelRevision.modelId,
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

const buildRevisionChanges = (input: {
  req: z.infer<typeof createModelRevisionReqSchema>;
  revisionId: string;
  createdAt: Date;
  currentNodesById: Map<string, NodeSnapshot>;
  currentMaterialsById: Map<string, MaterialSnapshot>;
  currentSectionProfilesById: Map<string, SectionProfileSnapshot>;
  currentElement1dsById: Map<string, Element1dSnapshot>;
}): RevisionChangeEntity[] => {
  const changes: RevisionChangeEntity[] = [];
  const toEntity = (change: {
    entityType: "node" | "material" | "sectionprofile" | "element1d";
    entityId: string;
    op: "insert" | "update" | "delete";
    payload: Record<string, unknown>;
  }) =>
    RevisionChangeEntity.create({
      id: Bun.randomUUIDv7(),
      revisionId: input.revisionId,
      entityType: change.entityType,
      entityId: change.entityId,
      schemaVersion: 1,
      op: change.op,
      payload: change.payload,
      createdAt: input.createdAt,
    });

  for (const createNode of input.req.body.nodes.create) {
    const id = Bun.randomUUIDv7();
    const payload =
      createNode.location.type === "internal"
        ? {
            id,
            modelRevisionId: input.revisionId,
            nodeType: "internalNode",
            nodeTypeDescriminator: "internal" as const,
            element1dId: createNode.location.element1dId,
            distanceAlongElement1d: createNode.location.ratioAlongElement1d,
            restraint: createNode.restraint,
          }
        : {
            id,
            modelRevisionId: input.revisionId,
            nodeType: "spatialNode",
            nodeTypeDescriminator: "external" as const,
            point: createNode.location.point,
            restraint: createNode.restraint,
          };

    changes.push(
      toEntity({
        entityType: "node",
        entityId: id,
        op: "insert",
        payload,
      }),
    );
  }

  for (const putNode of input.req.body.nodes.update) {
    const existing = input.currentNodesById.get(putNode.id);
    if (!existing) {
      throw httpError(`Node ${putNode.id} not found`, 400);
    }
    const payload =
      putNode.location.type === "internal"
        ? {
            id: putNode.id,
            modelRevisionId: input.revisionId,
            nodeType: "internalNode",
            nodeTypeDescriminator: "internal" as const,
            element1dId: putNode.location.element1dId,
            distanceAlongElement1d: putNode.location.ratioAlongElement1d,
            restraint: putNode.restraint,
          }
        : {
            id: putNode.id,
            modelRevisionId: input.revisionId,
            nodeType: "spatialNode",
            nodeTypeDescriminator: "external" as const,
            point: putNode.location.point,
            restraint: putNode.restraint,
          };

    changes.push(
      toEntity({
        entityType: "node",
        entityId: putNode.id,
        op: "update",
        payload,
      }),
    );
  }

  for (const deleteNodeId of input.req.body.nodes.delete) {
    changes.push(
      toEntity({
        entityType: "node",
        entityId: deleteNodeId,
        op: "delete",
        payload: { id: deleteNodeId },
      }),
    );
  }

  for (const createMaterial of input.req.body.materials.create) {
    const id = Bun.randomUUIDv7();
    changes.push(
      toEntity({
        entityType: "material",
        entityId: id,
        op: "insert",
        payload: {
          id,
          revisionId: input.revisionId,
          name: createMaterial.name,
          pressureE: {
            value: new Pressure(
              createMaterial.modulusOfElasticity,
              createMaterial.units.pressure,
            ).Pascals,
            unit: "Pascals",
          },
          pressureG: {
            value: new Pressure(
              createMaterial.modulusOfRigidity,
              createMaterial.units.pressure,
            ).Pascals,
            unit: "Pascals",
          },
        },
      }),
    );
  }

  for (const putMaterial of input.req.body.materials.update) {
    const existing = input.currentMaterialsById.get(putMaterial.id);
    if (!existing) {
      throw httpError(`Material ${putMaterial.id} not found`, 400);
    }

    changes.push(
      toEntity({
        entityType: "material",
        entityId: putMaterial.id,
        op: "update",
        payload: {
          id: putMaterial.id,
          revisionId: input.revisionId,
          name: putMaterial.name,
          pressureE: {
            value: new Pressure(
              putMaterial.modulusOfElasticity,
              putMaterial.units.pressure,
            ).Pascals,
            unit: "Pascals",
          },
          pressureG: {
            value: new Pressure(
              putMaterial.modulusOfRigidity,
              putMaterial.units.pressure,
            ).Pascals,
            unit: "Pascals",
          },
        },
      }),
    );
  }

  for (const deleteMaterialId of input.req.body.materials.delete) {
    changes.push(
      toEntity({
        entityType: "material",
        entityId: deleteMaterialId,
        op: "delete",
        payload: { id: deleteMaterialId },
      }),
    );
  }

  for (const createSectionProfile of input.req.body.sectionProfiles.create) {
    const id = Bun.randomUUIDv7();
    changes.push(
      toEntity({
        entityType: "sectionprofile",
        entityId: id,
        op: "insert",
        payload: {
          id,
          revisionId: input.revisionId,
          name: createSectionProfile.name,
          discriminator: createSectionProfile.discriminator,
          area: {
            value: createSectionProfile.area,
            unit: "SquareMeters",
          },
          strongAxisMomentOfInertia: {
            value: createSectionProfile.strongAxisMomentOfInertia,
            unit: "MetersToTheFourth",
          },
          weakAxisMomentOfInertia: {
            value: createSectionProfile.weakAxisMomentOfInertia,
            unit: "MetersToTheFourth",
          },
          torsionalConstant: {
            value: createSectionProfile.torsionalConstant,
            unit: "MetersToTheFourth",
          },
          warpingConstant: {
            value: createSectionProfile.warpingConstant,
            unit: "MetersToTheSixth",
          },
          strongAxisPlasticSectionModulus: {
            value: createSectionProfile.strongAxisPlasticSectionModulus,
            unit: "CubicMeters",
          },
          weakAxisPlasticSectionModulus: {
            value: createSectionProfile.weakAxisPlasticSectionModulus,
            unit: "CubicMeters",
          },
          strongAxisElasticSectionModulus: {
            value: createSectionProfile.strongAxisElasticSectionModulus,
            unit: "CubicMeters",
          },
          weakAxisElasticSectionModulus: {
            value: createSectionProfile.weakAxisElasticSectionModulus,
            unit: "CubicMeters",
          },
          ...(createSectionProfile.discriminator === "WITH_SHEAR_AREAS"
            ? {
                strongAxisShearArea: {
                  value: createSectionProfile.strongAxisShearArea,
                  unit: "SquareMeters",
                },
                weakAxisShearArea: {
                  value: createSectionProfile.weakAxisShearArea,
                  unit: "SquareMeters",
                },
              }
            : {}),
        },
      }),
    );
  }

  for (const putSectionProfile of input.req.body.sectionProfiles.update) {
    const existing = input.currentSectionProfilesById.get(putSectionProfile.id);
    if (!existing) {
      throw httpError(`Section profile ${putSectionProfile.id} not found`, 400);
    }

    changes.push(
      toEntity({
        entityType: "sectionprofile",
        entityId: putSectionProfile.id,
        op: "update",
        payload: {
          id: putSectionProfile.id,
          revisionId: input.revisionId,
          name: putSectionProfile.name,
          discriminator: putSectionProfile.discriminator,
          area: {
            value: putSectionProfile.area,
            unit: "SquareMeters",
          },
          strongAxisMomentOfInertia: {
            value: putSectionProfile.strongAxisMomentOfInertia,
            unit: "MetersToTheFourth",
          },
          weakAxisMomentOfInertia: {
            value: putSectionProfile.weakAxisMomentOfInertia,
            unit: "MetersToTheFourth",
          },
          torsionalConstant: {
            value: putSectionProfile.torsionalConstant,
            unit: "MetersToTheFourth",
          },
          warpingConstant: {
            value: putSectionProfile.warpingConstant,
            unit: "MetersToTheSixth",
          },
          strongAxisPlasticSectionModulus: {
            value: putSectionProfile.strongAxisPlasticSectionModulus,
            unit: "CubicMeters",
          },
          weakAxisPlasticSectionModulus: {
            value: putSectionProfile.weakAxisPlasticSectionModulus,
            unit: "CubicMeters",
          },
          strongAxisElasticSectionModulus: {
            value: putSectionProfile.strongAxisElasticSectionModulus,
            unit: "CubicMeters",
          },
          weakAxisElasticSectionModulus: {
            value: putSectionProfile.weakAxisElasticSectionModulus,
            unit: "CubicMeters",
          },
          ...(putSectionProfile.discriminator === "WITH_SHEAR_AREAS"
            ? {
                strongAxisShearArea: {
                  value: putSectionProfile.strongAxisShearArea,
                  unit: "SquareMeters",
                },
                weakAxisShearArea: {
                  value: putSectionProfile.weakAxisShearArea,
                  unit: "SquareMeters",
                },
              }
            : {}),
        },
      }),
    );
  }

  for (const deleteSectionProfileId of input.req.body.sectionProfiles.delete) {
    changes.push(
      toEntity({
        entityType: "sectionprofile",
        entityId: deleteSectionProfileId,
        op: "delete",
        payload: { id: deleteSectionProfileId },
      }),
    );
  }

  for (const createElement1d of input.req.body.element1ds.create) {
    const id = Bun.randomUUIDv7();
    changes.push(
      toEntity({
        entityType: "element1d",
        entityId: id,
        op: "insert",
        payload: {
          id,
          revisionId: input.revisionId,
          startNodeId: createElement1d.startNodeId,
          endNodeId: createElement1d.endNodeId,
          materialId: createElement1d.materialId,
          sectionProfileId: createElement1d.sectionProfileId,
        },
      }),
    );
  }

  for (const putElement1d of input.req.body.element1ds.update) {
    const existing = input.currentElement1dsById.get(putElement1d.id);
    if (!existing) {
      throw httpError(`Element1d ${putElement1d.id} not found`, 400);
    }

    changes.push(
      toEntity({
        entityType: "element1d",
        entityId: putElement1d.id,
        op: "update",
        payload: {
          id: putElement1d.id,
          revisionId: input.revisionId,
          startNodeId: putElement1d.startNodeId,
          endNodeId: putElement1d.endNodeId,
          materialId: putElement1d.materialId,
          sectionProfileId: putElement1d.sectionProfileId,
        },
      }),
    );
  }

  for (const deleteElement1dId of input.req.body.element1ds.delete) {
    changes.push(
      toEntity({
        entityType: "element1d",
        entityId: deleteElement1dId,
        op: "delete",
        payload: { id: deleteElement1dId },
      }),
    );
  }

  return changes;
};

export async function createNewRevisionHandler(
  req: {
    params: { modelId: string; branchName: string };
  },
  ctx: AppContext,
  tx: DbTransaction,
) {
  const { modelId, branchName } = req.params;
  const branch = await ctx.services.modelRevisionRepository.getBranchHead(
    modelId,
    branchName,
  );
  if (!branch) {
    throw httpError(
      `Could not find branch ${branchName} on model with ID ${modelId}`,
      404,
    );
  }

  const parentRevision =
    await ctx.services.modelRevisionRepository.getRevisionById(
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
    modelId,
    modelName: parentRevision.name,
    parentRevisionId: parentRevision.id,
    secondParentRevisionId: null,
    authorId: parentRevision.authorId,
    message: "Batch create materials",
    createdAt: now,
  });

  await tx
    .insert(modelBranchHeads)
    .values({
      modelId,
      branchName,
      headRevisionId: revisionId,
      updatedAt: now,
    })
    .onConflictDoUpdate({
      target: [modelBranchHeads.modelId, modelBranchHeads.branchName],
      set: {
        headRevisionId: revisionId,
        updatedAt: now,
      },
    });
  return revisionId;
}

export async function createNewRevisionAggregateHandler(
  req: {
    params: { modelId: string; branchName: string };
  },
  ctx: AppContext,
  message = "Batch create materials",
) {
  const { modelId, branchName } = req.params;
  const branch = await ctx.services.modelRevisionRepository.getBranchHead(
    modelId,
    branchName,
  );
  if (!branch) {
    throw httpError(
      `Could not find branch ${branchName} on model with ID ${modelId}`,
      404,
    );
  }

  const parentRevision =
    await ctx.services.modelRevisionRepository.getRevisionById(
      branch.headRevisionId,
    );
  if (!parentRevision) {
    throw httpError(
      `Could not find parent revision ${branch.headRevisionId} for branch ${branchName}`,
      404,
    );
  }

  return ModelRevisionAggregate.create({
    modelId,
    branchName,
    name: parentRevision.name,
    parentRevisionId: parentRevision.id,
    secondParentRevisionId: null,
    authorId: parentRevision.authorId,
    message,
    createdAt: new Date(),
    nodes: parentRevision.nodes.map((node) => node.toSnapshot()),
    materials: parentRevision.materials.map((material) => material.toSnapshot()),
    sectionProfiles: parentRevision.sectionProfiles.map((sectionProfile) =>
      sectionProfile.toSnapshot(),
    ),
    element1ds: parentRevision.element1ds.map((element1d) =>
      element1d.toSnapshot(),
    ),
  });
}
