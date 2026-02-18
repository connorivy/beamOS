import { defineEndpoint } from "../contracts/endpoint";
import { Pressure, PressureUnits } from "unitsnet-js";
import { z } from "zod";
import type { AppContext } from "../common/types";
import { httpError } from "../common/http-utils";
import { getDb } from "../db/client";
import type { DbTransaction } from "../db/client";
import { MaterialEntity } from "./material-entity";
import { uuidV7Schema } from "src/common/uuid";
import {
  materialResponseSchema,
  putMaterialRequestSchema,
} from "./material-contract-schemas";
import { createNewRevisionHandler } from "src/model-revisions/create-model-revision";

export const batchPutMaterialReqSchema = z
  .object({
    params: z.object({
      projectId: uuidV7Schema,
      branchName: z.string().trim().min(1),
    }),
    body: z
      .object({
        materials: z.array(putMaterialRequestSchema).min(1),
      })
      .meta({ id: "BatchPutMaterialRequest" }),
  })
  .meta({ id: "BatchPutMaterialEndpointRequest" });

export const batchPutMaterialResSchema = z
  .object({
    materials: z.array(materialResponseSchema),
  })
  .meta({ id: "BatchPutMaterialResponse" });

const toResponseMaterial = (material: MaterialEntity) => ({
  id: material.id,
  revisionId: material.revisionId,
  name: material.name,
  modulusOfElasticity: material.pressureE.Pascals,
  modulusOfRigidity: material.pressureG.Pascals,
  units: {
    pressure: PressureUnits.Pascals as const,
  },
});

export const batchPutMaterial = defineEndpoint({
  method: "PUT",
  path: "/api/projects/:projectId/branches/:branchName/materials/batch",
  req: batchPutMaterialReqSchema,
  res: batchPutMaterialResSchema,
  async handler(req, ctx: AppContext) {
    const branch = await ctx.services.modelRevisionRepository.getBranchHead(
      req.params.projectId,
      req.params.branchName,
    );
    if (!branch) {
      throw httpError(
        `Could not find branch ${req.params.branchName} on model with ID ${req.params.projectId}`,
        404,
      );
    }
    const parentRevision =
      await ctx.services.modelRevisionRepository.getRevisionById(
        branch.headRevisionId,
      );
    if (!parentRevision) {
      throw httpError(
        `Could not find parent revision ${branch.headRevisionId} for branch ${req.params.branchName}`,
        404,
      );
    }
    assertMaterialNamesAreUnique(req.body.materials, parentRevision.materials);

    return await getDb().transaction(async (tx) => {
      const revisionId = await createNewRevisionHandler(req, ctx, tx);
      return await batchPutMaterialHandler(req, ctx, tx, revisionId);
    });
  },
});

function assertMaterialNamesAreUnique(
  updates: z.infer<typeof putMaterialRequestSchema>[],
  existingMaterials: readonly MaterialEntity[],
) {
  const nameById = new Map(
    existingMaterials.map((material) => [material.id, material.name]),
  );
  const remainingNames = new Set(nameById.values());

  for (const update of updates) {
    const currentName = nameById.get(update.id);
    if (currentName) {
      remainingNames.delete(currentName);
    }
  }

  for (const update of updates) {
    if (remainingNames.has(update.name)) {
      throw httpError(`Duplicate material name "${update.name}"`, 400);
    }
    remainingNames.add(update.name);
  }
}

export async function batchPutMaterialHandler(
  req: z.infer<typeof batchPutMaterialReqSchema>,
  ctx: AppContext,
  tx: DbTransaction,
  revisionId: string,
) {
  const seenIds = new Set<string>();
  for (const material of req.body.materials) {
    if (seenIds.has(material.id)) {
      throw httpError(`Duplicate material id "${material.id}"`, 400);
    }
    seenIds.add(material.id);
  }

  const entities = req.body.materials.map((material) =>
    MaterialEntity.create({
      id: material.id,
      revisionId,
      name: material.name,
      pressureE: new Pressure(
        material.modulusOfElasticity,
        material.units.pressure,
      ),
      pressureG: new Pressure(
        material.modulusOfRigidity,
        material.units.pressure,
      ),
    }),
  );

  const saved = await ctx.services.materialRepository.batchCreate(tx, entities);
  return {
    materials: saved.map((material) => toResponseMaterial(material)),
  };
}
