import { eq, inArray } from "drizzle-orm";
import { getDb, type DbTransaction } from "../db/client";
import {
  modelBranchHeads,
  modelRevisions,
  models,
  revisionChanges,
} from "../db/schema";
import { modelBranchHeadMapper } from "../model-branch-heads/model-branch-head-mapper";
import { ModelAggregate } from "./model-aggregate";
import { modelMapper } from "./model-mapper";
import type { ModelDomainEvent } from "./model-events";
import { RevisionChangeEntity } from "../revision-changes/revision-change-entity";
import { revisionChangeMapper } from "../revision-changes/revision-change-mapper";

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
  getById: (input: {
    modelId: string;
    revisionId?: string;
    loadModelBranchHeadAggregates?: boolean;
  }) => Promise<ModelAggregate | undefined>;
  save: (input: {
    model: ModelAggregate;
    initialCommit?: {
      authorId: string;
      message: string;
      revisionId?: string;
      branchName?: string;
    };
  }) => Promise<ModelAggregate>;
};

export const drizzleModelRepository: ModelRepository = {
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
        description: "",
        modelBranchHeads: loadedModelBranchHeads,
      });
    }

    return ModelAggregate.rehydrate({
      ...modelMapper.toDomain(modelRows[0]).toSnapshot(),
      modelBranchHeads: loadedModelBranchHeads,
    });
  },

  async save(input) {
    const persistence = modelMapper.toPersistence(input.model);
    const events = input.model.pullDomainEvents();

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

      await publishModelDomainEvents({
        tx,
        events,
        initialCommit: input.initialCommit,
      });

      return ModelAggregate.rehydrate({
        id: row[0].id,
        name: row[0].name,
        description: row[0].description ?? "",
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

const publishModelDomainEvents = async (input: {
  tx: DbTransaction;
  events: ModelDomainEvent[];
  initialCommit?: {
    authorId: string;
    message: string;
    revisionId?: string;
    branchName?: string;
  };
}) => {
  for (const event of input.events) {
    if (event.type === "model_created") {
      await handleModelCreatedEvent({
        tx: input.tx,
        event,
        initialCommit: input.initialCommit,
      });
    }
  }
};

const handleModelCreatedEvent = async (input: {
  tx: DbTransaction;
  event: Extract<ModelDomainEvent, { type: "model_created" }>;
  initialCommit?: {
    authorId: string;
    message: string;
    revisionId?: string;
    branchName?: string;
  };
}) => {
  if (!input.initialCommit) {
    throw new Error(
      "Cannot handle model_created event without initial commit metadata",
    );
  }

  const revisionId = input.initialCommit.revisionId ?? Bun.randomUUIDv7();
  const branchName = input.initialCommit.branchName ?? "main";

  await input.tx.insert(modelRevisions).values({
    id: revisionId,
    modelId: input.event.payload.id,
    modelName: input.event.payload.name,
    parentRevisionId: null,
    secondParentRevisionId: null,
    authorId: input.initialCommit.authorId,
    message: input.initialCommit.message,
  });

  const modelRevisionChange = revisionChangeMapper.toPersistence(
    RevisionChangeEntity.create({
      id: Bun.randomUUIDv7(),
      revisionId,
      entityType: "model",
      entityId: input.event.payload.id,
      schemaVersion: 1,
      op: "insert",
      payload: {
        id: input.event.payload.id,
        name: input.event.payload.name,
      },
      createdAt: new Date(),
    }),
  );

  await input.tx.insert(revisionChanges).values(modelRevisionChange);

  await input.tx
    .insert(modelBranchHeads)
    .values({
      modelId: input.event.payload.id,
      branchName,
      headRevisionId: revisionId,
    })
    .onConflictDoUpdate({
      target: [modelBranchHeads.modelId, modelBranchHeads.branchName],
      set: {
        headRevisionId: revisionId,
        updatedAt: new Date(),
      },
    });
};
