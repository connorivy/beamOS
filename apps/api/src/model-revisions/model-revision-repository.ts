import crypto from "crypto";
import { and, eq, inArray, sql } from "drizzle-orm";
import { getDb } from "../db/client";
import {
  element1ds,
  materials,
  modelBranchHeads,
  modelRevisionDrafts,
  modelRevisions,
  revisionChanges,
  sectionProfiles,
} from "../db/schema";
import { modelBranchHeadMapper } from "../model-branch-heads/model-branch-head-mapper";
import { modelRevisionDraftMapper } from "../model-revision-drafts/model-revision-draft-mapper";
import { ModelRevisionDraftAggregate } from "../model-revision-drafts/model-revision-draft-aggregate";
import { ModelRevisionAggregate } from "./model-revision-aggregate";
import { modelRevisionMapper } from "./model-revision-mapper";
import { RevisionChangeEntity } from "../revision-changes/revision-change-entity";
import { revisionChangeMapper } from "../revision-changes/revision-change-mapper";
import type { DomainEvent, ModelRevisionRepository } from "../common/types";
import { materialMapper } from "../materials/material-mapper";
import { sectionProfileMapper } from "../section-profiles/section-profile-mapper";
import { element1dMapper } from "../element1ds/element1d-mapper";
import type { MaterialSnapshot } from "../materials/material-entity";
import type { SectionProfileSnapshot } from "../section-profiles/section-profile-aggregate";
import type { Element1dSnapshot } from "../element1ds/element1d-entity";
import type { NodeSnapshot } from "../nodes/node-entity";

