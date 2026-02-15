import { defineEndpoint } from "../contracts/endpoint";
import { z } from "zod";
import type { AppContext } from "../common/types";
import { httpError } from "../common/http-utils";
import { getDb } from "../db/client";
import { uuidV7Schema } from "src/common/uuid";
import { revisionNodeResponseSchema } from "./node-response-schema";
import { createNewRevisionAggregateHandler } from "src/model-revisions/create-model-revision";
import { createNodeRequestSchema } from "./create-node-request-schema";
import { Ratio } from "unitsnet-js";
import type { NodeSnapshot } from "./node-entity";
import type { ModelRevisionAggregate } from "src/model-revisions/model-revision-aggregate";

export const batchCreateNodeReqSchema = z.object({
  params: z.object({
    modelId: uuidV7Schema,
    branchName: z.string().trim().min(1),
  }),
  body: z.object({
    nodes: z.array(createNodeRequestSchema).min(1),
  }),
});

export const batchCreateNodeResSchema = z.object({
  nodes: z.array(revisionNodeResponseSchema),
  tempIdToId: z.record(z.string(), uuidV7Schema),
});

const toResponseNode = (input: {
  id: string;
  modelId: string;
  nodeTypeDescriminator: "external" | "internal";
}) => ({
  id: input.id,
  modelId: input.modelId,
  nodeTypeDescriminator: input.nodeTypeDescriminator,
});

export const batchCreateNode = defineEndpoint({
  method: "POST",
  path: "/api/models/:modelId/branches/:branchName/nodes/batch",
  req: batchCreateNodeReqSchema,
  res: batchCreateNodeResSchema,
  async handler(req, ctx: AppContext) {
    const revision = await createNewRevisionAggregateHandler(req, ctx);

    const seenTempIds = new Set<string>();
    return await batchCreateNodeHandler(req, seenTempIds, ctx, revision);
  },
});

export async function batchCreateNodeHandler(
  req: z.infer<typeof batchCreateNodeReqSchema>,
  seenTempIds: Set<string>,
  ctx: AppContext,
  revision: ModelRevisionAggregate,
) {
  for (const node of req.body.nodes) {
    if (!node.tempId) {
      continue;
    }

    if (seenTempIds.has(node.tempId)) {
      throw httpError(`Duplicate tempId "${node.tempId}"`, 400);
    }

    seenTempIds.add(node.tempId);
  }

  const tempIdToId: Record<string, string> = {};

  const nodes = req.body.nodes.map((node) => {
    const id = Bun.randomUUIDv7();

    if (node.tempId) {
      tempIdToId[node.tempId] = id;
    }

    let snapshot: NodeSnapshot;
    if (node.location.type === "internal") {
      snapshot = {
        id,
        modelRevisionId: revision.id,
        nodeType: "internalNode",
        nodeTypeDescriminator: "internal",
        element1dId: node.location.element1dId,
        distanceAlongElement1d: Ratio.FromDecimalFractions(
          node.location.ratioAlongElement1d,
        ),
        restraint: node.restraint,
      };
    } else {
      snapshot = {
        id,
        modelRevisionId: revision.id,
        nodeType: "spatialNode",
        nodeTypeDescriminator: "external",
        point: node.location.point,
        restraint: node.restraint,
      };
    }

    revision.addNode(snapshot);
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
    nodes: nodes.map((node) =>
      toResponseNode({
        id: node.id,
        modelId: req.params.modelId,
        nodeTypeDescriminator: node.nodeTypeDescriminator!,
      }),
    ),
    tempIdToId,
  };
}
