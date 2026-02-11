import { and, eq } from "drizzle-orm";
import { db } from "../db/client";
import { modelBranchHeads, modelRevisionNodes, modelRevisions } from "../db/schema";
import { modelBranchHeadMapper } from "../model-branch-heads/model-branch-head-mapper";
import { ModelRevisionNodeEntity } from "../model-revision-nodes/model-revision-node-entity";
import { modelRevisionNodeMapper } from "../model-revision-nodes/model-revision-node-mapper";
import { modelRevisionMapper } from "./model-revision-mapper";
import type { ModelVersionRepository } from "../services/types";

export const drizzleModelVersionRepository: ModelVersionRepository = {
  async getRevisionById(revisionId) {
    const [revisionRows, nodeRows] = await Promise.all([
      db.select().from(modelRevisions).where(eq(modelRevisions.id, revisionId)).limit(1),
      db
        .select()
        .from(modelRevisionNodes)
        .where(eq(modelRevisionNodes.revisionId, revisionId)),
    ]);

    if (!revisionRows[0]) {
      return undefined;
    }

    return modelRevisionMapper.toDomain(revisionRows[0], nodeRows);
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
        const revisionNodeRows = snapshot.nodes.map((node) => {
          const entity = ModelRevisionNodeEntity.create({
            revisionId: snapshot.id,
            nodeId: node.id,
            name: node.name,
          });

          return modelRevisionNodeMapper.toPersistence(entity);
        });

        await tx.insert(modelRevisionNodes).values(revisionNodeRows);
      }

      if (input.branchName) {
        const branchHead = modelBranchHeadMapper.fromInput({
          modelId: snapshot.modelId,
          branchName: input.branchName,
          headRevisionId: snapshot.id,
        });
        const branchPersistence = modelBranchHeadMapper.toPersistence(branchHead);

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

      const persistedNodeRows = await tx
        .select()
        .from(modelRevisionNodes)
        .where(eq(modelRevisionNodes.revisionId, snapshot.id));

      return modelRevisionMapper.toDomain(revisionRow[0], persistedNodeRows);
    });
  },
};