export const drizzleModelVersionRepository: ModelRevisionRepository = {
  async getRevisionById(revisionId) {
    const revisionRows = await getDb()
      .select()
      .from(modelRevisions)
      .where(eq(modelRevisions.id, revisionId))
      .limit(1);

    if (!revisionRows[0]) {
      return undefined;
    }

    const revision = revisionRows[0];
    const revisions = await loadRevisionHistory({
      revisionId: revision.id,
    });
    const [nodesById, materialsById, sectionProfilesById, element1dsById] =
      await Promise.all([
        buildNodesFromRevisions({
          modelId: revision.modelId,
          revisions,
        }),
        buildMaterialsFromRevisions({ revisions }),
        buildSectionProfilesFromRevisions({ revisions }),
        buildElement1dsFromRevisions({ revisions }),
      ]);

    return ModelRevisionAggregate.rehydrate({
      id: revision.id,
      modelId: revision.modelId,
      name: revision.modelName,
      parentRevisionId: revision.parentRevisionId,
      secondParentRevisionId: revision.secondParentRevisionId,
      authorId: revision.authorId,
      message: revision.message,
      createdAt: revision.createdAt,
      nodes: Array.from(nodesById.values()).map((node) => ({
        id: node.id,
        modelId: node.modelId,
        nodeTypeDescriminator: node.nodeTypeDescriminator,
      })),
      materials: Array.from(materialsById.values()),
      sectionProfiles: Array.from(sectionProfilesById.values()),
      element1ds: Array.from(element1dsById.values()),
    });
  },

  async getDraftById(draftId) {
    const [draftRows, draftChangeRows] = await Promise.all([
      getDb()
        .select()
        .from(modelRevisionDrafts)
        .where(eq(modelRevisionDrafts.id, draftId))
        .limit(1),
      getDb()
        .select()
        .from(revisionChanges)
        .where(eq(revisionChanges.draftId, draftId)),
    ]);

    if (!draftRows[0]) {
      return undefined;
    }

    const draft = draftRows[0];
    const baseRevisions = await loadRevisionHistory({
      revisionId: draft.parentRevisionId,
      secondRevisionId: draft.secondParentRevisionId,
    });
    const nodesById = await buildNodesFromRevisions({
      modelId: draft.modelId,
      revisions: baseRevisions,
    });

    applyNodeChanges({
      modelId: draft.modelId,
      current: nodesById,
      changes: draftChangeRows
        .filter((change) => change.entityType === "node")
        .map((change) => ({
          nodeId: change.entityId,
          name: extractNodeName(change.payload),
          nodeTypeDescriminator: extractNodeTypeDescriminator(change.payload),
          op: change.op,
        })),
    });

    return ModelRevisionDraftAggregate.rehydrate({
      id: draft.id,
      modelId: draft.modelId,
      name: draft.modelName,
      parentRevisionId: draft.parentRevisionId,
      secondParentRevisionId: draft.secondParentRevisionId,
      authorId: draft.authorId,
      message: draft.message,
      createdAt: draft.createdAt,
      updatedAt: draft.updatedAt,
      nodes: Array.from(nodesById.values()).map((node) => ({
        draftId: draft.id,
        nodeId: node.id,
        name: node.name,
        op: "update",
      })),
    });
  },

  async save(input) {
    const { revision, newRevision = false } = input;
    const snapshot = revision.toSnapshot();
    const events = revision.pullDomainEvents();

    if (newRevision) {
      return getDb().transaction(async (tx) => {
        const revisionRow = await tx
          .insert(modelRevisions)
          .values(modelRevisionMapper.toPersistence(revision))
          .returning();

        const changeRows = buildRevisionChangeRowsFromEvents({
          modelId: snapshot.modelId,
          revisionId: snapshot.id,
          draftId: null,
          events,
        });

        if (changeRows.length > 0) {
          await tx
            .insert(revisionChanges)
            .values(changeRows);
        }

        const persistedChangeRows = await tx
          .select()
          .from(revisionChanges)
          .where(eq(revisionChanges.revisionId, snapshot.id));

        return modelRevisionMapper.toDomain(
          revisionRow[0],
          persistedChangeRows,
        );
      });
    }

    return getDb().transaction(async (tx) => {
      const now = new Date();
      const draftRows = await tx
        .select()
        .from(modelRevisionDrafts)
        .where(eq(modelRevisionDrafts.id, snapshot.id))
        .limit(1);

      const existingDraft = draftRows[0];
      const draftId = existingDraft?.id ?? crypto.randomUUID();
      const createdAt = existingDraft?.createdAt ?? now;
      const parentRevisionId = existingDraft
        ? (snapshot.parentRevisionId ?? null)
        : snapshot.id;
      const secondParentRevisionId = existingDraft
        ? (snapshot.secondParentRevisionId ?? null)
        : null;

      await tx
        .insert(modelRevisionDrafts)
        .values({
          id: draftId,
          modelId: snapshot.modelId,
          modelName: snapshot.name,
          parentRevisionId,
          secondParentRevisionId,
          authorId: snapshot.authorId,
          message: snapshot.message,
          createdAt,
          updatedAt: now,
        })
        .onConflictDoUpdate({
          target: modelRevisionDrafts.id,
          set: {
            modelId: snapshot.modelId,
            modelName: snapshot.name,
            parentRevisionId,
            secondParentRevisionId,
            authorId: snapshot.authorId,
            message: snapshot.message,
            updatedAt: sql`excluded.updated_at`,
          },
        });

      const changeRows = buildRevisionChangeRowsFromEvents({
        modelId: snapshot.modelId,
        revisionId: null,
        draftId,
        events,
      });

      if (changeRows.length > 0) {
        await tx
          .insert(revisionChanges)
          .values(changeRows as any);
      }

      const [savedDraftRows, savedChangeRows] = await Promise.all([
        tx
          .select()
          .from(modelRevisionDrafts)
          .where(eq(modelRevisionDrafts.id, draftId))
          .limit(1),
        tx
          .select()
          .from(revisionChanges)
          .where(eq(revisionChanges.draftId, draftId)),
      ]);

      return modelRevisionDraftMapper.toDomain(
        savedDraftRows[0],
        savedChangeRows,
      );
    });
  },

  async getBranchHead(modelId, branchName) {
    const rows = await getDb()
      .select()
      .from(modelBranchHeads)
      .where(
        and(
          eq(modelBranchHeads.modelId, modelId),
          eq(modelBranchHeads.branchName, branchName),
        ),
      )
      .limit(1);

    if (!rows[0]) {
      return undefined;
    }

    return modelBranchHeadMapper.toDomain(rows[0]);
  },

  async listBranchHeads(modelId) {
    const rows = await getDb()
      .select()
      .from(modelBranchHeads)
      .where(eq(modelBranchHeads.modelId, modelId));

    return rows.map((row) => modelBranchHeadMapper.toDomain(row));
  },

  async createBranch(input) {
    const aggregate = modelBranchHeadMapper.fromInput(input);
    const persistence = modelBranchHeadMapper.toPersistence(aggregate);

    await getDb()
      .insert(modelBranchHeads)
      .values({
        modelId: persistence.modelId,
        branchName: persistence.branchName,
        headRevisionId: persistence.headRevisionId,
      })
      .onConflictDoUpdate({
        target: [modelBranchHeads.modelId, modelBranchHeads.branchName],
        set: {
          headRevisionId: persistence.headRevisionId,
          updatedAt: new Date(),
        },
      });
  },

  async saveDraft(input) {
    const now = new Date();

    return getDb().transaction(async (tx) => {
      const existingRows = await tx
        .select()
        .from(modelRevisionDrafts)
        .where(eq(modelRevisionDrafts.id, input.id))
        .limit(1);

      const createdAt = existingRows[0]?.createdAt ?? now;

      const draftAggregate = ModelRevisionDraftAggregate.create({
        id: input.id,
        modelId: input.modelId,
        name: input.name,
        parentRevisionId: input.parentRevisionId ?? null,
        secondParentRevisionId: input.secondParentRevisionId ?? null,
        authorId: input.authorId,
        message: input.message,
        createdAt,
        updatedAt: now,
        nodes: input.nodes.map((node) => ({
          draftId: input.id,
          nodeId: node.nodeId,
          name: node.name,
          op: node.op === "upsert" ? "update" : (node.op ?? "update"),
        })),
      });

      const draftPersistence =
        modelRevisionDraftMapper.toPersistence(draftAggregate);

      await tx
        .insert(modelRevisionDrafts)
        .values(draftPersistence)
        .onConflictDoUpdate({
          target: modelRevisionDrafts.id,
          set: {
            modelId: draftPersistence.modelId,
            modelName: draftPersistence.modelName,
            parentRevisionId: draftPersistence.parentRevisionId,
            secondParentRevisionId: draftPersistence.secondParentRevisionId,
            authorId: draftPersistence.authorId,
            message: draftPersistence.message,
            updatedAt: sql`excluded.updated_at`,
          },
        });

      await tx
        .delete(revisionChanges)
        .where(eq(revisionChanges.draftId, input.id));

      const changeRows = draftAggregate.toSnapshot().nodes.map((node) => {
        const entity = RevisionChangeEntity.create({
          id: crypto.randomUUID(),
          revisionId: null,
          draftId: input.id,
          entityType: "node",
          entityId: node.nodeId,
          schemaVersion: 1,
          op: node.op,
          payload: {
            id: node.nodeId,
            modelId: input.modelId,
            name: node.name,
          },
          createdAt: now,
        });

        return revisionChangeMapper.toPersistence(entity);
      });

      if (changeRows.length > 0) {
        await tx
          .insert(revisionChanges)
          .values(changeRows as any);
      }

      const [savedDraftRows, savedChangeRows] = await Promise.all([
        tx
          .select()
          .from(modelRevisionDrafts)
          .where(eq(modelRevisionDrafts.id, input.id))
          .limit(1),
        tx
          .select()
          .from(revisionChanges)
          .where(eq(revisionChanges.draftId, input.id)),
      ]);

      return modelRevisionDraftMapper.toDomain(
        savedDraftRows[0],
        savedChangeRows,
      );
    });
  },

  async commitDraft(input) {
    return getDb().transaction(async (tx) => {
      const [draftRows, draftChangeRows] = await Promise.all([
        tx
          .select()
          .from(modelRevisionDrafts)
          .where(eq(modelRevisionDrafts.id, input.draftId))
          .limit(1),
        tx
          .select()
          .from(revisionChanges)
          .where(eq(revisionChanges.draftId, input.draftId)),
      ]);

      if (!draftRows[0]) {
        throw new Error("Model revision draft not found");
      }

      const draft = modelRevisionDraftMapper.toDomain(
        draftRows[0],
        draftChangeRows,
      );
      const draftSnapshot = draft.toSnapshot();

      const aggregate = modelRevisionMapper.fromCommitInput({
        id: draftSnapshot.id,
        modelId: draftSnapshot.modelId,
        name: draftSnapshot.name,
        parentRevisionId: draftSnapshot.parentRevisionId,
        secondParentRevisionId: draftSnapshot.secondParentRevisionId,
        authorId: draftSnapshot.authorId,
        message: draftSnapshot.message,
        nodes: draftSnapshot.nodes
          .filter((node) => node.op !== "delete")
          .map((node): NodeSnapshot => ({
            id: node.nodeId,
            modelId: draftSnapshot.modelId,
            nodeTypeDescriminator: "internal",
          })),
      });

      const snapshot = aggregate.toSnapshot();

      const revisionRow = await tx
        .insert(modelRevisions)
        .values(modelRevisionMapper.toPersistence(aggregate))
        .returning();

      const changeRows = buildRevisionChangeRowsFromNodeOps({
        modelId: snapshot.modelId,
        revisionId: snapshot.id,
        draftId: null,
        nodes: draftSnapshot.nodes.map((node) => ({
          nodeId: node.nodeId,
          name: node.name,
          op: node.op,
        })),
      });

      if (changeRows.length > 0) {
        await tx
          .insert(revisionChanges)
          .values(changeRows as any);
      }

      if (input.branchName) {
        const branchHead = modelBranchHeadMapper.fromInput({
          modelId: snapshot.modelId,
          branchName: input.branchName,
          headRevisionId: snapshot.id,
        });
        const branchPersistence =
          modelBranchHeadMapper.toPersistence(branchHead);

        await tx
          .insert(modelBranchHeads)
          .values({
            modelId: branchPersistence.modelId,
            branchName: branchPersistence.branchName,
            headRevisionId: branchPersistence.headRevisionId,
          })
          .onConflictDoUpdate({
            target: [modelBranchHeads.modelId, modelBranchHeads.branchName],
            set: {
              headRevisionId: branchPersistence.headRevisionId,
              updatedAt: new Date(),
            },
          });
      }

      await tx
        .delete(revisionChanges)
        .where(eq(revisionChanges.draftId, input.draftId));
      await tx
        .delete(modelRevisionDrafts)
        .where(eq(modelRevisionDrafts.id, input.draftId));

      const persistedChangeRows = await tx
        .select()
        .from(revisionChanges)
        .where(eq(revisionChanges.revisionId, snapshot.id));

      return modelRevisionMapper.toDomain(revisionRow[0], persistedChangeRows);
    });
  },

  async commitRevision(input) {
    const aggregate = modelRevisionMapper.fromCommitInput({
      id: input.id,
      modelId: input.modelId,
      name: input.name,
      parentRevisionId: input.parentRevisionId ?? null,
      secondParentRevisionId: input.secondParentRevisionId ?? null,
      authorId: input.authorId,
      message: input.message,
      nodes: input.nodes,
    });

    const snapshot = aggregate.toSnapshot();

    return getDb().transaction(async (tx) => {
      const revisionRow = await tx
        .insert(modelRevisions)
        .values(modelRevisionMapper.toPersistence(aggregate))
        .returning();

      if (input.includeModelChange) {
        const modelChangeRow = buildModelRevisionChangeRow({
          modelId: snapshot.modelId,
          modelName: snapshot.name,
          revisionId: snapshot.id,
          draftId: null,
          op: snapshot.parentRevisionId ? "update" : "insert",
        });
        await tx
          .insert(revisionChanges)
          .values(modelChangeRow as any);
      }

      if (snapshot.nodes.length > 0) {
        const changeRows = buildRevisionChangeRowsFromNodes({
          modelId: snapshot.modelId,
          revisionId: snapshot.id,
          draftId: null,
          nodes: snapshot.nodes,
        });

        if (changeRows.length > 0) {
          await tx
            .insert(revisionChanges)
            .values(changeRows as any);
        }
      }

      if (input.branchName) {
        const branchHead = modelBranchHeadMapper.fromInput({
          modelId: snapshot.modelId,
          branchName: input.branchName,
          headRevisionId: snapshot.id,
        });
        const branchPersistence =
          modelBranchHeadMapper.toPersistence(branchHead);

        await tx
          .insert(modelBranchHeads)
          .values({
            modelId: branchPersistence.modelId,
            branchName: branchPersistence.branchName,
            headRevisionId: branchPersistence.headRevisionId,
          })
          .onConflictDoUpdate({
            target: [modelBranchHeads.modelId, modelBranchHeads.branchName],
            set: {
              headRevisionId: branchPersistence.headRevisionId,
              updatedAt: new Date(),
            },
          });
      }

      const persistedChangeRows = await tx
        .select()
        .from(revisionChanges)
        .where(eq(revisionChanges.revisionId, snapshot.id));

      return modelRevisionMapper.toDomain(revisionRow[0], persistedChangeRows);
    });
  },

  async createRevisionAndUpdateBranchHead(input) {
    const branch = await drizzleModelVersionRepository.getBranchHead(input.modelId, input.branchName);
    if (!branch) {
      throw new Error(
        `Could not find branch ${input.branchName} on model with ID ${input.modelId}`,
      );
    }

    const parentRevision = await drizzleModelVersionRepository.getRevisionById(branch.headRevisionId);
    if (!parentRevision) {
      throw new Error(
        `Could not find parent revision with ID ${branch.headRevisionId}`,
      );
    }

    const parentSnapshot = parentRevision.toSnapshot();
    const newRevisionId = Bun.randomUUIDv7();

    const revision = await drizzleModelVersionRepository.commitRevision({
      id: newRevisionId,
      modelId: input.modelId,
      branchName: input.branchName,
      name: parentSnapshot.name,
      parentRevisionId: branch.headRevisionId,
      authorId: input.authorId,
      message: input.message,
      nodes: [],
    });

    return revision.toSnapshot().id;
  },
};

