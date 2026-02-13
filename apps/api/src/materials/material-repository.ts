import { eq, inArray } from "drizzle-orm";
import { getDb } from "../db/client";
import { materials } from "../db/schema";
import { MaterialEntity } from "./material-entity";
import { materialMapper } from "./material-mapper";

export type MaterialRepository = {
  batchCreate: (input: MaterialEntity[]) => Promise<MaterialEntity[]>;
  getById: (materialId: string) => Promise<MaterialEntity | undefined>;
};

export const drizzleMaterialRepository: MaterialRepository = {
  async batchCreate(input) {
    if (input.length === 0) {
      return [];
    }

    await getDb()
      .insert(materials)
      .values(input.map((material) => materialMapper.toPersistence(material)))
      .onConflictDoNothing();

    const ids = input.map((material) => material.id);
    const rows = await getDb()
      .select()
      .from(materials)
      .where(inArray(materials.id, ids));

    const byId = new Map(rows.map((row) => [row.id, row]));
    return input
      .map((material) => byId.get(material.id))
      .filter((row): row is (typeof materials.$inferSelect) => row !== undefined)
      .map((row) => materialMapper.toDomain(row));
  },

  async getById(materialId) {
    const rows = await getDb()
      .select()
      .from(materials)
      .where(eq(materials.id, materialId))
      .limit(1);

    if (!rows[0]) {
      return undefined;
    }

    return materialMapper.toDomain(rows[0]);
  },
};
