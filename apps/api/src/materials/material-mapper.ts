import { MaterialEntity } from "./material-entity";

export type MaterialPersistence = {
  id: string;
  revisionId: string;
  name: string;
  pressureESi: number;
  pressureGSi: number;
};

export const materialMapper = {
  toDomain(row: MaterialPersistence): MaterialEntity {
    return MaterialEntity.rehydrate({
      id: row.id,
      revisionId: row.revisionId,
      name: row.name,
      pressureE: { value: row.pressureESi, unit: "Pascals" },
      pressureG: { value: row.pressureGSi, unit: "Pascals" },
    });
  },

  toPersistence(aggregate: MaterialEntity): MaterialPersistence {
    return {
      id: aggregate.id,
      revisionId: aggregate.revisionId,
      name: aggregate.name,
      pressureESi: aggregate.pressureE.Pascals,
      pressureGSi: aggregate.pressureG.Pascals,
    };
  },
};
