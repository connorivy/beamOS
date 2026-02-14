import crypto from "crypto";
import { eq, inArray } from "drizzle-orm";
import { Ratio } from "unitsnet-js";
import { getDb } from "../db/client";
import {
  modelRevisionDrafts,
  modelRevisions,
  models,
  nodes,
  revisionChanges,
} from "../db/schema";
import { ModelAggregate } from "./model-aggregate";
import { modelMapper } from "./model-mapper";
import type { ModelDomainEvent } from "./model-events";
import { RevisionChangeEntity } from "../revision-changes/revision-change-entity";
import { revisionChangeMapper } from "../revision-changes/revision-change-mapper";
import type { NodeRestraint, NodeSnapshot } from "../nodes/node-entity";

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
      restraint: {
        canTranslateAlongX: true,
        canTranslateAlongY: true,
        canTranslateAlongZ: true,
        canRotateAboutX: true,
        canRotateAboutY: true,
        canRotateAboutZ: true,
      },
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
    draftId?: string;
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

    if (input.draftId) {
      const [draftRows, draftChangeRows] = await Promise.all([
        getDb()
          .select()
          .from(modelRevisionDrafts)
          .where(eq(modelRevisionDrafts.id, input.draftId))
          .limit(1),
        getDb()
          .select()
          .from(revisionChanges)
          .where(eq(revisionChanges.draftId, input.draftId)),
      ]);

      if (!draftRows[0] || draftRows[0].modelId !== input.modelId) {
        return undefined;
      }

      const orderedDraftChanges = [...draftChangeRows].sort((a, b) => {
        const timeDiff = a.createdAt.getTime() - b.createdAt.getTime();
        if (timeDiff !== 0) {
          return timeDiff;
        }

        return a.id.localeCompare(b.id);
      });

      const baseRevisions = await loadRevisionHistory({
        revisionId: draftRows[0].parentRevisionId,
        secondRevisionId: draftRows[0].secondParentRevisionId,
      });
      const nodesById = await buildNodesFromRevisions({
        revisions: baseRevisions,
      });

      applyNodeChanges({
        current: nodesById,
        changes: orderedDraftChanges
          .filter((change) => change.entityType === "node")
          .map((change) => ({
            nodeId: change.entityId,
            modelRevisionId: draftRows[0].id,
            nodeTypeDescriminator: extractNodeTypeDescriminator(change.payload),
            op: change.op,
          })),
      });

      const modelName = applyModelChanges({
        currentName: draftRows[0].modelName,
        changes: orderedDraftChanges
          .filter((change) => change.entityType === "model")
          .map((change) => ({
            op: change.op,
            payload: change.payload,
          })),
      });

      return ModelAggregate.rehydrate({
        id: input.modelId,
        name: modelName,
        nodes: Array.from(nodesById.values()),
        sourceDraftId: input.draftId,
      });
    }

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
        sourceRevisionId: input.revisionId,
      });
    }

    return modelMapper.toDomain(modelRows[0]);
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
        const targetDraftId = model.sourceDraftId;

        if (!targetRevisionId && !targetDraftId) {
          throw new Error(
            "Cannot persist model changes without a source revision or draft",
          );
        }

        const changeRows = buildRevisionChanges({
          modelId: model.id,
          revisionId: targetRevisionId,
          draftId: targetDraftId,
          events: revisionEvents,
        });

        if (changeRows.length > 0) {
          await tx.insert(revisionChanges).values(changeRows);
        }
      }

      return modelMapper.toDomain(row[0]);
    });

    return savedModel;
  },
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

  const nodeRows = await getDb()
    .select()
    .from(nodes)
    .where(inArray(nodes.revisionId, revisionIds));

  nodeRows.sort((a, b) => {
    const orderA = revisionOrder.get(a.revisionId) ?? 0;
    const orderB = revisionOrder.get(b.revisionId) ?? 0;
    if (orderA !== orderB) {
      return orderA - orderB;
    }
    return a.id.localeCompare(b.id);
  });

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

  for (const row of nodeRows) {
    nodesById.set(row.id, toNodeSnapshotFromRow(row));
  }

  applyNodeChanges({
    current: nodesById,
    changes: changeRows
      .filter((change) => change.entityType === "node" && change.op === "delete")
      .map((change) => ({
        nodeId: change.entityId,
        modelRevisionId:
          change.revisionId ?? input.revisions[input.revisions.length - 1].id,
        op: change.op,
        nodeTypeDescriminator: "internal",
      })),
  });

  return nodesById;
};

const toNodeSnapshotFromRow = (row: typeof nodes.$inferSelect): NodeSnapshot => {
  const restraint = parseNodeRestraint(row.restraint);

  if (row.locationDiscriminator === "internal") {
    return {
      id: row.id,
      modelRevisionId: row.revisionId,
      nodeType: "internalNode",
      nodeTypeDescriminator: "internal",
      element1dId: row.element1dId ?? row.id,
      distanceAlongElement1d: Ratio.FromDecimalFractions(
        row.ratioAlongElement1d ?? 0,
      ),
      restraint,
    };
  }

  return {
    id: row.id,
    modelRevisionId: row.revisionId,
    nodeType: "spatialNode",
    nodeTypeDescriminator: "external",
    point: {
      x: row.pointX ?? 0,
      y: row.pointY ?? 0,
      z: row.pointZ ?? 0,
    },
    restraint,
  };
};

const parseNodeRestraint = (value: unknown): NodeRestraint => {
  const defaultRestraint: NodeRestraint = {
    canTranslateAlongX: true,
    canTranslateAlongY: true,
    canTranslateAlongZ: true,
    canRotateAboutX: true,
    canRotateAboutY: true,
    canRotateAboutZ: true,
  };

  if (!value || typeof value !== "object") {
    return defaultRestraint;
  }

  const obj = value as Record<string, unknown>;
  return {
    canTranslateAlongX: typeof obj.canTranslateAlongX === "boolean" ? obj.canTranslateAlongX : true,
    canTranslateAlongY: typeof obj.canTranslateAlongY === "boolean" ? obj.canTranslateAlongY : true,
    canTranslateAlongZ: typeof obj.canTranslateAlongZ === "boolean" ? obj.canTranslateAlongZ : true,
    canRotateAboutX: typeof obj.canRotateAboutX === "boolean" ? obj.canRotateAboutX : true,
    canRotateAboutY: typeof obj.canRotateAboutY === "boolean" ? obj.canRotateAboutY : true,
    canRotateAboutZ: typeof obj.canRotateAboutZ === "boolean" ? obj.canRotateAboutZ : true,
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
  draftId: string | null;
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
        draftId: input.draftId,
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
        draftId: input.draftId,
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
        draftId: input.draftId,
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
