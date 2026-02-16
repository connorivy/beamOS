import { models, type Model as ModelRow } from "../db/schema";
import { ModelAggregate } from "./model-aggregate";

export const modelMapper = {
  toDomain(row: ModelRow): ModelAggregate {
    return ModelAggregate.rehydrate({
      id: row.id,
      name: row.name,
      description: row.description,
      modelBranchHeads: null,
    });
  },
  toPersistence(aggregate: ModelAggregate): typeof models.$inferInsert {
    return {
      id: aggregate.id,
      name: aggregate.name,
      description: aggregate.description,
    };
  },
};
