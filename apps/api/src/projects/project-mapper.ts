import { projects } from "../db/schema";
import { ProjectEntity } from "./project-aggregate";
import { projectResponseSchema } from "./create-project";
import z from "zod";

export const projectMapper = {
  toDomain(row: typeof projects.$inferSelect): ProjectEntity {
    return ProjectEntity.rehydrate({
      id: row.id,
      name: row.name,
      description: row.description,
      modelBranchHeads: null,
    });
  },
  toPersistence(aggregate: ProjectEntity): typeof projects.$inferInsert {
    return {
      id: aggregate.id,
      name: aggregate.name,
      description: aggregate.description,
    };
  },
  toResponse(aggregate: ProjectEntity): z.infer<typeof projectResponseSchema> {
    const revisionLastModified = aggregate.modelRevisions?.reduce(
      (latest, revision) =>
        revision.createdAt > latest ? revision.createdAt : latest,
      aggregate.modelRevisions[0]?.createdAt,
    );
    if (!revisionLastModified) {
      throw new Error(
        "ProjectEntity must have at least one revision to determine lastModified",
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
