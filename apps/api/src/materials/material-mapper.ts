import { Pressure } from "unitsnet-js";
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
            pressureE: Pressure.FromPascals(row.pressureESi),
            pressureG: Pressure.FromPascals(row.pressureGSi),
        });
    },

    toPersistence(aggregate: MaterialEntity): MaterialPersistence {
        return {
            id: aggregate.id,
            revisionId: aggregate.revisionId,
            name: aggregate.name,
            pressureESi: aggregate.modulusOfElasticity.Pascals,
            pressureGSi: aggregate.this.modulusOfRigidity.Pascals,
        };
    },
};
