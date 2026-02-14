import { assertUuidV7 } from "../common/uuid";
import { Pressure } from "unitsnet-js";
import type { MaterialDomainEvent } from "./material-events";

export type MaterialSnapshot = {
  id: string;
  revisionId: string;
  pressureE: Pressure;
  pressureG: Pressure;
};

export class MaterialEntity {
  private _domainEvents: MaterialDomainEvent[];

  private constructor(snapshot: MaterialSnapshot) {
    assertUuidV7(snapshot.id, "id");
    assertUuidV7(snapshot.revisionId, "revisionId");
    this.assertPressure(snapshot.pressureE, "pressureE");
    this.assertPressure(snapshot.pressureG, "pressureG");

    this.id = snapshot.id;
    this.revisionId = snapshot.revisionId;
    this.pressureE = snapshot.pressureE;
    this.pressureG = snapshot.pressureG;
    this._domainEvents = [];
  }

  readonly id: string;
  readonly revisionId: string;
  readonly pressureE: Pressure;
  readonly pressureG: Pressure;

  static create(snapshot: MaterialSnapshot): MaterialEntity {
    const entity = new MaterialEntity(snapshot);
    entity._domainEvents.push({
      type: "material_created",
      payload: entity.toSnapshot(),
    });
    return entity;
  }

  static rehydrate(snapshot: MaterialSnapshot): MaterialEntity {
    return new MaterialEntity(snapshot);
  }

  toSnapshot(): MaterialSnapshot {
    return {
      id: this.id,
      revisionId: this.revisionId,
      pressureE: this.pressureE,
      pressureG: this.pressureG,
    };
  }

  pullDomainEvents(): MaterialDomainEvent[] {
    const events = [...this._domainEvents];
    this._domainEvents = [];
    return events;
  }

  private assertPressure(value: Pressure, field: string): void {
    if (!(value instanceof Pressure) || !Number.isFinite(value.BaseValue)) {
      throw new Error(`${field} must be a finite Pressure`);
    }
  }
}
