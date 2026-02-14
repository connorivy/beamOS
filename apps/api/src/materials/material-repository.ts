import { and, desc, eq } from "drizzle-orm";
import { getDb } from "../db/client";
import type { DbTransaction } from "../db/client";
import { revisionChanges } from "../db/schema";
import { MaterialEntity } from "./material-entity";
import { RevisionChangeEntity } from "../revision-changes/revision-change-entity";
import { revisionChangeMapper } from "../revision-changes/revision-change-mapper";
import { Pressure, PressureUnits } from "unitsnet-js";
import { z } from "zod";

const materialPayloadSchema = z.object({
  id: z.uuid(),
  revisionId: z.uuid(),
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
    const changeRows = input.flatMap((material) =>
      material.pullDomainEvents().map((event) =>
        revisionChangeMapper.toPersistence(
          RevisionChangeEntity.create({
            id: Bun.randomUUIDv7(),
            revisionId: event.payload.revisionId,
            draftId: null,
            entityType: "material",
            entityId: event.payload.id,
            schemaVersion: 1,
            op: "insert",
            payload: {
              id: event.payload.id,
              revisionId: event.payload.revisionId,
              pressureE: {
                value: event.payload.pressureE.Pascals,
                unit: PressureUnits.Pascals,
              },
              pressureG: {
                value: event.payload.pressureG.Pascals,
                unit: PressureUnits.Pascals,
              },
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
      pressureE: new Pressure(payload.pressureE.value, payload.pressureE.unit),
      pressureG: new Pressure(payload.pressureG.value, payload.pressureG.unit),
    });
  },
};
