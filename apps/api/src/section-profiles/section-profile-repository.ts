import { eq, inArray } from "drizzle-orm";
import { getDb } from "../db/client";
import type { DbTransaction } from "../db/client";
import { sectionProfiles } from "../db/schema";
import { SectionProfileAggregate } from "./section-profile-aggregate";
import { sectionProfileMapper } from "./section-profile-mapper";

export type SectionProfileRepository = {
  batchCreate: (
    tx: DbTransaction,
    input: SectionProfileAggregate[],
  ) => Promise<SectionProfileAggregate[]>;
  getById: (
    sectionProfileId: string,
  ) => Promise<SectionProfileAggregate | undefined>;
};

export const drizzleSectionProfileRepository: SectionProfileRepository = {
  async batchCreate(tx, input) {
    if (input.length === 0) {
      return [];
    }

    await tx
      .insert(sectionProfiles)
      .values(input.map((sectionProfile) => sectionProfileMapper.toPersistence(sectionProfile)))
      .onConflictDoNothing();

    const ids = input.map((sectionProfile) => sectionProfile.id);
    const rows = await tx
      .select()
      .from(sectionProfiles)
      .where(inArray(sectionProfiles.id, ids));

    const byId = new Map(rows.map((row) => [row.id, row]));
    return input
      .map((sectionProfile) => byId.get(sectionProfile.id))
      .filter(
        (row): row is (typeof sectionProfiles.$inferSelect) => row !== undefined,
      )
      .map((row) => sectionProfileMapper.toDomain(row));
  },

  async getById(sectionProfileId) {
    const rows = await getDb()
      .select()
      .from(sectionProfiles)
      .where(eq(sectionProfiles.id, sectionProfileId))
      .limit(1);

    if (!rows[0]) {
      return undefined;
    }

    return sectionProfileMapper.toDomain(rows[0]);
  },
};
