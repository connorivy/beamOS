import { defineEndpoint } from "../contracts/endpoint";
import { modelRevisionResponseSchema } from "./model-revision-response-schema";
import { createModelRevisionReqSchema } from "./create-model-revision-request-schema";
import { AppContext } from "src/common/types";
import { httpError } from "src/common/http-utils";
import { DbTransaction, getDb } from "src/db/client";
import { modelBranchHeads, modelRevisions } from "src/db/schema";
import { RevisionChangeEntity } from "src/revision-changes/revision-change-entity";
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
import type { SectionProfileSnapshot } from "src/section-profiles/section-profile-aggregate";
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
      draftId: null,
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

  for (const updateNode of input.req.body.nodes.update) {
    const existing = input.currentNodesById.get(updateNode.id);
    if (!existing) {
      throw httpError(`Node ${updateNode.id} not found`, 400);
    }

    const nodeTypeDescriminator =
      updateNode.nodeTypeDescriminator ?? existing.nodeTypeDescriminator ?? "internal";

    changes.push(
      toEntity({
        entityType: "node",
        entityId: updateNode.id,
        op: "update",
        payload: {
          id: updateNode.id,
          modelRevisionId: input.revisionId,
          nodeType:
            nodeTypeDescriminator === "external"
              ? "spatialNode"
              : "internalNode",
          nodeTypeDescriminator,
          point:
            nodeTypeDescriminator === "external"
              ? (existing.point ?? { x: 0, y: 0, z: 0 })
              : null,
          element1dId:
            nodeTypeDescriminator === "internal"
              ? (existing.element1dId ?? updateNode.id)
              : null,
          distanceAlongElement1d:
            nodeTypeDescriminator === "internal"
              ? (existing.distanceAlongElement1d?.DecimalFractions ?? 0)
              : null,
          restraint: existing.restraint ?? {},
        },
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
          pressureE: {
            value: new Pressure(
              createMaterial.pressureE.value,
              createMaterial.pressureE.unit,
            ).Pascals,
            unit: "Pascals",
          },
          pressureG: {
            value: new Pressure(
              createMaterial.pressureG.value,
              createMaterial.pressureG.unit,
            ).Pascals,
            unit: "Pascals",
          },
          ...(createMaterial.name ? { name: createMaterial.name } : {}),
        },
      }),
    );
  }

  for (const updateMaterial of input.req.body.materials.update) {
    const existing = input.currentMaterialsById.get(updateMaterial.id);
    if (!existing) {
      throw httpError(`Material ${updateMaterial.id} not found`, 400);
    }

    changes.push(
      toEntity({
        entityType: "material",
        entityId: updateMaterial.id,
        op: "update",
        payload: {
          id: updateMaterial.id,
          revisionId: input.revisionId,
          pressureE: {
            value: existing.pressureE.Pascals,
            unit: "Pascals",
          },
          pressureG: {
            value: existing.pressureG.Pascals,
            unit: "Pascals",
          },
          ...(updateMaterial.name ? { name: updateMaterial.name } : {}),
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
    const id = createSectionProfile.id;
    changes.push(
      toEntity({
        entityType: "sectionprofile",
        entityId: id,
        op: "insert",
        payload: {
          id,
          revisionId: input.revisionId,
          name: createSectionProfile.name,
        },
      }),
    );
  }

  for (const updateSectionProfile of input.req.body.sectionProfiles.update) {
    const existing = input.currentSectionProfilesById.get(updateSectionProfile.id);
    if (!existing) {
      throw httpError(`Section profile ${updateSectionProfile.id} not found`, 400);
    }

    changes.push(
      toEntity({
        entityType: "sectionprofile",
        entityId: updateSectionProfile.id,
        op: "update",
        payload: {
          id: updateSectionProfile.id,
          revisionId: input.revisionId,
          name: updateSectionProfile.name ?? existing.name,
          discriminator: existing.discriminator,
          area: {
            value: existing.area.SquareMeters,
            unit: "SquareMeters",
          },
          strongAxisMomentOfInertia: {
            value: existing.strongAxisMomentOfInertia.MetersToTheFourth,
            unit: "MetersToTheFourth",
          },
          weakAxisMomentOfInertia: {
            value: existing.weakAxisMomentOfInertia.MetersToTheFourth,
            unit: "MetersToTheFourth",
          },
          torsionalConstant: {
            value: existing.torsionalConstant.MetersToTheFourth,
            unit: "MetersToTheFourth",
          },
          warpingConstant: {
            value: existing.warpingConstant.MetersToTheSixth,
            unit: "MetersToTheSixth",
          },
          strongAxisPlasticSectionModulus: {
            value: existing.strongAxisPlasticSectionModulus.CubicMeters,
            unit: "CubicMeters",
          },
          weakAxisPlasticSectionModulus: {
            value: existing.weakAxisPlasticSectionModulus.CubicMeters,
            unit: "CubicMeters",
          },
          strongAxisElasticSectionModulus: {
            value: existing.strongAxisElasticSectionModulus.CubicMeters,
            unit: "CubicMeters",
          },
          weakAxisElasticSectionModulus: {
            value: existing.weakAxisElasticSectionModulus.CubicMeters,
            unit: "CubicMeters",
          },
          ...(existing.strongAxisShearArea
            ? {
                strongAxisShearArea: {
                  value: existing.strongAxisShearArea.SquareMeters,
                  unit: "SquareMeters",
                },
              }
            : {}),
          ...(existing.weakAxisShearArea
            ? {
                weakAxisShearArea: {
                  value: existing.weakAxisShearArea.SquareMeters,
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

  for (const updateElement1d of input.req.body.element1ds.update) {
    const existing = input.currentElement1dsById.get(updateElement1d.id);
    if (!existing) {
      throw httpError(`Element1d ${updateElement1d.id} not found`, 400);
    }

    changes.push(
      toEntity({
        entityType: "element1d",
        entityId: updateElement1d.id,
        op: "update",
        payload: {
          id: updateElement1d.id,
          revisionId: input.revisionId,
          startNodeId: updateElement1d.startNode ?? existing.startNodeId,
          endNodeId: updateElement1d.endNode ?? existing.endNodeId,
          materialId: updateElement1d.material ?? existing.materialId,
          sectionProfileId: updateElement1d.section ?? existing.sectionProfileId,
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
