import { models, type Model as ModelRow } from "../db/schema";
import { ModelAggregate } from "./model-aggregate";

export type ModelResponse = {
  id: string;
  name: string;
  description: string;
  lastModified: string;
  role: "Owner" | "Contributor" | "Reviewer";
};

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
  toResponse(aggregate: ModelAggregate): ModelResponse {
    const revisionLastModified = aggregate.modelRevisions?.reduce(
      (latest, revision) =>
        revision.createdAt > latest ? revision.createdAt : latest,
      aggregate.modelRevisions[0]?.createdAt,
    );
    if (!revisionLastModified) {
      throw new Error(
        "ModelAggregate must have at least one revision to determine lastModified",
      );
    }

    return {
      id: aggregate.id,
      name: aggregate.name,
      description: aggregate.description,
      lastModified: revisionLastModified.toISOString(),
      role: "Owner",
    };
  },
};
