import { revisionChanges } from "../db/schema";
import { RevisionChangeEntity } from "./revision-change-entity";

export const revisionChangeMapper = {
  toDomain(row: typeof revisionChanges.$inferSelect): RevisionChangeEntity {
    const op =
      row.op === "delete"
        ? "delete"
        : row.op === "insert"
          ? "insert"
          : "update";

    return RevisionChangeEntity.rehydrate({
      id: row.id,
      revisionId: row.revisionId,
      draftId: row.draftId,
      entityType: row.entityType,
      entityId: row.entityId,
      schemaVersion: row.schemaVersion,
      op,
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
      schemaVersion: snapshot.schemaVersion,
      op: snapshot.op,
      payload: snapshot.payload,
      createdAt: snapshot.createdAt,
    };
  },
};
