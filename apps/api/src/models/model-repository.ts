import { and, eq, notInArray, sql } from "drizzle-orm";
import { db } from "../db/client";
import { models, nodes } from "../db/schema";
import { modelMapper } from "./model-mapper";
import type { ModelRepository } from "../services/types";
import { nodeMapper } from "../nodes/node-mapper";

export const drizzleModelRepository: ModelRepository = {
  async getById(id) {
    const [modelRows, nodeRows] = await Promise.all([
      db.select().from(models).where(eq(models.id, id)).limit(1),
      db.select().from(nodes).where(eq(nodes.modelId, id)),
    ]);

    if (!modelRows[0]) {
      return undefined;
    }

    return modelMapper.toDomain(modelRows[0], nodeRows);
  },

  async save(model) {
    const persistence = modelMapper.toPersistence(model);
    const nodePersistence = model.nodes.map((node) =>
      nodeMapper.toPersistence(node),
    );

    const savedModel = await db.transaction(async (tx) => {
      const row = await tx
        .insert(models)
        .values(persistence)
        .onConflictDoUpdate({
          target: models.id,
          set: {
            name: persistence.name,
          },
        })
        .returning();

      if (nodePersistence.length === 0) {
        await tx.delete(nodes).where(eq(nodes.modelId, model.id));
      } else {
        await tx.delete(nodes).where(
          and(
            eq(nodes.modelId, model.id),
            notInArray(
              nodes.id,
              nodePersistence.map((node) => node.id),
            ),
          ),
        );

        await tx
          .insert(nodes)
          .values(nodePersistence)
          .onConflictDoUpdate({
            target: nodes.id,
            set: {
              modelId: sql`excluded.model_id`,
              name: sql`excluded.name`,
            },
          });
      }

      const savedNodes = await tx
        .select()
        .from(nodes)
        .where(eq(nodes.modelId, model.id));

      return modelMapper.toDomain(row[0], savedNodes);
    });

    return savedModel;
  },
};
