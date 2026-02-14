import { Element1dEntity } from "./element1d-entity";

export type Element1dPersistence = {
  id: string;
  revisionId: string;
  startNodeId: string;
  endNodeId: string;
  materialId: string;
  sectionProfileId: string;
};

export const element1dMapper = {
  toDomain(row: Element1dPersistence): Element1dEntity {
    return Element1dEntity.rehydrate({
      id: row.id,
      revisionId: row.revisionId,
      startNodeId: row.startNodeId,
      endNodeId: row.endNodeId,
      materialId: row.materialId,
      sectionProfileId: row.sectionProfileId,
    });
  },

  toPersistence(aggregate: Element1dEntity): Element1dPersistence {
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
