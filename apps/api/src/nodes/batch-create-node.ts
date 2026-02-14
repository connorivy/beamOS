import { defineEndpoint } from "@beamos/contracts";
import { z } from "zod";
import type { AppContext } from "../common/types";
import { httpError } from "../common/http-utils";
import { getDb, type DbTransaction } from "../db/client";
import { nodes, revisionChanges } from "../db/schema";
import { uuidV7Schema } from "src/common/uuid";
import { revisionNodeResponseSchema } from "./node-response-schema";
import { createNewRevisionHandler } from "src/model-revisions/create-model-revision";
import { createNodeRequestSchema } from "./create-node-request-schema";

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
  const createdNodes = req.body.nodes.map((node) => {
    const id = Bun.randomUUIDv7();
    const nodeTypeDescriminator =
      node.location.type === "spatial" ? "external" : "internal";

    if (node.tempId) {
      tempIdToId[node.tempId] = id;
    }

    return {
      id,
      modelId: req.params.modelId,
      nodeTypeDescriminator: nodeTypeDescriminator as "external" | "internal",
      locationDiscriminator: node.location.type,
      pointX: node.location.type === "spatial" ? node.location.point.x : null,
      pointY: node.location.type === "spatial" ? node.location.point.y : null,
      pointZ: node.location.type === "spatial" ? node.location.point.z : null,
      element1dId:
        node.location.type === "internal" ? node.location.element1dId : null,
      ratioAlongElement1d:
        node.location.type === "internal"
          ? node.location.ratioAlongElement1d.DecimalFractions
          : null,
      restraint: node.restraint,
    };
  });

  await tx.insert(nodes).values(
    createdNodes.map((node) => ({
      id: node.id,
      revisionId,
      locationDiscriminator: node.locationDiscriminator,
      pointX: node.pointX,
      pointY: node.pointY,
      pointZ: node.pointZ,
      element1dId: node.element1dId,
      ratioAlongElement1d: node.ratioAlongElement1d,
      restraint: node.restraint,
    })),
  );

  await tx.insert(revisionChanges).values(
    createdNodes.map((node) => ({
      id: Bun.randomUUIDv7(),
      revisionId,
      draftId: null,
      entityType: "node",
      entityId: node.id,
      schemaVersion: 1,
      op: "insert",
      payload: {
        id: node.id,
        modelId: node.modelId,
        nodeTypeDescriminator: node.nodeTypeDescriminator,
        locationDiscriminator: node.locationDiscriminator,
        point:
          node.locationDiscriminator === "spatial"
            ? {
                x: node.pointX,
                y: node.pointY,
                z: node.pointZ,
              }
            : null,
        element1dId: node.element1dId,
        ratioAlongElement1d: node.ratioAlongElement1d,
        restraint: node.restraint,
      },
      createdAt: new Date(),
    })),
  );

  return {
    nodes: createdNodes.map((node) => toResponseNode(node)),
    tempIdToId,
  };
}
