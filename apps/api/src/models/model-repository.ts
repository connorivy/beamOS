import { eq, inArray, max } from "drizzle-orm";
import { getDb } from "../db/client";
import {
  modelBranchHeads,
  modelRevisions,
  models,
  revisionChanges,
} from "../db/schema";
import { modelBranchHeadMapper } from "../model-branch-heads/model-branch-head-mapper";
import { ModelAggregate } from "./model-aggregate";
import { modelMapper } from "./model-mapper";

const applyModelChanges = (input: {
  currentName: string;
  changes: { op: string; payload: unknown }[];
}): string => {
  let name = input.currentName;

  for (const change of input.changes) {
    if (change.op === "delete") {
      continue;
    }

    name = extractModelName(change.payload, name);
  }

  return name;
};

export type ModelRepository = {
  getUserModels: () => Promise<
    {
      id: string;
      name: string;
      description: string;
      lastModified: Date | null;
      role: "Owner" | "Contributor" | "Reviewer";
    }[]
  >;
  getById: (input: {
    modelId: string;
    revisionId?: string;
    loadModelBranchHeadAggregates?: boolean;
  }) => Promise<ModelAggregate | undefined>;
  create: (input: {
    model: ModelAggregate;
    authorId: string;
    message: string;
  }) => Promise<{ model: ModelAggregate; revisionId: string }>;
  update: (input: {
    model: ModelAggregate;
  }) => Promise<ModelAggregate>;
};

export const drizzleModelRepository: ModelRepository = {
  async getUserModels() {
    const latestRevisionByModel = getDb()
      .select({
        modelId: modelRevisions.modelId,
        lastModified: max(modelRevisions.createdAt).as("lastModified"),
      })
      .from(modelRevisions)
      .groupBy(modelRevisions.modelId)
      .as("latest_revision_by_model");

    const modelRows = await getDb()
      .select({
        id: models.id,
        name: models.name,
        description: models.description,
        lastModified: latestRevisionByModel.lastModified,
      })
      .from(models)
      .leftJoin(latestRevisionByModel, eq(models.id, latestRevisionByModel.modelId));

    return modelRows.map((model) => ({
      id: model.id,
      name: model.name,
      description: model.description,
      lastModified: model.lastModified,
      role: "Owner" as const,
    }));
  },

  async getById(input) {
    const modelRows = await getDb()
      .select()
      .from(models)
      .where(eq(models.id, input.modelId))
      .limit(1);

    if (!modelRows[0]) {
      return undefined;
    }
    const loadedModelBranchHeads = input.loadModelBranchHeadAggregates
      ? await listModelBranchHeads(input.modelId)
      : null;

    if (input.revisionId) {
      const revisions = await loadRevisionHistory({
        revisionId: input.revisionId,
      });

      if (
        revisions.length === 0 ||
        revisions[revisions.length - 1].modelId !== input.modelId
      ) {
        return undefined;
      }

      const modelName = await buildModelNameFromRevisions({
        initialName: revisions[revisions.length - 1].modelName,
        revisions,
      });

      return ModelAggregate.rehydrate({
        id: input.modelId,
        name: modelName,
        description: modelRows[0].description,
        modelBranchHeads: loadedModelBranchHeads,
      });
    }

    return ModelAggregate.rehydrate({
      ...modelMapper.toDomain(modelRows[0]).toSnapshot(),
      modelBranchHeads: loadedModelBranchHeads,
    });
  },

  async create(input) {
    const persistence = modelMapper.toPersistence(input.model);
    const initialRevisionId = Bun.randomUUIDv7();
    input.model.pullDomainEvents();

    const model = await getDb().transaction(async (tx) => {
      const row = await tx
        .insert(models)
        .values(persistence)
        .returning();

      await tx.insert(modelRevisions).values({
        id: initialRevisionId,
        modelId: input.model.id,
        modelName: input.model.name,
        parentRevisionId: null,
        secondParentRevisionId: null,
        authorId: input.authorId,
        message: input.message,
      });
      await tx.insert(modelBranchHeads).values({
        modelId: input.model.id,
        branchName: "main",
        headRevisionId: initialRevisionId,
      });

      return ModelAggregate.rehydrate({
        id: row[0].id,
        name: row[0].name,
        description: row[0].description,
        modelBranchHeads: input.model.modelBranchHeads,
      });
    });

    return {
      model,
      revisionId: initialRevisionId,
    };
  },

  async update(input) {
    const persistence = modelMapper.toPersistence(input.model);
    input.model.pullDomainEvents();

    const savedModel = await getDb().transaction(async (tx) => {
      const row = await tx
        .insert(models)
        .values(persistence)
        .onConflictDoUpdate({
          target: models.id,
          set: {
            name: persistence.name,
            description: persistence.description,
          },
        })
        .returning();

      return ModelAggregate.rehydrate({
        id: row[0].id,
        name: row[0].name,
        description: row[0].description,
        modelBranchHeads: input.model.modelBranchHeads,
      });
    });

    return savedModel;
  },
};

