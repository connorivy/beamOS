import { revisionChanges } from "../db/schema";
import { RevisionChangeEntity } from "./revision-change-entity";

export const revisionChangeMapper = {
  toDomain(row: typeof revisionChanges.$inferSelect): RevisionChangeEntity {
    return RevisionChangeEntity.rehydrate({
      id: row.id,
      revisionId: row.revisionId,
      draftId: row.draftId,
      entityType: row.entityType,
      entityId: row.entityId,
      op: row.op === "delete" ? "delete" : "upsert",
      payload: row.payload as Record<string, unknown>,
      createdAt: row.createdAt,
    });
  },

  toPersistence(
    entity: RevisionChangeEntity,
  ): typeof revisionChanges.$inferInsert {
    const snapshot = entity.toSnapshot();
    return {
      id: snapshot.id,
      revisionId: snapshot.revisionId,
      draftId: snapshot.draftId,
      entityType: snapshot.entityType,
      entityId: snapshot.entityId,
      op: snapshot.op,
      payload: snapshot.payload,
      createdAt: snapshot.createdAt,
    };
  },
};