const buildRevisionChangeRowsFromNodeOps = (input: {
  modelId: string;
  revisionId: string | null;
  draftId: string | null;
  nodes: {
    nodeId: string;
    name?: string;
    nodeTypeDescriminator?: "external" | "internal";
    op: "insert" | "update" | "delete";
  }[];
}): (typeof revisionChanges.$inferInsert)[] => {
  const now = new Date();

  return input.nodes.map((node) => {
    const entity = RevisionChangeEntity.create({
      id: crypto.randomUUID(),
      revisionId: input.revisionId,
      draftId: input.draftId,
      entityType: "node",
      entityId: node.nodeId,
      schemaVersion: 1,
      op: node.op,
      payload: {
        id: node.nodeId,
        modelId: input.modelId,
        name: node.name ?? "",
        nodeTypeDescriminator: node.nodeTypeDescriminator ?? "internal",
      },
      createdAt: now,
    });

    return revisionChangeMapper.toPersistence(entity);
  });
};

const buildRevisionChangeRowsFromNodes = (input: {
  modelId: string;
  revisionId: string | null;
  draftId: string | null;
  nodes: NodeSnapshot[];
}): (typeof revisionChanges.$inferInsert)[] =>
  buildRevisionChangeRowsFromNodeOps({
    modelId: input.modelId,
    revisionId: input.revisionId,
    draftId: input.draftId,
    nodes: input.nodes.map((node) => ({
      nodeId: node.id,
      nodeTypeDescriminator: node.nodeTypeDescriminator,
      op: "update",
    })),
  });

