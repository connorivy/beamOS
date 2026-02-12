import { modelRevisionDrafts, revisionChanges } from "../db/schema";
import {
  ModelRevisionDraftAggregate,
  type ModelRevisionDraftSnapshot,
} from "./model-revision-draft-aggregate";

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
        .map((change) => ({
          draftId: row.id,
          nodeId: change.entityId,
          name: extractNodeName(change.payload),
          op: change.op === "delete" ? "delete" : "upsert",
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
    nodes: { nodeId: string; name: string; op?: "upsert" | "delete" }[];
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
        draftId: input.id,
        nodeId: node.nodeId,
        name: node.name,
        op: node.op ?? "upsert",
      })),
    };

    return ModelRevisionDraftAggregate.create(snapshot);
  },
};

const extractNodeName = (payload: unknown): string => {
  if (payload && typeof payload === "object") {
    const name = (payload as { name?: unknown }).name;
    if (typeof name === "string") {
      return name;
    }
  }
  return "";
};
