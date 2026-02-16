import crypto from "crypto";
import { eq, inArray } from "drizzle-orm";
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
  create: (input: {
    model: ModelAggregate;
    authorId: string;
    message: string;
  }) => Promise<{ model: ModelAggregate; revisionId: string }>;
  update: (input: {
    model: ModelAggregate;
    revisionId?: string | null;
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

  async create(input) {
    const persistence = modelMapper.toPersistence(input.model);
    const initialRevisionId = crypto.randomUUID();
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

      const modelChange = RevisionChangeEntity.create({
        id: crypto.randomUUID(),
        revisionId: initialRevisionId,
        entityType: "model",
        entityId: input.model.id,
        schemaVersion: 1,
        op: "insert",
        payload: {
          id: input.model.id,
          modelId: input.model.id,
          name: input.model.name,
        },
        createdAt: new Date(),
      });
      await tx.insert(revisionChanges).values(revisionChangeMapper.toPersistence(modelChange));

      return ModelAggregate.rehydrate({
        id: row[0].id,
        name: row[0].name,
        description: input.model.description,
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
    const events = input.model.pullDomainEvents();
    const revisionEvents = events.filter(
      (event) => event.type !== "model_created",
    );

    const savedModel = await getDb().transaction(async (tx) => {
      const row = await tx
        .insert(models)
        .values(persistence)
        .onConflictDoUpdate({
          target: models.id,
          set: {
            name: persistence.name,
          },
        })
        .returning();

      if (revisionEvents.length > 0) {
        const revisionId = input.revisionId;

        if (!revisionId) {
          throw new Error(
            "Cannot persist model changes without a source revision",
          );
        }

        const changeRows = buildRevisionChanges({
          modelId: input.model.id,
          revisionId,
          events: revisionEvents,
        });

        if (changeRows.length > 0) {
          await tx.insert(revisionChanges).values(changeRows);
        }
      }

      return ModelAggregate.rehydrate({
        id: row[0].id,
        name: row[0].name,
        description: input.model.description,
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

const buildRevisionChanges = (input: {
  modelId: string;
  revisionId: string | null;
  events: ModelDomainEvent[];
}): (typeof revisionChanges.$inferInsert)[] => {
  const now = new Date();

  return input.events.map((event) => {
    if (event.type === "model_renamed") {
      const payload = {
        id: event.modelId,
        name: event.name,
      };

      const entity = RevisionChangeEntity.create({
        id: crypto.randomUUID(),
        revisionId: input.revisionId,
        entityType: "model",
        entityId: event.modelId,
        schemaVersion: 1,
        op: "update",
        payload,
        createdAt: now,
      });

      return revisionChangeMapper.toPersistence(entity);
    }

    if (event.type === "node_added") {
      const payload = {
        id: event.node.id,
        modelId: input.modelId,
        name: event.node.name,
      };

      const entity = RevisionChangeEntity.create({
        id: crypto.randomUUID(),
        revisionId: input.revisionId,
        entityType: "node",
        entityId: event.node.id,
        schemaVersion: 1,
        op: "insert",
        payload,
        createdAt: now,
      });

      return revisionChangeMapper.toPersistence(entity);
    }

    if (event.type === "node_updated") {
      const payload = {
        id: event.node.id,
        modelId: input.modelId,
        name: event.node.name,
      };

      const entity = RevisionChangeEntity.create({
        id: crypto.randomUUID(),
        revisionId: input.revisionId,
        entityType: "node",
        entityId: event.node.id,
        schemaVersion: 1,
        op: "update",
        payload,
        createdAt: now,
      });

      return revisionChangeMapper.toPersistence(entity);
    }

    throw new Error(`Unsupported model domain event: ${event}`);
  });
};
