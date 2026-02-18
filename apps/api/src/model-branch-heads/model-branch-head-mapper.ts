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
      projectId: row.projectId,
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
      projectId: snapshot.projectId,
      branchName: snapshot.branchName,
      headRevisionId: snapshot.headRevisionId,
      updatedAt: snapshot.updatedAt,
    };
  },

  fromInput(input: {
    projectId: string;
    branchName: string;
    headRevisionId: string;
  }): ModelBranchHeadAggregate {
    const snapshot: ModelBranchHeadSnapshot = {
      projectId: input.projectId,
      branchName: input.branchName,
      headRevisionId: input.headRevisionId,
      updatedAt: new Date(),
    };

    return ModelBranchHeadAggregate.create(snapshot);
  },
};
