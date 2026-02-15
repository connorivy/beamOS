import { defineEndpoint } from "../contracts/endpoint";
import { Pressure, PressureUnits } from "unitsnet-js";
import { z } from "zod";
import type { AppContext } from "../common/types";
import { httpError } from "../common/http-utils";
import { getDb } from "../db/client";
import type { MaterialSnapshot } from "./material-entity";
import { uuidV7Schema } from "src/common/uuid";
import {
  materialResponseSchema,
  createMaterialRequestSchema,
} from "./material-contract-schemas";
import { ModelRevisionAggregate } from "src/model-revisions/model-revision-aggregate";
import { createNewRevisionAggregateHandler } from "src/model-revisions/create-model-revision";

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

const toResponseMaterial = (material: MaterialSnapshot) => ({
  id: material.id,
  revisionId: material.revisionId,
  name: material.name,
  modulusOfElasticity: material.pressureE.Pascals,
  modulusOfRigidity: material.pressureG.Pascals,
  units: {
    pressure: PressureUnits.Pascals as const,
  },
});

export const batchCreateMaterial = defineEndpoint({
  method: "POST",
  path: "/api/models/:modelId/branches/:branchName/materials/batch",
  req: batchCreateMaterialReqSchema,
  res: batchCreateMaterialResSchema,
  async handler(req, ctx: AppContext) {
    const revision = await createNewRevisionAggregateHandler(req, ctx);

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

  const materials = req.body.materials.map((material) => {
    const id = Bun.randomUUIDv7();
    if (material.tempId) {
      tempIdToId[material.tempId] = id;
    }

    const snapshot: MaterialSnapshot = {
      id,
      revisionId: revision.id,
      name: material.name,
      pressureE: new Pressure(
        material.modulusOfElasticity,
        material.units.pressure,
      ),
      pressureG: new Pressure(
        material.modulusOfRigidity,
        material.units.pressure,
      ),
    };
    revision.addMaterial(snapshot);
    return snapshot;
  });

  await getDb().transaction(async (tx) => {
    await ctx.services.modelRevisionRepository.save({
      revision,
      newRevision: true,
      tx,
    });
  });

  return {
    materials: materials.map((material) => toResponseMaterial(material)),
    tempIdToId,
  };
}
