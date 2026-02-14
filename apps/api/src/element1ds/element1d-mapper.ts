import { element1ds, type Element1d as Element1dRow } from "../db/schema";
import { Element1dEntity } from "./element1d-entity";

export const element1dMapper = {
  toDomain(row: Element1dRow): Element1dEntity {
    return Element1dEntity.rehydrate({
      id: row.id,
      revisionId: row.revisionId,
      startNodeId: row.startNodeId,
      endNodeId: row.endNodeId,
      materialId: row.materialId,
      sectionProfileId: row.sectionProfileId,
    });
  },

  toPersistence(aggregate: Element1dEntity): typeof element1ds.$inferInsert {
    return {
      id: aggregate.id,
      revisionId: aggregate.revisionId,
      startNodeId: aggregate.startNodeId,
      endNodeId: aggregate.endNodeId,
      materialId: aggregate.materialId,
      sectionProfileId: aggregate.sectionProfileId,
    };
  },
};
