import { modelRevisionDrafts, revisionChanges } from "../db/schema";
import {
  ModelRevisionDraftAggregate,
  type ModelRevisionDraftSnapshot,
} from "./model-revision-draft-aggregate";
import type { NodeSnapshot } from "../nodes/node-entity";

export const modelRevisionDraftMapper = {
  toDomain(
    row: typeof modelRevisionDrafts.$inferSelect,
    changeRows: (typeof revisionChanges.$inferSelect)[],
  ): ModelRevisionDraftAggregate {
    return ModelRevisionDraftAggregate.rehydrate({
      id: row.id,
      modelId: row.modelId,
      name: row.modelName,
      parentRevisionId: row.parentRevisionId,
      secondParentRevisionId: row.secondParentRevisionId,
      authorId: row.authorId,
      message: row.message,
      createdAt: row.createdAt,
      updatedAt: row.updatedAt,
      nodes: changeRows
        .filter((change) => change.entityType === "node")
        .filter((change) => change.op !== "delete")
        .map((change) => ({
          id: change.entityId,
          modelRevisionId: row.id,
          nodeType: extractNodeType(change.payload),
          nodeTypeDescriminator: extractNodeTypeDescriminator(change.payload),
        })),
    });
  },

  toPersistence(
    aggregate: ModelRevisionDraftAggregate,
  ): typeof modelRevisionDrafts.$inferInsert {
    const snapshot = aggregate.toSnapshot();
    return {
      id: snapshot.id,
      modelId: snapshot.modelId,
      modelName: snapshot.name,
      parentRevisionId: snapshot.parentRevisionId,
      secondParentRevisionId: snapshot.secondParentRevisionId,
      authorId: snapshot.authorId,
      message: snapshot.message,
      createdAt: snapshot.createdAt,
      updatedAt: snapshot.updatedAt,
    };
  },

  fromDraftInput(input: {
    id: string;
    modelId: string;
    name: string;
    parentRevisionId: string | null;
    secondParentRevisionId: string | null;
    authorId: string;
    message: string;
    nodes: {
      id: string;
      modelRevisionId: string;
      nodeType?: "spatialNode" | "internalNode";
      nodeTypeDescriminator: "external" | "internal";
    }[];
  }): ModelRevisionDraftAggregate {
    const now = new Date();
    const snapshot: ModelRevisionDraftSnapshot = {
      id: input.id,
      modelId: input.modelId,
      name: input.name,
      parentRevisionId: input.parentRevisionId,
      secondParentRevisionId: input.secondParentRevisionId,
      authorId: input.authorId,
      message: input.message,
      createdAt: now,
      updatedAt: now,
      nodes: input.nodes.map((node) => ({
        id: node.id,
        modelRevisionId: node.modelRevisionId,
        nodeType: node.nodeType ?? "spatialNode",
        nodeTypeDescriminator: node.nodeTypeDescriminator,
      })),
    };

    return ModelRevisionDraftAggregate.create(snapshot);
  },
};

const extractNodeTypeDescriminator = (
  payload: unknown,
): "external" | "internal" => {
  if (payload && typeof payload === "object") {
    const nodeTypeDescriminator = (
      payload as { nodeTypeDescriminator?: unknown }
    ).nodeTypeDescriminator;
    if (nodeTypeDescriminator === "external") {
      return "external";
    }
  }
  return "internal";
};

const extractNodeType = (
  payload: unknown,
): NodeSnapshot["nodeType"] => {
  if (payload && typeof payload === "object") {
    const nodeType = (payload as { nodeType?: unknown }).nodeType;
    if (nodeType === "internalNode" || nodeType === "spatialNode") {
      return nodeType;
    }
  }
  return "spatialNode";
};
