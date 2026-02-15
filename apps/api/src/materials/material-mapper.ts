import { Pressure } from "unitsnet-js";
import { MaterialEntity } from "./material-entity";

export type MaterialPersistence = {
  id: string;
  revisionId: string;
  pressureESi: number;
  pressureGSi: number;
};

export const materialMapper = {
  toDomain(row: MaterialPersistence): MaterialEntity {
    return MaterialEntity.rehydrate({
      id: row.id,
      revisionId: row.revisionId,
      pressureE: Pressure.FromPascals(row.pressureESi),
      pressureG: Pressure.FromPascals(row.pressureGSi),
    });
  },

  toPersistence(aggregate: MaterialEntity): MaterialPersistence {
    return {
      id: aggregate.id,
      revisionId: aggregate.revisionId,
      pressureESi: aggregate.pressureE.Pascals,
      pressureGSi: aggregate.pressureG.Pascals,
    };
  },
};
