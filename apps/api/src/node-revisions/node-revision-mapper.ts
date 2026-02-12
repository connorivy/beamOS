import { nodeRevisions } from "../db/schema";
import { NodeRevisionAggregate } from "./node-revision-aggregate";

export const nodeRevisionMapper = {
  toDomain(row: typeof nodeRevisions.$inferSelect): NodeRevisionAggregate {
    return NodeRevisionAggregate.rehydrate({
      revisionId: row.revisionId,
      nodeId: row.nodeId,
      name: row.name,
      op: row.op === "delete" ? "delete" : "upsert",
    });
  },

  toPersistence(
    aggregate: NodeRevisionAggregate,
  ): typeof nodeRevisions.$inferInsert {
    const snapshot = aggregate.toSnapshot();
    return {
      revisionId: snapshot.revisionId,
      nodeId: snapshot.nodeId,
      name: snapshot.name,
      op: snapshot.op,
    };
  },
};
