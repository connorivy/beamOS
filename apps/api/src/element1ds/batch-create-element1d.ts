import { defineEndpoint } from "@beamos/contracts";
import { z } from "zod";
import type { AppContext } from "../common/types";
import { httpError } from "../common/http-utils";
import { DbTransaction, getDb } from "../db/client";
import { Element1dEntity } from "./element1d-entity";
import { element1dResponseSchema } from "./element1d-response-schema";
import { createElement1dRequestSchema } from "./create-element1d-request-schema";
import { uuidV7Schema } from "src/common/uuid";

export const batchCreateElement1dReqSchema = z.object({
  params: z.object({
    modelId: uuidV7Schema,
    branchName: z.string().trim().min(1),
  }),
  body: z.object({
    element1ds: z.array(createElement1dRequestSchema).min(1),
  }),
});

export const batchCreateElement1dResSchema = z.object({
  element1ds: z.array(element1dResponseSchema),
  tempIdToId: z.record(z.string(), uuidV7Schema),
});

const toResponseElement1d = (element1d: Element1dEntity) => ({
  id: element1d.id,
  revisionId: element1d.revisionId,
  startNodeId: element1d.startNodeId,
  endNodeId: element1d.endNodeId,
  materialId: element1d.materialId,
  sectionProfileId: element1d.sectionProfileId,
});

export const batchCreateElement1d = defineEndpoint({
  method: "POST",
  path: "/api/models/:modelId/branches/:branchName/element1ds/batch",
  req: batchCreateElement1dReqSchema,
  res: batchCreateElement1dResSchema,
  async handler(req, ctx: AppContext) {
    return await getDb().transaction(async (tx) => {
      return await batchCreateElement1dHandler(req, ctx, tx);
    });
  },
});

async function batchCreateElement1dHandler(
  req: z.infer<typeof batchCreateElement1dReqSchema>,
  ctx: AppContext,
  tx: DbTransaction,
) {
  const seenTempIds = new Set<string>();

  for (const element1d of req.body.element1ds) {
    if (!element1d.tempId) {
      continue;
    }

    if (seenTempIds.has(element1d.tempId)) {
      throw httpError(`Duplicate tempId "${element1d.tempId}"`, 400);
    }

    seenTempIds.add(element1d.tempId);
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

  const entities = req.body.element1ds.map((element1d) => {
    const id = Bun.randomUUIDv7();

    if (element1d.tempId) {
      tempIdToId[element1d.tempId] = id;
    }

    return Element1dEntity.create({
      id,
      revisionId: branch.headRevisionId,
      startNodeId: element1d.startNodeId,
      endNodeId: element1d.endNodeId,
      materialId: element1d.materialId,
      sectionProfileId: element1d.sectionProfileId,
    });
  });

  const saved = await ctx.services.element1dRepository.batchCreate(
    tx,
    entities,
  );

  return {
    element1ds: saved.map((element1d) => toResponseElement1d(element1d)),
    tempIdToId,
  };
}
