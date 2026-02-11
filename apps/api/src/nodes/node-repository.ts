import { eq } from "drizzle-orm";
import { db } from "../db/client";
import { nodes } from "../db/schema";
import { nodeMapper } from "./node-mapper";
import type { NodeRepository } from "../services/types";

export const drizzleNodeRepository: NodeRepository = {
  async getById(id) {
    const row = await db.select().from(nodes).where(eq(nodes.id, id)).limit(1);
    if (!row[0]) {
      return undefined;
    }

    return nodeMapper.toDomain(row[0]);
  },
  async listByModelId(modelId) {
    const rows = await db.select().from(nodes).where(eq(nodes.modelId, modelId));
    return rows.map((row) => nodeMapper.toDomain(row));
  },
  async save(node) {
    const persistence = nodeMapper.toPersistence(node);
    const row = await db
      .insert(nodes)
      .values(persistence)
      .onConflictDoUpdate({
        target: nodes.id,
        set: {
          modelId: persistence.modelId,
          name: persistence.name,
        },
      })
      .returning();
    return nodeMapper.toDomain(row[0]);
  },
};
