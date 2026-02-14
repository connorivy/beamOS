import { defineEndpoint } from "@beamos/contracts";
import { Pressure, PressureUnits } from "unitsnet-js";
import { z } from "zod";
import type { AppContext } from "../common/types";
import { httpError } from "../common/http-utils";
import { getDb } from "../db/client";
import type { DbTransaction } from "../db/client";
import { MaterialEntity } from "./material-entity";
import { uuidV7Schema } from "src/common/uuid";
import { materialResponseSchema } from "./material-response-schema";
import { createMaterialRequestSchema } from "./create-material-request-schema";

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
    const seenTempIds = new Set<string>();
    return await getDb().transaction(async (tx) => {
      return await batchCreateMaterialHandler(req, seenTempIds, ctx, tx);
    });
  },
});

export async function batchCreateMaterialHandler(
  req: z.infer<typeof batchCreateMaterialReqSchema>,
  seenTempIds: Set<string>,
  ctx: AppContext,
  tx: DbTransaction,
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
  const entities = req.body.materials.map((material) => {
    const id = Bun.randomUUIDv7();
    if (material.tempId) {
      tempIdToId[material.tempId] = id;
    }

    return MaterialEntity.create({
      id,
      revisionId: branch.headRevisionId,
      pressureE: new Pressure(
        material.pressureE.value,
        material.pressureE.unit,
      ),
      pressureG: new Pressure(
        material.pressureG.value,
        material.pressureG.unit,
      ),
    });
  });

  const saved = await ctx.services.materialRepository.batchCreate(tx, entities);
  return {
    materials: saved.map((material) => toResponseMaterial(material)),
    tempIdToId,
  };
}
