import crypto from "crypto";
import { eq, inArray } from "drizzle-orm";
import { Ratio } from "unitsnet-js";
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
import type { NodeRestraint, NodeSnapshot } from "../nodes/node-entity";
import { NodeRestraints, parseRestraint } from "../nodes/node-entity";

const applyNodeChanges = (input: {
  current: Map<string, NodeSnapshot>;
  changes: {
    nodeId: string;
    modelRevisionId: string;
    nodeTypeDescriminator: "external" | "internal";
    op: string;
  }[];
}) => {
  for (const change of input.changes) {
    if (change.op === "delete") {
      input.current.delete(change.nodeId);
      continue;
    }

    input.current.set(change.nodeId, {
      id: change.nodeId,
      modelRevisionId: change.modelRevisionId,
      nodeType:
        change.nodeTypeDescriminator === "external"
          ? "spatialNode"
          : "internalNode",
      nodeTypeDescriminator: change.nodeTypeDescriminator ?? "internal",
      point:
        change.nodeTypeDescriminator === "external"
          ? { x: 0, y: 0, z: 0 }
          : undefined,
      element1dId:
        change.nodeTypeDescriminator === "external" ? undefined : change.nodeId,
      distanceAlongElement1d:
        change.nodeTypeDescriminator === "external"
          ? undefined
          : Ratio.FromDecimalFractions(0),
      restraint: { ...NodeRestraints.FREE },
    });
  }
};

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
  save: (model: ModelAggregate) => Promise<ModelAggregate>;
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

      const nodesById = await buildNodesFromRevisions({
        revisions,
      });

      const modelName = await buildModelNameFromRevisions({
        initialName: revisions[revisions.length - 1].modelName,
        revisions,
      });

      return ModelAggregate.rehydrate({
        id: input.modelId,
        name: modelName,
        nodes: Array.from(nodesById.values()),
        description: "",
        modelBranchHeads: loadedModelBranchHeads,
        sourceRevisionId: input.revisionId,
      });
    }

    return ModelAggregate.rehydrate({
      ...modelMapper.toDomain(modelRows[0]).toSnapshot(),
      modelBranchHeads: loadedModelBranchHeads,
    });
  },

  async save(model) {
    const persistence = modelMapper.toPersistence(model);
    const events = model.pullDomainEvents();
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
        const targetRevisionId = model.sourceRevisionId;

        if (!targetRevisionId) {
          throw new Error(
            "Cannot persist model changes without a source revision",
          );
        }

        const changeRows = buildRevisionChanges({
          modelId: model.id,
          revisionId: targetRevisionId,
          events: revisionEvents,
        });

        if (changeRows.length > 0) {
          await tx.insert(revisionChanges).values(changeRows);
        }
      }

      return ModelAggregate.rehydrate({
        id: row[0].id,
        name: row[0].name,
        nodes: model.nodes.map((node) => node.toSnapshot()),
        description: model.description,
        modelBranchHeads: model.modelBranchHeads,
        sourceRevisionId: model.sourceRevisionId,
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

const buildNodesFromRevisions = async (input: {
  revisions: (typeof modelRevisions.$inferSelect)[];
}): Promise<Map<string, NodeSnapshot>> => {
  if (input.revisions.length === 0) {
    return new Map();
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
    return a.entityId.localeCompare(b.entityId);
  });

  const nodesById = new Map<string, NodeSnapshot>();
  for (const change of changeRows) {
    if (change.entityType !== "node") {
      continue;
    }
    if (change.op === "delete") {
      nodesById.delete(change.entityId);
      continue;
    }
    nodesById.set(change.entityId, toNodeSnapshotFromChange(change));
  }

  return nodesById;
};

const toNodeSnapshotFromChange = (
  change: typeof revisionChanges.$inferSelect,
): NodeSnapshot => {
  const payload =
    change.payload && typeof change.payload === "object"
      ? (change.payload as Record<string, unknown>)
      : {};
  const modelRevisionId =
    typeof payload.modelRevisionId === "string"
      ? payload.modelRevisionId
      : (change.revisionId ?? "");
  const nodeTypeDescriminator = extractNodeTypeDescriminator(payload);
  const restraint = parseRestraint(payload.restraint);

  if (nodeTypeDescriminator === "internal") {
    const element1dId =
      typeof payload.element1dId === "string" ? payload.element1dId : change.entityId;
    const distanceAlongElement1d =
      typeof payload.distanceAlongElement1d === "number" &&
      Number.isFinite(payload.distanceAlongElement1d)
        ? payload.distanceAlongElement1d
        : 0;
    return {
      id: change.entityId,
      modelRevisionId,
      nodeType: "internalNode",
      nodeTypeDescriminator: "internal",
      element1dId,
      distanceAlongElement1d: Ratio.FromDecimalFractions(distanceAlongElement1d),
      restraint,
    };
  }

  const point =
    payload.point && typeof payload.point === "object"
      ? (payload.point as Record<string, unknown>)
      : {};
  return {
    id: change.entityId,
    modelRevisionId,
    nodeType: "spatialNode",
    nodeTypeDescriminator: "external",
    point: {
      x: typeof point.x === "number" && Number.isFinite(point.x) ? point.x : 0,
      y: typeof point.y === "number" && Number.isFinite(point.y) ? point.y : 0,
      z: typeof point.z === "number" && Number.isFinite(point.z) ? point.z : 0,
    },
    restraint,
  };
};

const extractNodeTypeDescriminator = (
  payload: unknown,
): "external" | "internal" => {
  if (payload && typeof payload === "object") {
    const nodeTypeDescriminator = (
      payload as { nodeTypeDescriminator?: unknown }
    ).nodeTypeDescriminator;
    if (nodeTypeDescriminator === "external") {
      return "external";
    }
  }
  return "internal";
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
