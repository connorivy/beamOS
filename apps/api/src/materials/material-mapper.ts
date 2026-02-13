import { Pressure } from "unitsnet-js";
import { materials, type Material as MaterialRow } from "../db/schema";
import { MaterialEntity } from "./material-entity";

export const materialMapper = {
  toDomain(row: MaterialRow): MaterialEntity {
    return MaterialEntity.rehydrate({
      id: row.id,
      modelId: row.modelId,
      pressureE: Pressure.FromPascals(row.pressureESi),
      pressureG: Pressure.FromPascals(row.pressureGSi),
    });
  },

  toPersistence(aggregate: MaterialEntity): typeof materials.$inferInsert {
    return {
      id: aggregate.id,
      modelId: aggregate.modelId,
      pressureESi: aggregate.pressureE.Pascals,
      pressureGSi: aggregate.pressureG.Pascals,
    };
  },
};
