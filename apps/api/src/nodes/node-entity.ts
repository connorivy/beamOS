import { assertUuid } from "../common/uuid";
import { Ratio } from "unitsnet-js";
import { CreateNodeRequest, PutNodeRequest } from "./node-contract-schemas";

export type NodeRestraint = {
    canTranslateAlongX: boolean;
    canTranslateAlongY: boolean;
    canTranslateAlongZ: boolean;
    canRotateAboutX: boolean;
    canRotateAboutY: boolean;
    canRotateAboutZ: boolean;
};

export const NodeRestraints = {
    FREE: {
        canTranslateAlongX: true,
        canTranslateAlongY: true,
        canTranslateAlongZ: true,
        canRotateAboutX: true,
        canRotateAboutY: true,
        canRotateAboutZ: true,
    } as NodeRestraint,
    FIXED: {
        canTranslateAlongX: false,
        canTranslateAlongY: false,
        canTranslateAlongZ: false,
        canRotateAboutX: false,
        canRotateAboutY: false,
        canRotateAboutZ: false,
    } as NodeRestraint,
};

export const parseRestraint = (value: unknown): NodeRestraint => {
    if (!value || typeof value !== "object") {
        throw new Error("restraint must be a valid object");
    }

    const obj = value as Record<string, unknown>;

    if (
        typeof obj.canTranslateAlongX !== "boolean" ||
        typeof obj.canTranslateAlongY !== "boolean" ||
        typeof obj.canTranslateAlongZ !== "boolean" ||
        typeof obj.canRotateAboutX !== "boolean" ||
        typeof obj.canRotateAboutY !== "boolean" ||
        typeof obj.canRotateAboutZ !== "boolean"
    ) {
        throw new Error("restraint must have all required boolean properties");
    }

    return {
        canTranslateAlongX: obj.canTranslateAlongX,
        canTranslateAlongY: obj.canTranslateAlongY,
        canTranslateAlongZ: obj.canTranslateAlongZ,
        canRotateAboutX: obj.canRotateAboutX,
        canRotateAboutY: obj.canRotateAboutY,
        canRotateAboutZ: obj.canRotateAboutZ,
    };
};

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
    private constructor(snapshot: CreateNodeRequest, revisionId: string, id?: string) {
        id = id ?? Bun.randomUUIDv7();
        assertUuid(id, "id");

        this.id = id;
        this.modelRevisionId = revisionId;
        this.restraint = snapshot.restraint ?? NodeRestraints.FREE;

        this.nodeType = "spatialNode";
        this.location = snapshot.location;
    }

    readonly id: string;
    readonly modelRevisionId: string;
    readonly nodeType: "spatialNode" | "internalNode";
    readonly location:
        | { type: "spatial"; point: NodePoint }
        | { type: "internal"; element1dId: string; ratioAlongElement1d: number };
    readonly point: NodePoint | undefined;
    readonly element1dId: string | undefined;
    readonly distanceAlongElement1d: Ratio | undefined;
    readonly restraint: NodeRestraint;

    static create(snapshot: CreateNodeRequest, revisionId: string, id?: string): NodeEntity {
        return new NodeEntity(snapshot, revisionId, id);
    }

    static rehydrate(snapshot: PutNodeRequest, revisionId: string): NodeEntity {
        return new NodeEntity(snapshot, revisionId, snapshot.id);
    }

    toSnapshot(): NodeSnapshot {
        if (this.nodeType === "internalNode") {
            return {
                id: this.id,
                modelRevisionId: this.modelRevisionId,
                nodeType: "internalNode",
                element1dId: this.element1dId as string,
                distanceAlongElement1d: this.distanceAlongElement1d as Ratio,
                restraint: { ...this.restraint },
            };
        }

        return {
            id: this.id,
            modelRevisionId: this.modelRevisionId,
            nodeType: "spatialNode",
            point: this.point as NodePoint,
            restraint: { ...this.restraint },
        };
    }
}
