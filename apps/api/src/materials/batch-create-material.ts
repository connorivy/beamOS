import { defineEndpoint } from "../contracts/endpoint";
import { Pressure, PressureUnits } from "unitsnet-js";
import { z } from "zod";
import type { AppContext } from "../common/types";
import { httpError } from "../common/http-utils";
import { getDb } from "../db/client";
import { MaterialEntity } from "./material-entity";
import { modelBranchHeads } from "src/db/schema";
import { uuidV7Schema } from "src/common/uuid";
import { materialResponseSchema } from "./material-response-schema";
import { createMaterialRequestSchema } from "./create-material-request-schema";
import { ModelRevisionAggregate } from "src/model-revisions/model-revision-aggregate";

export const batchCreateMaterialReqSchema = z.object({
  params: z.object({
    modelId: uuidV7Schema,
    branchName: z.string().trim().min(1),
  }),
  body: z.object({
    materials: z.array(createMaterialRequestSchema).min(1),
  }),
});

export const batchCreateMaterialResSchema = z.object({
  materials: z.array(materialResponseSchema),
  tempIdToId: z.record(z.string(), uuidV7Schema),
});

const toResponseMaterial = (material: MaterialEntity) => ({
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
});

export const batchCreateMaterial = defineEndpoint({
  method: "POST",
  path: "/api/models/:modelId/branches/:branchName/materials/batch",
  req: batchCreateMaterialReqSchema,
  res: batchCreateMaterialResSchema,
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

    const revision = ModelRevisionAggregate.create({
      id: Bun.randomUUIDv7(),
      modelId: req.params.modelId,
      name: parentRevision.name,
      parentRevisionId: parentRevision.id,
      secondParentRevisionId: null,
      authorId: parentRevision.authorId,
      message: "Batch create materials",
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

    const seenTempIds = new Set<string>();
    return await batchCreateMaterialHandler(req, seenTempIds, ctx, revision);
  },
});

export async function batchCreateMaterialHandler(
  req: z.infer<typeof batchCreateMaterialReqSchema>,
  seenTempIds: Set<string>,
  ctx: AppContext,
  revision: ModelRevisionAggregate,
) {
  for (const material of req.body.materials) {
    if (!material.tempId) {
      continue;
    }
    if (seenTempIds.has(material.tempId)) {
      throw httpError(`Duplicate tempId "${material.tempId}"`, 400);
    }
    seenTempIds.add(material.tempId);
  }

  const tempIdToId: Record<string, string> = {};

  const entities = req.body.materials.map((material) => {
    const id = Bun.randomUUIDv7();
    if (material.tempId) {
      tempIdToId[material.tempId] = id;
    }

    const entity = MaterialEntity.create({
      id,
      revisionId: revision.id,
      pressureE: new Pressure(
        material.pressureE.value,
        material.pressureE.unit,
      ),
      pressureG: new Pressure(
        material.pressureG.value,
        material.pressureG.unit,
      ),
    });
    revision.addMaterial(entity.toSnapshot());
    return entity;
  });

  await getDb().transaction(async (tx) => {
    await ctx.services.modelRevisionRepository.save({
      revision,
      newRevision: true,
      tx,
    });
    await tx
      .insert(modelBranchHeads)
      .values({
        modelId: req.params.modelId,
        branchName: req.params.branchName,
        headRevisionId: revision.id,
        updatedAt: revision.createdAt,
      })
      .onConflictDoUpdate({
        target: [modelBranchHeads.modelId, modelBranchHeads.branchName],
        set: {
          headRevisionId: revision.id,
          updatedAt: revision.createdAt,
        },
      });
  });

  return {
    materials: entities.map((material) => toResponseMaterial(material)),
    tempIdToId,
  };
}
