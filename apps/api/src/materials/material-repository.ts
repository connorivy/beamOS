import { and, desc, eq } from "drizzle-orm";
import { getDb } from "../db/client";
import type { DbTransaction } from "../db/client";
import { revisionChanges } from "../db/schema";
import { MaterialEntity } from "./material-entity";
import { RevisionChangeEntity } from "../revision-changes/revision-change-entity";
import { revisionChangeMapper } from "../revision-changes/revision-change-mapper";
import { PressureUnits } from "unitsnet-js";
import { z } from "zod";

const materialPayloadSchema = z.object({
  id: z.uuid(),
  revisionId: z.uuid(),
  name: z.string(),
  pressureE: z.object({
    value: z.number().finite(),
    unit: z.enum(PressureUnits),
  }),
  pressureG: z.object({
    value: z.number().finite(),
    unit: z.enum(PressureUnits),
  }),
});

export type MaterialRepository = {
  batchCreate: (
    tx: DbTransaction,
    input: MaterialEntity[],
  ) => Promise<MaterialEntity[]>;
  getById: (materialId: string) => Promise<MaterialEntity | undefined>;
};

export const drizzleMaterialRepository: MaterialRepository = {
  async batchCreate(tx, input) {
    if (input.length === 0) {
      return [];
    }

    const now = new Date();
    const changeRows = input.flatMap((material) => {
      const v1 = material.toRevisionV1();
      return material.pullDomainEvents().map(() =>
        revisionChangeMapper.toPersistence(
          RevisionChangeEntity.create({
            id: Bun.randomUUIDv7(),
            revisionId: v1.revisionId,
            entityType: "material",
            entityId: v1.id,
            schemaVersion: 1,
            op: "insert",
            payload: {
              id: v1.id,
              revisionId: v1.revisionId,
              name: v1.name,
              pressureE: v1.pressureE,
              pressureG: v1.pressureG,
            },
            createdAt: now,
          }),
        ),
      );
    });

    if (changeRows.length > 0) {
      await tx.insert(revisionChanges).values(changeRows).onConflictDoNothing();
    }

    return input;
  },

  async getById(materialId) {
    const rows = await getDb()
      .select()
      .from(revisionChanges)
      .where(
        and(
          eq(revisionChanges.entityType, "material"),
          eq(revisionChanges.entityId, materialId),
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

    const payload = materialPayloadSchema.parse(latestChange.payload);
    return MaterialEntity.rehydrate({
      id: payload.id,
      revisionId: payload.revisionId,
      name: payload.name,
      pressureE: { value: payload.pressureE.value, unit: payload.pressureE.unit },
      pressureG: { value: payload.pressureG.value, unit: payload.pressureG.unit },
    });
  },
};
