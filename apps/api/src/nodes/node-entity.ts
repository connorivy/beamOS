import { assertUuid } from "../common/uuid";
import { Ratio } from "unitsnet-js";
import type { NodeDomainEvents } from "./node-events";

export type NodeRestraint = Record<string, boolean>;

export type NodePoint = {
  x: number;
  y: number;
  z: number;
};

export type SpatialNodeSnapshot = {
  id: string;
  modelRevisionId: string;
  nodeType: "spatialNode";
  point: NodePoint;
  restraint: NodeRestraint;
};

export type InternalNodeSnapshot = {
  id: string;
  modelRevisionId: string;
  nodeType: "internalNode";
  element1dId: string;
  distanceAlongElement1d: Ratio;
  restraint: NodeRestraint;
};

type LegacyNodeSnapshot = {
  id: string;
  modelRevisionId?: string;
  restraint?: NodeRestraint;
  point?: Partial<NodePoint>;
  nodeType?: "spatialNode" | "internalNode";
  nodeTypeDescriminator?: "external" | "internal";
  element1dId?: string;
  distanceAlongElement1d?: Ratio;
};

export type NodeSnapshot = {
  id: string;
  modelRevisionId?: string;
  nodeType?: "spatialNode" | "internalNode";
  nodeTypeDescriminator?: "external" | "internal";
  point?: NodePoint;
  element1dId?: string;
  distanceAlongElement1d?: Ratio;
  restraint?: NodeRestraint;
};

export class NodeEntity {
  private _domainEvents: NodeDomainEvents[];

  private constructor(snapshot: LegacyNodeSnapshot) {
    assertUuid(snapshot.id, "id");

    const normalizedPoint = this.normalizePoint(snapshot.point);
    const normalizedRestraint = this.normalizeRestraint(snapshot.restraint);
    if (!snapshot.modelRevisionId) {
      throw new Error("modelRevisionId is required");
    }
    const modelRevisionId = snapshot.modelRevisionId;
    assertUuid(modelRevisionId, "modelRevisionId");

    this.id = snapshot.id;
    this.modelRevisionId = modelRevisionId;
    this.restraint = normalizedRestraint;
    this._domainEvents = [];

    const isInternalNode =
      snapshot.nodeType === "internalNode" ||
      snapshot.nodeTypeDescriminator === "internal";

    if (isInternalNode) {
      const element1dId = snapshot.element1dId ?? snapshot.id;
      assertUuid(element1dId, "element1dId");
      const distanceAlongElement1d =
        snapshot.distanceAlongElement1d ?? Ratio.FromDecimalFractions(0);
      this.assertRatio(distanceAlongElement1d, "distanceAlongElement1d");

      this.nodeType = "internalNode";
      this.nodeTypeDescriminator = "internal";
      this.element1dId = element1dId;
      this.distanceAlongElement1d = distanceAlongElement1d;
      this.point = undefined;
      return;
    }

    this.nodeType = "spatialNode";
    this.nodeTypeDescriminator = "external";
    this.point = normalizedPoint;
    this.element1dId = undefined;
    this.distanceAlongElement1d = undefined;
  }

  readonly id: string;
  readonly modelRevisionId: string;
  readonly nodeType: "spatialNode" | "internalNode";
  readonly nodeTypeDescriminator: "external" | "internal";
  readonly point: NodePoint | undefined;
  readonly element1dId: string | undefined;
  readonly distanceAlongElement1d: Ratio | undefined;
  readonly restraint: NodeRestraint;

  static create(snapshot: NodeSnapshot): NodeEntity {
    const entity = new NodeEntity(snapshot);
    entity._domainEvents.push({
      type: "node_created",
      payload: entity.toSnapshot(),
    });
    return entity;
  }

  static rehydrate(snapshot: NodeSnapshot): NodeEntity {
    return new NodeEntity(snapshot);
  }

  toSnapshot(): NodeSnapshot {
    if (this.nodeType === "internalNode") {
      return {
        id: this.id,
        modelRevisionId: this.modelRevisionId,
        nodeType: "internalNode",
        nodeTypeDescriminator: this.nodeTypeDescriminator,
        element1dId: this.element1dId as string,
        distanceAlongElement1d: this.distanceAlongElement1d as Ratio,
        restraint: { ...this.restraint },
      };
    }

    return {
      id: this.id,
      modelRevisionId: this.modelRevisionId,
      nodeType: "spatialNode",
      nodeTypeDescriminator: this.nodeTypeDescriminator,
      point: this.point as NodePoint,
      restraint: { ...this.restraint },
    };
  }

  pullDomainEvents(): NodeDomainEvents[] {
    const events = [...this._domainEvents];
    this._domainEvents = [];
    return events;
  }

  private assertRatio(value: Ratio, field: string): void {
    if (!(value instanceof Ratio) || !Number.isFinite(value.BaseValue)) {
      throw new Error(`${field} must be a finite Ratio`);
    }
  }

  private normalizePoint(point: Partial<NodePoint> | undefined): NodePoint {
    const normalized: NodePoint = {
      x: point?.x ?? 0,
      y: point?.y ?? 0,
      z: point?.z ?? 0,
    };
    this.assertPoint(normalized);
    return normalized;
  }

  private assertPoint(point: NodePoint): void {
    if (
      !Number.isFinite(point.x) ||
      !Number.isFinite(point.y) ||
      !Number.isFinite(point.z)
    ) {
      throw new Error("point must contain finite x, y, and z values");
    }
  }

  private normalizeRestraint(
    restraint: NodeRestraint | undefined,
  ): NodeRestraint {
    if (!restraint) {
      return {};
    }
    for (const value of Object.values(restraint)) {
      if (typeof value !== "boolean") {
        throw new Error("restraint must contain boolean values");
      }
    }
    return { ...restraint };
  }
}
