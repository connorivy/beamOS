import { defineEndpoint } from "../contracts/endpoint";
import { PressureUnits } from "unitsnet-js";
import { z } from "zod";
import type { AppContext } from "../common/types";
import { httpError } from "../common/http-utils";
import { getDb } from "../db/client";
import type { MaterialEntity } from "./material-entity";
import { uuidV7Schema } from "src/common/uuid";
import {
  createMaterialRequestSchema,
  materialResponseArraySchema,
} from "./material-contract-schemas";
import { ModelRevisionAggregate } from "src/model-revisions/model-revision-aggregate";
import { createNewRevisionAggregateHandler } from "src/model-revisions/create-model-revision";

export const batchCreateMaterialReqSchema = z
  .object({
    params: z.object({
      projectId: uuidV7Schema,
      branchName: z.string().trim().min(1),
    }),
    body: z
      .object({
        materials: z.array(createMaterialRequestSchema).min(1),
      })
      .meta({ id: "BatchCreateMaterialRequest" }),
  })
  .meta({ id: "BatchCreateMaterialEndpointRequest" });

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

export const batchCreateMaterial = defineEndpoint({
  method: "POST",
  path: "/api/projects/:projectId/branches/:branchName/materials/batch",
  req: batchCreateMaterialReqSchema,
  res: materialResponseArraySchema,
  async handler(req, ctx: AppContext) {
    const revision = await createNewRevisionAggregateHandler(req, ctx);
    return await batchCreateMaterialHandler(req, ctx, revision);
  },
});

export async function batchCreateMaterialHandler(
  req: z.infer<typeof batchCreateMaterialReqSchema>,
  ctx: AppContext,
  revision: ModelRevisionAggregate,
) {
  const existingMaterialNames = new Set(
    [...revision.materials.values()].map((material) => material.name),
  );

  const materials = req.body.materials.map((material) => {
    if (existingMaterialNames.has(material.name)) {
      throw httpError(`Duplicate material name "${material.name}"`, 400);
    }
    existingMaterialNames.add(material.name);

    const id = Bun.randomUUIDv7();
    revision.addMaterial({ id, ...material });
    return revision.materials.get(id)!;
  });

  await getDb().transaction(async (tx) => {
    await ctx.services.modelRevisionRepository.save({
      revision,
      branchName: req.params.branchName,
      tx,
    });
  });

  return materials.map((material) => toResponseMaterial(material));
}
