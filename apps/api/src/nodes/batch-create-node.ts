import { defineEndpoint } from "../contracts/endpoint";
import { z } from "zod";
import type { AppContext } from "../common/types";
import { httpError } from "../common/http-utils";
import { getDb, type DbTransaction } from "../db/client";
import { revisionChanges } from "../db/schema";
import { uuidV7Schema } from "src/common/uuid";
import { revisionNodeResponseSchema } from "./node-response-schema";
import { createNewRevisionHandler } from "src/model-revisions/create-model-revision";
import { createNodeRequestSchema } from "./create-node-request-schema";
import { NodeEntity } from "./node-entity";
import { Ratio } from "unitsnet-js";
import { RevisionChangeEntity } from "../revision-changes/revision-change-entity";
import { revisionChangeMapper } from "../revision-changes/revision-change-mapper";

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
    return await getDb().transaction(async (tx) => {
      const revisionId = await createNewRevisionHandler(req, ctx, tx);
      return await batchCreateNodeHandler(req, tx, revisionId);
    });
  },
});

async function batchCreateNodeHandler(
  req: z.infer<typeof batchCreateNodeReqSchema>,
  tx: DbTransaction,
  revisionId: string,
) {
  const seenTempIds = new Set<string>();

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
  const entities = req.body.nodes.map((node) => {
    const id = Bun.randomUUIDv7();

    if (node.tempId) {
      tempIdToId[node.tempId] = id;
    }

    if (node.location.type === "internal") {
      return NodeEntity.create({
        id,
        modelRevisionId: revisionId,
        nodeType: "internalNode",
        nodeTypeDescriminator: "internal",
        element1dId: node.location.element1dId,
        distanceAlongElement1d: node.location.ratioAlongElement1d,
        restraint: node.restraint,
      });
    }

    return NodeEntity.create({
      id,
      modelRevisionId: revisionId,
      nodeType: "spatialNode",
      nodeTypeDescriminator: "external",
      point: node.location.point,
      restraint: node.restraint,
    });
  });

  const now = new Date();
  const changeRows = entities.flatMap((node) =>
    node.pullDomainEvents().map((event) => {
      const snapshot = event.payload;
      return revisionChangeMapper.toPersistence(
        RevisionChangeEntity.create({
          id: Bun.randomUUIDv7(),
          revisionId,
          draftId: null,
          entityType: "node",
          entityId: snapshot.id,
          schemaVersion: 1,
          op: "insert",
          payload: {
            id: snapshot.id,
            modelRevisionId: snapshot.modelRevisionId,
            nodeType: snapshot.nodeType,
            nodeTypeDescriminator: snapshot.nodeTypeDescriminator,
            point: snapshot.point ?? null,
            element1dId: snapshot.element1dId ?? null,
            distanceAlongElement1d:
              snapshot.distanceAlongElement1d instanceof Ratio
                ? snapshot.distanceAlongElement1d.DecimalFractions
                : null,
            restraint: snapshot.restraint ?? {},
          },
          createdAt: now,
        }),
      );
    }),
  );

  if (changeRows.length > 0) {
    await tx.insert(revisionChanges).values(changeRows).onConflictDoNothing();
  }

  return {
    nodes: entities.map((node) =>
      toResponseNode({
        id: node.id,
        modelId: req.params.modelId,
        nodeTypeDescriminator: node.nodeTypeDescriminator,
      }),
    ),
    tempIdToId,
  };
}
