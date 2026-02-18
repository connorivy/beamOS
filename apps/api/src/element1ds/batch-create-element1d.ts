import { defineEndpoint } from "../contracts/endpoint";
import { z } from "zod";
import type { AppContext } from "../common/types";
import { httpError } from "../common/http-utils";
import { getDb } from "../db/client";
import type { Element1dSnapshot } from "./element1d-entity";
import { element1dResponseSchema } from "./element1d-contract-schemas";
import { createElement1dRequestSchema } from "./element1d-contract-schemas";
import { uuidV7Schema } from "src/common/uuid";
import { createNewRevisionAggregateHandler } from "src/model-revisions/create-model-revision";
import { ModelRevisionAggregate } from "src/model-revisions/model-revision-aggregate";

export const batchCreateElement1dReqSchema = z
  .object({
    params: z.object({
      modelId: uuidV7Schema,
      branchName: z.string().trim().min(1),
    }),
    body: z
      .object({
        element1ds: z.array(createElement1dRequestSchema).min(1),
      })
      .meta({ id: "BatchCreateElement1dRequest" }),
  })
  .meta({ id: "BatchCreateElement1dEndpointRequest" });

export const batchCreateElement1dResSchema = z
  .object({
    element1ds: z.array(element1dResponseSchema),
    tempIdToId: z.record(z.string(), uuidV7Schema),
  })
  .meta({ id: "BatchCreateElement1dResponse" });

const toResponseElement1d = (element1d: Element1dSnapshot) => ({
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
    const revision = await createNewRevisionAggregateHandler(req, ctx);

    const seenTempIds = new Set<string>();
    return await batchCreateElement1dHandler(req, seenTempIds, ctx, revision);
  },
});

export async function batchCreateElement1dHandler(
  req: z.infer<typeof batchCreateElement1dReqSchema>,
  seenTempIds: Set<string>,
  ctx: AppContext,
  revision: ModelRevisionAggregate,
) {
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

  const element1ds = req.body.element1ds.map((element1d) => {
    const id = Bun.randomUUIDv7();
    if (element1d.tempId) {
      tempIdToId[element1d.tempId] = id;
    }

    const snapshot: Element1dSnapshot = {
      id,
      revisionId: revision.id,
      startNodeId: element1d.startNodeId,
      endNodeId: element1d.endNodeId,
      materialId: element1d.materialId,
      sectionProfileId: element1d.sectionProfileId,
    };
    revision.addElement1d(snapshot);
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
    element1ds: element1ds.map((element1d) => toResponseElement1d(element1d)),
    tempIdToId,
  };
}
