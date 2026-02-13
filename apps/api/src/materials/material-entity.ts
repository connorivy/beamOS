import { assertUuidV7 } from "../common/uuid";
import { Pressure } from "unitsnet-js";

export type MaterialSnapshot = {
  id: string;
  revisionId: string;
  pressureE: Pressure;
  pressureG: Pressure;
};

export class MaterialEntity {
  private constructor(snapshot: MaterialSnapshot) {
    assertUuidV7(snapshot.id, "id");
    assertUuidV7(snapshot.revisionId, "revisionId");
    this.assertPressure(snapshot.pressureE, "pressureE");
    this.assertPressure(snapshot.pressureG, "pressureG");

    this.id = snapshot.id;
    this.revisionId = snapshot.revisionId;
    this.pressureE = snapshot.pressureE;
    this.pressureG = snapshot.pressureG;
  }

  readonly id: string;
  readonly revisionId: string;
  readonly pressureE: Pressure;
  readonly pressureG: Pressure;

  static create(snapshot: MaterialSnapshot): MaterialEntity {
    return new MaterialEntity(snapshot);
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

  private assertPressure(value: Pressure, field: string): void {
    if (!(value instanceof Pressure) || !Number.isFinite(value.BaseValue)) {
      throw new Error(`${field} must be a finite Pressure`);
    }
  }
}
