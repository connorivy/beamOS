import { nodeRevisionDrafts } from "../db/schema";
import { NodeRevisionDraftAggregate } from "./node-revision-draft-aggregate";

export const nodeRevisionDraftMapper = {
  toDomain(
    row: typeof nodeRevisionDrafts.$inferSelect,
  ): NodeRevisionDraftAggregate {
    const op =
      row.op === "delete"
        ? "delete"
        : row.op === "insert"
          ? "insert"
          : "update";

    return NodeRevisionDraftAggregate.rehydrate({
      draftId: row.draftId,
      nodeId: row.nodeId,
      name: row.name,
      op,
    });
  },

  toPersistence(
    aggregate: NodeRevisionDraftAggregate,
  ): typeof nodeRevisionDrafts.$inferInsert {
    const snapshot = aggregate.toSnapshot();
    return {
      draftId: snapshot.draftId,
      nodeId: snapshot.nodeId,
      name: snapshot.name,
      op: snapshot.op,
    };
  },
};