const listModelBranchHeads = async (modelId: string) => {
  const rows = await getDb()
    .select()
    .from(modelBranchHeads)
    .where(eq(modelBranchHeads.modelId, modelId));
  return rows.map((row) => modelBranchHeadMapper.toDomain(row));
};

const loadRevisionHistory = async (input: {
  revisionId?: string | null;
  secondRevisionId?: string | null;
}): Promise<(typeof modelRevisions.$inferSelect)[]> => {
  const visited = new Set<string>();
  const queue: string[] = [];

  if (input.revisionId) {
    queue.push(input.revisionId);
  }
  if (input.secondRevisionId) {
    queue.push(input.secondRevisionId);
  }

  const revisions: (typeof modelRevisions.$inferSelect)[] = [];

  while (queue.length > 0) {
    const id = queue.shift();
    if (!id || visited.has(id)) {
      continue;
    }
    visited.add(id);

    const rows = await getDb()
      .select()
      .from(modelRevisions)
      .where(eq(modelRevisions.id, id))
      .limit(1);

    if (!rows[0]) {
      continue;
    }

    const revision = rows[0];
    revisions.push(revision);

    if (revision.parentRevisionId) {
      queue.push(revision.parentRevisionId);
    }
    if (revision.secondParentRevisionId) {
      queue.push(revision.secondParentRevisionId);
    }
  }

  return revisions.sort((a, b) => {
    const timeDiff = a.createdAt.getTime() - b.createdAt.getTime();
    if (timeDiff !== 0) {
      return timeDiff;
    }
    return a.id.localeCompare(b.id);
  });
};


const extractModelName = (payload: unknown, fallback: string): string => {
  if (payload && typeof payload === "object") {
    const name = (payload as { name?: unknown }).name;
    if (typeof name === "string" && name.trim().length > 0) {
      return name;
    }
  }

  return fallback;
};

const buildModelNameFromRevisions = async (input: {
  initialName: string;
  revisions: (typeof modelRevisions.$inferSelect)[];
}): Promise<string> => {
  if (input.revisions.length === 0) {
    return input.initialName;
  }

  const revisionIds = input.revisions.map((revision) => revision.id);
  const revisionOrder = new Map(
    input.revisions.map((revision, index) => [revision.id, index]),
  );

  const changeRows = await getDb()
    .select()
    .from(revisionChanges)
    .where(inArray(revisionChanges.revisionId, revisionIds));

  changeRows.sort((a, b) => {
    const orderA = revisionOrder.get(a.revisionId ?? "") ?? 0;
    const orderB = revisionOrder.get(b.revisionId ?? "") ?? 0;
    if (orderA !== orderB) {
      return orderA - orderB;
    }
    return a.createdAt.getTime() - b.createdAt.getTime();
  });

  return applyModelChanges({
    currentName: input.initialName,
    changes: changeRows
      .filter((change) => change.entityType === "model")
      .map((change) => ({
        op: change.op,
        payload: change.payload,
      })),
  });
};
