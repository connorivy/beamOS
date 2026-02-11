import { modelRevisionNodes, modelRevisions } from "../db/schema";
import { modelRevisionNodeMapper } from "../model-revision-nodes/model-revision-node-mapper";
import {
  ModelRevisionAggregate,
  type ModelRevisionSnapshot,
} from "./model-revision-aggregate";

export const modelRevisionMapper = {
  toDomain(
    row: typeof modelRevisions.$inferSelect,
    nodeRows: (typeof modelRevisionNodes.$inferSelect)[],
  ): ModelRevisionAggregate {
    const nodes = nodeRows.map((rowNode) => {
      const nodeEntity = modelRevisionNodeMapper.toDomain(rowNode);
      const node = nodeEntity.toSnapshot();
      return {
        id: node.nodeId,
        modelId: row.modelId,
        name: node.name,
      };
    });

    return ModelRevisionAggregate.rehydrate({
      id: row.id,
      modelId: row.modelId,
      name: row.modelName,
      parentRevisionId: row.parentRevisionId,
      secondParentRevisionId: row.secondParentRevisionId,
      authorId: row.authorId,
      message: row.message,
      createdAt: row.createdAt,
      nodes,
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
    nodes: { id: string; modelId: string; name: string }[];
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
      nodes: input.nodes,
    };

    return ModelRevisionAggregate.create(snapshot);
  },
};
