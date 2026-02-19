import { assertUuidV7 } from "../common/uuid";
import { Pressure, PressureUnits } from "unitsnet-js";
import type { MaterialDomainEvent } from "./material-events";
import type { z } from "zod";
import type { materialPropertiesSchema } from "./material-contract-schemas";

export type MaterialRevisionV1 = {
  id: string;
  revisionId: string;
  name: string;
  pressureE: { value: number; unit: string };
  pressureG: { value: number; unit: string };
};

export class MaterialEntity {
  private _domainEvents: MaterialDomainEvent[];

  private constructor(
    id: string,
    revisionId: string,
    name: string,
    pressureE: Pressure,
    pressureG: Pressure,
  ) {
    assertUuidV7(id, "id");
    assertUuidV7(revisionId, "revisionId");
    this.assertPressure(pressureE, "pressureE");
    this.assertPressure(pressureG, "pressureG");

    this.id = id;
    this.revisionId = revisionId;
    this.name = name;
    this.pressureE = pressureE;
    this.pressureG = pressureG;
    this._domainEvents = [];
  }

  readonly id: string;
  readonly revisionId: string;
  readonly name: string;
  readonly pressureE: Pressure;
  readonly pressureG: Pressure;

  static create(
    input: z.infer<typeof materialPropertiesSchema> & {
      id: string;
      revisionId: string;
    },
  ): MaterialEntity {
    const entity = new MaterialEntity(
      input.id,
      input.revisionId,
      input.name,
      new Pressure(input.modulusOfElasticity, input.units.pressure),
      new Pressure(input.modulusOfRigidity, input.units.pressure),
    );
    entity._domainEvents.push({
      type: "material_created",
      payload: entity.id,
    });
    return entity;
  }

  static rehydrate(v1: MaterialRevisionV1): MaterialEntity {
    return new MaterialEntity(
      v1.id,
      v1.revisionId,
      v1.name,
      Pressure.FromPascals(v1.pressureE.value),
      Pressure.FromPascals(v1.pressureG.value),
    );
  }

  toRevisionV1(): MaterialRevisionV1 {
    return {
      id: this.id,
      revisionId: this.revisionId,
      name: this.name,
      pressureE: { value: this.pressureE.Pascals, unit: PressureUnits.Pascals },
      pressureG: { value: this.pressureG.Pascals, unit: PressureUnits.Pascals },
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
