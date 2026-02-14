import { eq, inArray } from "drizzle-orm";
import { getDb } from "../db/client";
import { element1ds } from "../db/schema";
import { Element1dEntity } from "./element1d-entity";
import { element1dMapper } from "./element1d-mapper";

export type Element1dRepository = {
  batchCreate: (input: Element1dEntity[]) => Promise<Element1dEntity[]>;
  getById: (element1dId: string) => Promise<Element1dEntity | undefined>;
};

export const drizzleElement1dRepository: Element1dRepository = {
  async batchCreate(input) {
    if (input.length === 0) {
      return [];
    }

    await getDb()
      .insert(element1ds)
      .values(input.map((element1d) => element1dMapper.toPersistence(element1d)))
      .onConflictDoNothing();

    const ids = input.map((element1d) => element1d.id);
    const rows = await getDb()
      .select()
      .from(element1ds)
      .where(inArray(element1ds.id, ids));

    const byId = new Map(rows.map((row) => [row.id, row]));
    return input
      .map((element1d) => byId.get(element1d.id))
      .filter((row): row is (typeof element1ds.$inferSelect) => row !== undefined)
      .map((row) => element1dMapper.toDomain(row));
  },

  async getById(element1dId) {
    const rows = await getDb()
      .select()
      .from(element1ds)
      .where(eq(element1ds.id, element1dId))
      .limit(1);

    if (!rows[0]) {
      return undefined;
    }

    return element1dMapper.toDomain(rows[0]);
  },
};
