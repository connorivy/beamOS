import { nodeRevisions } from "../db/schema";
import { NodeRevisionAggregate } from "./node-revision-aggregate";

export const nodeRevisionMapper = {
  toDomain(row: typeof nodeRevisions.$inferSelect): NodeRevisionAggregate {
    const op =
      row.op === "delete"
        ? "delete"
        : row.op === "insert"
          ? "insert"
          : "update";

    return NodeRevisionAggregate.rehydrate({
      revisionId: row.revisionId,
      nodeId: row.nodeId,
      name: row.name,
      op,
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