const buildRevisionChangeRowsFromEvents = (input: {
  modelId: string;
  revisionId: string | null;
  draftId: string | null;
  events: DomainEvent[];
}): (typeof revisionChanges.$inferInsert)[] =>
  buildRevisionChangeRowsFromNodeOps({
    modelId: input.modelId,
    revisionId: input.revisionId,
    draftId: input.draftId,
    nodes: input.events.map((event) => ({
      nodeId: event.payload.id,
      name: extractNodeName(event.payload),
      nodeTypeDescriminator: extractNodeTypeDescriminator(event.payload),
      op: event.type === "node_deleted" ? "delete" : "insert",
    })),
  });

const buildModelRevisionChangeRow = (input: {
  modelId: string;
  modelName: string;
  revisionId: string | null;
  draftId: string | null;
  op: "insert" | "update";
}): typeof revisionChanges.$inferInsert => {
  const entity = RevisionChangeEntity.create({
    id: crypto.randomUUID(),
    revisionId: input.revisionId,
    draftId: input.draftId,
    entityType: "model",
    entityId: input.modelId,
    schemaVersion: 1,
    op: input.op,
    payload: {
      id: input.modelId,
      name: input.modelName,
    },
    createdAt: new Date(),
  });

  return revisionChangeMapper.toPersistence(entity);
};

