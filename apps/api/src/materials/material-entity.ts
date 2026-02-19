import { assertUuidV7 } from "../common/uuid";
import { Pressure } from "unitsnet-js";
import { CreateMaterialRequest } from "./material-contract-schemas";

export type MaterialSnapshot = {
    id: string;
    revisionId: string;
    name: string;
    pressureE: Pressure;
    pressureG: Pressure;
};

export class MaterialEntity {
    private constructor(snapshot: CreateMaterialRequest, revisionId: string, id?: string) {
        id = id ?? Bun.randomUUIDv7();
        assertUuidV7(id, "id");
        assertUuidV7(revisionId, "revisionId");
        this.modulusOfElasticity = new Pressure(
            snapshot.modulusOfElasticity,
            snapshot.units.pressure,
        );
        this.modulusOfRigidity = new Pressure(snapshot.modulusOfRigidity, snapshot.units.pressure);
        this.assertPressure(this.modulusOfElasticity, "modulusOfElasticity");
        this.assertPressure(this.modulusOfRigidity, "modulusOfRigidity");

        this.id = id;
        this.revisionId = revisionId;
        this.name = snapshot.name;
    }

    readonly id: string;
    readonly revisionId: string;
    readonly name: string;
    readonly modulusOfElasticity: Pressure;
    readonly modulusOfRigidity: Pressure;

    static create(
        snapshot: CreateMaterialRequest,
        revisionId: string,
        id?: string,
    ): MaterialEntity {
        return new MaterialEntity(snapshot, revisionId, id);
    }

    static rehydrate(
        snapshot: CreateMaterialRequest,
        revisionId: string,
        id: string,
    ): MaterialEntity {
        return new MaterialEntity(snapshot, revisionId, id);
    }

    toSnapshot(): MaterialSnapshot {
        return {
            id: this.id,
            revisionId: this.revisionId,
            name: this.name,
            pressureE: this.modulusOfElasticity,
            pressureG: this.modulusOfRigidity,
        };
    }

    private assertPressure(value: Pressure, field: string): void {
        if (!(value instanceof Pressure) || !Number.isFinite(value.BaseValue)) {
            throw new Error(`${field} must be a finite Pressure`);
        }
    }
}
