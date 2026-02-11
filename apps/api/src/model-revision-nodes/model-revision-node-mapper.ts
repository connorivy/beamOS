import { modelRevisionNodes } from "../db/schema";
import { ModelRevisionNodeEntity } from "./model-revision-node-entity";

export const modelRevisionNodeMapper = {
  toDomain(row: typeof modelRevisionNodes.$inferSelect): ModelRevisionNodeEntity {
    return ModelRevisionNodeEntity.rehydrate({
      revisionId: row.revisionId,
      nodeId: row.nodeId,
      name: row.name,
    });
  },

  toPersistence(
    entity: ModelRevisionNodeEntity,
  ): typeof modelRevisionNodes.$inferInsert {
    const snapshot = entity.toSnapshot();
    return {
      revisionId: snapshot.revisionId,
      nodeId: snapshot.nodeId,
      name: snapshot.name,
    };
  },
};
