import { and, desc, eq } from "drizzle-orm";
import { getDb } from "../db/client";
import type { DbTransaction } from "../db/client";
import { revisionChanges } from "../db/schema";
import { Element1dEntity } from "./element1d-entity";
import { RevisionChangeEntity } from "../revision-changes/revision-change-entity";
import { revisionChangeMapper } from "../revision-changes/revision-change-mapper";
import { z } from "zod";

const element1dPayloadSchema = z.object({
  id: z.uuid(),
  revisionId: z.uuid(),
  startNodeId: z.uuid(),
  endNodeId: z.uuid(),
  materialId: z.uuid(),
  sectionProfileId: z.uuid(),
});

export type Element1dRepository = {
  batchCreate: (
    tx: DbTransaction,
    input: Element1dEntity[],
  ) => Promise<Element1dEntity[]>;
  getById: (element1dId: string) => Promise<Element1dEntity | undefined>;
};

export const drizzleElement1dRepository: Element1dRepository = {
  async batchCreate(tx, input) {
    if (input.length === 0) {
      return [];
    }

    const now = new Date();
    const changeRows = input.flatMap((element1d) =>
      element1d.pullDomainEvents().map((event) =>
        revisionChangeMapper.toPersistence(
          RevisionChangeEntity.create({
            id: Bun.randomUUIDv7(),
            revisionId: event.payload.revisionId,
            draftId: null,
            entityType: "element1d",
            entityId: event.payload.id,
            schemaVersion: 1,
            op: "insert",
            payload: {
              id: event.payload.id,
              revisionId: event.payload.revisionId,
              startNodeId: event.payload.startNodeId,
              endNodeId: event.payload.endNodeId,
              materialId: event.payload.materialId,
              sectionProfileId: event.payload.sectionProfileId,
            },
            createdAt: now,
          }),
        ),
      ),
    );

    if (changeRows.length > 0) {
      await tx.insert(revisionChanges).values(changeRows).onConflictDoNothing();
    }

    return input;
  },

  async getById(element1dId) {
    const rows = await getDb()
      .select()
      .from(revisionChanges)
      .where(
        and(
          eq(revisionChanges.entityType, "element1d"),
          eq(revisionChanges.entityId, element1dId),
        ),
      )
      .orderBy(desc(revisionChanges.createdAt), desc(revisionChanges.id))
      .limit(1);

    if (!rows[0]) {
      return undefined;
    }

    const latestChange = rows[0];
    if (latestChange.op === "delete") {
      return undefined;
    }

    const payload = element1dPayloadSchema.parse(latestChange.payload);
    return Element1dEntity.rehydrate({
      id: payload.id,
      revisionId: payload.revisionId,
      startNodeId: payload.startNodeId,
      endNodeId: payload.endNodeId,
      materialId: payload.materialId,
      sectionProfileId: payload.sectionProfileId,
    });
  },
};
