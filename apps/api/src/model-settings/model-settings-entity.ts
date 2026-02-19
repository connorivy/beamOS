import { assertUuidV7 } from "../common/uuid";
import {
    AreaMomentOfInertiaUnits,
    AreaUnits,
    PressureUnits,
    VolumeUnits,
    WarpingMomentOfInertiaUnits,
} from "unitsnet-js";

export type ModelSettingsUnitsSnapshot = {
    pressure: PressureUnits;
    area: AreaUnits;
    areaMomentOfInertia: AreaMomentOfInertiaUnits;
    warpingMomentOfInertia: WarpingMomentOfInertiaUnits;
    volume: VolumeUnits;
};

export type ModelSettingsSnapshot = {
    id: string;
    revisionId: string;
    units: ModelSettingsUnitsSnapshot;
    yAxisUp: boolean;
};

export class ModelSettingsEntity {
    private constructor(snapshot: ModelSettingsSnapshot) {
        assertUuidV7(snapshot.id, "id");
        assertUuidV7(snapshot.revisionId, "revisionId");

        this.id = snapshot.id;
        this.revisionId = snapshot.revisionId;
        this.units = snapshot.units;
        this.yAxisUp = snapshot.yAxisUp;
    }

    readonly id: string;
    readonly revisionId: string;
    readonly units: ModelSettingsUnitsSnapshot;
    readonly yAxisUp: boolean;

    static create(snapshot: ModelSettingsSnapshot): ModelSettingsEntity {
        return new ModelSettingsEntity(snapshot);
    }

    static rehydrate(snapshot: ModelSettingsSnapshot): ModelSettingsEntity {
        return new ModelSettingsEntity(snapshot);
    }

    toSnapshot(): ModelSettingsSnapshot {
        return {
            id: this.id,
            revisionId: this.revisionId,
            units: this.units,
            yAxisUp: this.yAxisUp,
        };
    }
}
