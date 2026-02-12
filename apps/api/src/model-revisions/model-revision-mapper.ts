import { modelRevisions, revisionChanges } from "../db/schema";
import {
  ModelRevisionAggregate,
  type ModelRevisionSnapshot,
} from "./model-revision-aggregate";
import type { NodeSnapshot } from "../nodes/node-entity";

export const modelRevisionMapper = {
  toDomain(
    row: typeof modelRevisions.$inferSelect,
    changeRows: (typeof revisionChanges.$inferSelect)[],
  ): ModelRevisionAggregate {
    return ModelRevisionAggregate.rehydrate({
      id: row.id,
      modelId: row.modelId,
      name: row.modelName,
      parentRevisionId: row.parentRevisionId,
      secondParentRevisionId: row.secondParentRevisionId,
      authorId: row.authorId,
      message: row.message,
      createdAt: row.createdAt,
      nodes: changeRows
        .filter((change) => change.entityType === "node")
        .filter((change) => change.op !== "delete")
        .map((change) => ({
          id: change.entityId,
          modelId: row.modelId,
          name: extractNodeName(change.payload),
        })),
    });
  },

  toPersistence(
    aggregate: ModelRevisionAggregate,
  ): typeof modelRevisions.$inferInsert {
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
    };
  },

  fromCommitInput(input: {
    id: string;
    modelId: string;
    name: string;
    parentRevisionId: string | null;
    secondParentRevisionId: string | null;
    authorId: string;
    message: string;
    nodes: NodeSnapshot[];
  }): ModelRevisionAggregate {
    const snapshot: ModelRevisionSnapshot = {
      id: input.id,
      modelId: input.modelId,
      name: input.name,
      parentRevisionId: input.parentRevisionId,
      secondParentRevisionId: input.secondParentRevisionId,
      authorId: input.authorId,
      message: input.message,
      createdAt: new Date(),
      nodes: input.nodes.map((node) => ({
        id: node.id,
        modelId: node.modelId,
        name: node.name,
      })),
    };

    return ModelRevisionAggregate.create(snapshot);
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
