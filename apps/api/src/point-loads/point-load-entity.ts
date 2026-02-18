import { assertUuidV7 } from "../common/uuid";
import { Force, Torque } from "unitsnet-js";
import type { PointLoadDomainEvent } from "./point-load-events";

export type Vector3d = {
  x: number;
  y: number;
  z: number;
};

export type Force3d = {
  forceAlongX: Force;
  forceAlongY: Force;
  forceAlongZ: Force;
  momentAboutX: Torque;
  momentAboutY: Torque;
  momentAboutZ: Torque;
};

export type PointLoadSnapshot = {
  id: string;
  revisionId: string;
  nodeId: string;
  loadCaseId: string;
  force: Force3d;
  direction: Vector3d;
};

export class PointLoadEntity {
  private _domainEvents: PointLoadDomainEvent[];

  private constructor(snapshot: PointLoadSnapshot) {
    assertUuidV7(snapshot.id, "id");
    assertUuidV7(snapshot.revisionId, "revisionId");
    assertUuidV7(snapshot.nodeId, "nodeId");
    assertUuidV7(snapshot.loadCaseId, "loadCaseId");
    this.assertForce(snapshot.force.forceAlongX, "force.forceAlongX");
    this.assertForce(snapshot.force.forceAlongY, "force.forceAlongY");
    this.assertForce(snapshot.force.forceAlongZ, "force.forceAlongZ");
    this.assertTorque(snapshot.force.momentAboutX, "force.momentAboutX");
    this.assertTorque(snapshot.force.momentAboutY, "force.momentAboutY");
    this.assertTorque(snapshot.force.momentAboutZ, "force.momentAboutZ");
    this.assertDirection(snapshot.direction);

    this.id = snapshot.id;
    this.revisionId = snapshot.revisionId;
    this.nodeId = snapshot.nodeId;
    this.loadCaseId = snapshot.loadCaseId;
    this.force = snapshot.force;
    this.direction = { ...snapshot.direction };
    this._domainEvents = [];
  }

  readonly id: string;
  readonly revisionId: string;
  readonly nodeId: string;
  readonly loadCaseId: string;
  readonly force: Force3d;
  readonly direction: Vector3d;

  static create(snapshot: PointLoadSnapshot): PointLoadEntity {
    const entity = new PointLoadEntity(snapshot);
    entity._domainEvents.push({
      type: "point_load_created",
      payload: entity.toSnapshot(),
    });
    return entity;
  }

  static rehydrate(snapshot: PointLoadSnapshot): PointLoadEntity {
    return new PointLoadEntity(snapshot);
  }

  toSnapshot(): PointLoadSnapshot {
    return {
      id: this.id,
      revisionId: this.revisionId,
      nodeId: this.nodeId,
      loadCaseId: this.loadCaseId,
      force: this.force,
      direction: { ...this.direction },
    };
  }

  pullDomainEvents(): PointLoadDomainEvent[] {
    const events = [...this._domainEvents];
    this._domainEvents = [];
    return events;
  }

  private assertForce(value: Force, field: string): void {
    if (!(value instanceof Force) || !Number.isFinite(value.BaseValue)) {
      throw new Error(`${field} must be a finite Force`);
    }
  }

  private assertTorque(value: Torque, field: string): void {
    if (!(value instanceof Torque) || !Number.isFinite(value.BaseValue)) {
      throw new Error(`${field} must be a finite Torque`);
    }
  }

  private assertDirection(direction: Vector3d): void {
    if (
      !Number.isFinite(direction.x) ||
      !Number.isFinite(direction.y) ||
      !Number.isFinite(direction.z)
    ) {
      throw new Error("direction must contain finite x, y, and z values");
    }
  }
}
