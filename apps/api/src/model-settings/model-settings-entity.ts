import { assertUuidV7 } from "../common/uuid";
import type { ModelSettingsDomainEvent } from "./model-settings-events";

export type ModelSettingsUnitsSnapshot = {
  pressure: string;
  area: string;
  areaMomentOfInertia: string;
  warpingMomentOfInertia: string;
  volume: string;
};

export type ModelSettingsSnapshot = {
  id: string;
  revisionId: string;
  units: ModelSettingsUnitsSnapshot;
  yAxisUp: boolean;
};

export class ModelSettingsEntity {
  private _domainEvents: ModelSettingsDomainEvent[];

  private constructor(snapshot: ModelSettingsSnapshot) {
    assertUuidV7(snapshot.id, "id");
    assertUuidV7(snapshot.revisionId, "revisionId");
    this.assertUnits(snapshot.units);

    this.id = snapshot.id;
    this.revisionId = snapshot.revisionId;
    this.units = snapshot.units;
    this.yAxisUp = snapshot.yAxisUp;
    this._domainEvents = [];
  }

  readonly id: string;
  readonly revisionId: string;
  readonly units: ModelSettingsUnitsSnapshot;
  readonly yAxisUp: boolean;

  static create(snapshot: ModelSettingsSnapshot): ModelSettingsEntity {
    const entity = new ModelSettingsEntity(snapshot);
    entity._domainEvents.push({
      type: "model_settings_set",
      payload: entity.toSnapshot(),
    });
    return entity;
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

  pullDomainEvents(): ModelSettingsDomainEvent[] {
    const events = [...this._domainEvents];
    this._domainEvents = [];
    return events;
  }

  private assertUnits(units: ModelSettingsUnitsSnapshot): void {
    if (
      !units.pressure ||
      !units.area ||
      !units.areaMomentOfInertia ||
      !units.warpingMomentOfInertia ||
      !units.volume
    ) {
      throw new Error("units must include all model entity unit types");
    }
  }
}
