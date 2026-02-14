import { inArray } from "drizzle-orm";
import type { DbTransaction } from "../db/client";
import { revisionChanges } from "../db/schema";
import { revisionChangeMapper } from "./revision-change-mapper";
import { RevisionChangeEntity } from "./revision-change-entity";

export type RevisionChangeRepository = {
  batchCreate: (
    tx: DbTransaction,
    input: RevisionChangeEntity[],
  ) => Promise<RevisionChangeEntity[]>;
};

export const drizzleRevisionChangeRepository: RevisionChangeRepository = {
  async batchCreate(tx, input) {
    if (input.length === 0) {
      return [];
    }

    await tx
      .insert(revisionChanges)
      .values(input.map((change) => revisionChangeMapper.toPersistence(change)))
      .onConflictDoNothing();

    const ids = input.map((change) => change.id);
    const rows = await tx
      .select()
      .from(revisionChanges)
      .where(inArray(revisionChanges.id, ids));

    const byId = new Map(rows.map((row) => [row.id, row]));
    return input
      .map((change) => byId.get(change.id))
      .filter(
        (row): row is (typeof revisionChanges.$inferSelect) => row !== undefined,
      )
      .map((row) => revisionChangeMapper.toDomain(row));
  },
};
