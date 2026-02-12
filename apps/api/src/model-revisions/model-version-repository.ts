import crypto from "crypto";
import { and, eq, sql } from "drizzle-orm";
import { db } from "../db/client";
import {
  modelBranchHeads,
  modelRevisionDrafts,
  modelRevisions,
  revisionChanges,
} from "../db/schema";
import { modelBranchHeadMapper } from "../model-branch-heads/model-branch-head-mapper";
import { modelRevisionDraftMapper } from "../model-revision-drafts/model-revision-draft-mapper";
import { ModelRevisionDraftAggregate } from "../model-revision-drafts/model-revision-draft-aggregate";
import { NodeRevisionAggregate } from "../node-revisions/node-revision-aggregate";
import { modelRevisionMapper } from "./model-revision-mapper";
import { RevisionChangeEntity } from "../revision-changes/revision-change-entity";
import { revisionChangeMapper } from "../revision-changes/revision-change-mapper";
import type { ModelRevisionRepository } from "../services/types";

export const drizzleModelVersionRepository: ModelRevisionRepository = {
  async getRevisionById(revisionId) {
    const [revisionRows, changeRows] = await Promise.all([
      db
        .select()
        .from(modelRevisions)
        .where(eq(modelRevisions.id, revisionId))
        .limit(1),
      db
        .select()
        .from(revisionChanges)
        .where(eq(revisionChanges.revisionId, revisionId)),
    ]);

    if (!revisionRows[0]) {
      return undefined;
    }

    return modelRevisionMapper.toDomain(revisionRows[0], changeRows);
  },

  async getDraftById(draftId) {
    const [draftRows, changeRows] = await Promise.all([
      db
        .select()
        .from(modelRevisionDrafts)
        .where(eq(modelRevisionDrafts.id, draftId))
        .limit(1),
      db
        .select()
        .from(revisionChanges)
        .where(eq(revisionChanges.draftId, draftId)),
    ]);

    if (!draftRows[0]) {
      return undefined;
    }

    return modelRevisionDraftMapper.toDomain(draftRows[0], changeRows);
  },

  async getBranchHead(modelId, branchName) {
    const rows = await db
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
    const rows = await db
      .select()
      .from(modelBranchHeads)
      .where(eq(modelBranchHeads.modelId, modelId));

    return rows.map((row) => modelBranchHeadMapper.toDomain(row));
  },

  async createBranch(input) {
    const aggregate = modelBranchHeadMapper.fromInput(input);
    const persistence = modelBranchHeadMapper.toPersistence(aggregate);

    await db
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

    return db.transaction(async (tx) => {
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
          op: node.op ?? "upsert",
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
        await tx.insert(revisionChanges).values(changeRows);
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
    return db.transaction(async (tx) => {
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
        nodes: draftSnapshot.nodes.map((node) => ({
          nodeId: node.nodeId,
          name: node.name,
          op: node.op,
        })),
      });

      const snapshot = aggregate.toSnapshot();

      const revisionRow = await tx
        .insert(modelRevisions)
        .values(modelRevisionMapper.toPersistence(aggregate))
        .returning();

      if (snapshot.nodes.length > 0) {
        const changeRows = snapshot.nodes.map((node) => {
          const entity = RevisionChangeEntity.create({
            id: crypto.randomUUID(),
            revisionId: snapshot.id,
            draftId: null,
            entityType: "node",
            entityId: node.nodeId,
            op: node.op,
            payload: {
              id: node.nodeId,
              modelId: snapshot.modelId,
              name: node.name,
            },
            createdAt: new Date(),
          });

          return revisionChangeMapper.toPersistence(entity);
        });

        await tx.insert(revisionChanges).values(changeRows);
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

    return db.transaction(async (tx) => {
      const revisionRow = await tx
        .insert(modelRevisions)
        .values(modelRevisionMapper.toPersistence(aggregate))
        .returning();

      if (snapshot.nodes.length > 0) {
        const changeRows = snapshot.nodes.map((node) => {
          const aggregateNode = NodeRevisionAggregate.create({
            revisionId: snapshot.id,
            nodeId: node.nodeId,
            name: node.name,
            op: node.op,
          });
          const nodeSnapshot = aggregateNode.toSnapshot();
          const entity = RevisionChangeEntity.create({
            id: crypto.randomUUID(),
            revisionId: snapshot.id,
            draftId: null,
            entityType: "node",
            entityId: nodeSnapshot.nodeId,
            op: nodeSnapshot.op,
            payload: {
              id: nodeSnapshot.nodeId,
              modelId: snapshot.modelId,
              name: nodeSnapshot.name,
            },
            createdAt: new Date(),
          });

          return revisionChangeMapper.toPersistence(entity);
        });

        await tx.insert(revisionChanges).values(changeRows);
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
};
