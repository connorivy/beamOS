import { Pressure } from "unitsnet-js";
import { materials, type Material as MaterialRow } from "../db/schema";
import { MaterialEntity } from "./material-entity";

export const materialMapper = {
  toDomain(row: MaterialRow): MaterialEntity {
    return MaterialEntity.rehydrate({
      id: row.id,
      revisionId: row.revisionId,
      pressureE: Pressure.FromPascals(row.pressureESi),
      pressureG: Pressure.FromPascals(row.pressureGSi),
    });
  },

  toPersistence(aggregate: MaterialEntity): typeof materials.$inferInsert {
    return {
      id: aggregate.id,
      revisionId: aggregate.revisionId,
      pressureESi: aggregate.pressureE.Pascals,
      pressureGSi: aggregate.pressureG.Pascals,
    };
  },
};