const applyNodeChanges = (input: {
  modelId: string;
  current: Map<
    string,
    {
      id: string;
      modelId: string;
      name: string;
      nodeTypeDescriminator: "external" | "internal";
    }
  >;
  changes: {
    nodeId: string;
    name?: string;
    nodeTypeDescriminator?: "external" | "internal";
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
      modelId: input.modelId,
      name: change.name ?? "",
      nodeTypeDescriminator: change.nodeTypeDescriminator ?? "internal",
    });
  }
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
  modelId: string;
  revisions: (typeof modelRevisions.$inferSelect)[];
}): Promise<
  Map<
    string,
    {
      id: string;
      modelId: string;
      name: string;
      nodeTypeDescriminator: "external" | "internal";
    }
  >
> => {
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

  const nodesById = new Map<
    string,
    {
      id: string;
      modelId: string;
      name: string;
      nodeTypeDescriminator: "external" | "internal";
    }
  >();

  applyNodeChanges({
    modelId: input.modelId,
    current: nodesById,
    changes: changeRows
      .filter((change) => change.entityType === "node")
      .map((change) => ({
        nodeId: change.entityId,
        name: extractNodeName(change.payload),
        nodeTypeDescriminator: extractNodeTypeDescriminator(change.payload),
        op: change.op,
      })),
  });

  return nodesById;
};

