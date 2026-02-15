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

export const batchPutMaterialReqSchema = z.object({
  params: z.object({
    modelId: uuidV7Schema,
    branchName: z.string().trim().min(1),
  }),
  body: z.object({
    materials: z.array(putMaterialRequestSchema).min(1),
  }),
});

export const batchPutMaterialResSchema = z.object({
  materials: z.array(materialResponseSchema),
});

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
  path: "/api/models/:modelId/branches/:branchName/materials/batch",
  req: batchPutMaterialReqSchema,
  res: batchPutMaterialResSchema,
  async handler(req, ctx: AppContext) {
    return await getDb().transaction(async (tx) => {
      const revisionId = await createNewRevisionHandler(req, ctx, tx);
      return await batchPutMaterialHandler(req, ctx, tx, revisionId);
    });
  },
});

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
