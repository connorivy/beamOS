import { assertUuid } from "../lib/uuid";

export type NodeRevisionSnapshot = {
  revisionId: string;
  nodeId: string;
  name: string;
  op: "insert" | "update" | "delete";
};

export class NodeRevisionAggregate {
  private _name: string;
  private _op: "insert" | "update" | "delete";

  private constructor(snapshot: NodeRevisionSnapshot) {
    assertUuid(snapshot.revisionId, "revisionId");
    assertUuid(snapshot.nodeId, "nodeId");
    if (snapshot.op !== "delete") {
      this.assertName(snapshot.name);
    }
    this.assertOp(snapshot.op);

    this.revisionId = snapshot.revisionId;
    this.nodeId = snapshot.nodeId;
    this._name = snapshot.name.trim();
    this._op = snapshot.op;
  }

  readonly revisionId: string;
  readonly nodeId: string;

  static create(snapshot: NodeRevisionSnapshot): NodeRevisionAggregate {
    return new NodeRevisionAggregate(snapshot);
  }

  static rehydrate(snapshot: NodeRevisionSnapshot): NodeRevisionAggregate {
    return new NodeRevisionAggregate(snapshot);
  }

  get name(): string {
    return this._name;
  }

  get op(): "insert" | "update" | "delete" {
    return this._op;
  }

  toSnapshot(): NodeRevisionSnapshot {
    return {
      revisionId: this.revisionId,
      nodeId: this.nodeId,
      name: this._name,
      op: this._op,
    };
  }

  private assertName(name: string): void {
    if (name.trim().length === 0) {
      throw new Error("Model revision node name is required");
    }
  }

  private assertOp(op: string): void {
    if (op !== "insert" && op !== "update" && op !== "delete") {
      throw new Error("Model revision node op is invalid");
    }
  }
}