const buildMaterialsFromRevisions = async (input: {
  revisions: (typeof modelRevisions.$inferSelect)[];
}): Promise<Map<string, MaterialSnapshot>> => {
  if (input.revisions.length === 0) {
    return new Map();
  }

  const revisionIds = input.revisions.map((revision) => revision.id);
  const revisionOrder = new Map(
    input.revisions.map((revision, index) => [revision.id, index]),
  );

  const rows = await getDb()
    .select()
    .from(materials)
    .where(inArray(materials.revisionId, revisionIds));

  rows.sort((a, b) => {
    const orderA = revisionOrder.get(a.revisionId) ?? 0;
    const orderB = revisionOrder.get(b.revisionId) ?? 0;
    if (orderA !== orderB) {
      return orderA - orderB;
    }
    return a.id.localeCompare(b.id);
  });

  const materialsById = new Map<string, MaterialSnapshot>();

  for (const row of rows) {
    materialsById.set(row.id, materialMapper.toDomain(row).toSnapshot());
  }

  return materialsById;
};

const buildSectionProfilesFromRevisions = async (input: {
  revisions: (typeof modelRevisions.$inferSelect)[];
}): Promise<Map<string, SectionProfileSnapshot>> => {
  if (input.revisions.length === 0) {
    return new Map();
  }

  const revisionIds = input.revisions.map((revision) => revision.id);
  const revisionOrder = new Map(
    input.revisions.map((revision, index) => [revision.id, index]),
  );

  const rows = await getDb()
    .select()
    .from(sectionProfiles)
    .where(inArray(sectionProfiles.revisionId, revisionIds));

  rows.sort((a, b) => {
    const orderA = revisionOrder.get(a.revisionId) ?? 0;
    const orderB = revisionOrder.get(b.revisionId) ?? 0;
    if (orderA !== orderB) {
      return orderA - orderB;
    }
    return a.id.localeCompare(b.id);
  });

  const sectionProfilesById = new Map<string, SectionProfileSnapshot>();

  for (const row of rows) {
    sectionProfilesById.set(
      row.id,
      sectionProfileMapper.toDomain(row).toSnapshot(),
    );
  }

  return sectionProfilesById;
};

const buildElement1dsFromRevisions = async (input: {
  revisions: (typeof modelRevisions.$inferSelect)[];
}): Promise<Map<string, Element1dSnapshot>> => {
  if (input.revisions.length === 0) {
    return new Map();
  }

  const revisionIds = input.revisions.map((revision) => revision.id);
  const revisionOrder = new Map(
    input.revisions.map((revision, index) => [revision.id, index]),
  );

  const rows = await getDb()
    .select()
    .from(element1ds)
    .where(inArray(element1ds.revisionId, revisionIds));

  rows.sort((a, b) => {
    const orderA = revisionOrder.get(a.revisionId) ?? 0;
    const orderB = revisionOrder.get(b.revisionId) ?? 0;
    if (orderA !== orderB) {
      return orderA - orderB;
    }
    return a.id.localeCompare(b.id);
  });

  const element1dsById = new Map<string, Element1dSnapshot>();

  for (const row of rows) {
    element1dsById.set(row.id, element1dMapper.toDomain(row).toSnapshot());
  }

  return element1dsById;
};

const extractNodeName = (payload: unknown): string => {
  if (payload && typeof payload === "object") {
    const name = (payload as { name?: unknown }).name;
    if (typeof name === "string") {
      return name;
    }
  }
  return "";
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
