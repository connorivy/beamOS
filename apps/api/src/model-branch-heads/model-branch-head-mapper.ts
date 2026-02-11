import { modelBranchHeads } from "../db/schema";
import {
  ModelBranchHeadAggregate,
  type ModelBranchHeadSnapshot,
} from "./model-branch-head-aggregate";

export const modelBranchHeadMapper = {
  toDomain(
    row: typeof modelBranchHeads.$inferSelect,
  ): ModelBranchHeadAggregate {
    return ModelBranchHeadAggregate.rehydrate({
      modelId: row.modelId,
      branchName: row.branchName,
      headRevisionId: row.headRevisionId,
      updatedAt: row.updatedAt,
    });
  },

  toPersistence(
    aggregate: ModelBranchHeadAggregate,
  ): typeof modelBranchHeads.$inferInsert {
    const snapshot = aggregate.toSnapshot();
    return {
      modelId: snapshot.modelId,
      branchName: snapshot.branchName,
      headRevisionId: snapshot.headRevisionId,
      updatedAt: snapshot.updatedAt,
    };
  },

  fromInput(input: {
    modelId: string;
    branchName: string;
    headRevisionId: string;
  }): ModelBranchHeadAggregate {
    const snapshot: ModelBranchHeadSnapshot = {
      modelId: input.modelId,
      branchName: input.branchName,
      headRevisionId: input.headRevisionId,
      updatedAt: new Date(),
    };

    return ModelBranchHeadAggregate.create(snapshot);
  },
};
