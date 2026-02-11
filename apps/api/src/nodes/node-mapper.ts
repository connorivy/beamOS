import { nodes, type Node as NodeRow } from "../db/schema";
import { NodeAggregate } from "./node-aggregate";

export const nodeMapper = {
  toDomain(row: NodeRow): NodeAggregate {
    return NodeAggregate.rehydrate({
      id: row.id,
      modelId: row.modelId,
      name: row.name,
    });
  },
  toPersistence(aggregate: NodeAggregate): typeof nodes.$inferInsert {
    const snapshot = aggregate.toSnapshot();
    return {
      id: snapshot.id,
      modelId: snapshot.modelId,
      name: snapshot.name,
    };
  },
};
