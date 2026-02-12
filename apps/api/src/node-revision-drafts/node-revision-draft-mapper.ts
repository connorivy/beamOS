import { nodeRevisionDrafts } from "../db/schema";
import { NodeRevisionDraftAggregate } from "./node-revision-draft-aggregate";

export const nodeRevisionDraftMapper = {
  toDomain(
    row: typeof nodeRevisionDrafts.$inferSelect,
  ): NodeRevisionDraftAggregate {
    return NodeRevisionDraftAggregate.rehydrate({
      draftId: row.draftId,
      nodeId: row.nodeId,
      name: row.name,
      op: row.op === "delete" ? "delete" : "upsert",
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
