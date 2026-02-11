import {
  models,
  type Model as ModelRow,
  type Node as NodeRow,
} from "../db/schema";
import { ModelAggregate } from "./model-aggregate";

export const modelMapper = {
  toDomain(row: ModelRow, nodeRows: NodeRow[] = []): ModelAggregate {
    return ModelAggregate.rehydrate({
      id: row.id,
      name: row.name,
      nodes: nodeRows.map((node) => ({
        id: node.id,
        modelId: node.modelId,
        name: node.name,
      })),
    });
  },
  toPersistence(aggregate: ModelAggregate): typeof models.$inferInsert {
    return {
      id: aggregate.id,
      name: aggregate.name,
    };
  },
};
