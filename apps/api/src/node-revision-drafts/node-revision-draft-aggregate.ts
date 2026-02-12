import { assertUuid } from "../lib/uuid";

export type NodeRevisionDraftSnapshot = {
  draftId: string;
  nodeId: string;
  name: string;
  op: "upsert" | "delete";
};

export class NodeRevisionDraftAggregate {
  private _name: string;
  private _op: "upsert" | "delete";

  private constructor(snapshot: NodeRevisionDraftSnapshot) {
    assertUuid(snapshot.draftId, "draftId");
    assertUuid(snapshot.nodeId, "nodeId");
    if (snapshot.op === "upsert") {
      this.assertName(snapshot.name);
    }
    this.assertOp(snapshot.op);

    this.draftId = snapshot.draftId;
    this.nodeId = snapshot.nodeId;
    this._name = snapshot.name.trim();
    this._op = snapshot.op;
  }

  readonly draftId: string;
  readonly nodeId: string;

  static create(snapshot: NodeRevisionDraftSnapshot): NodeRevisionDraftAggregate {
    return new NodeRevisionDraftAggregate(snapshot);
  }

  static rehydrate(
    snapshot: NodeRevisionDraftSnapshot,
  ): NodeRevisionDraftAggregate {
    return new NodeRevisionDraftAggregate(snapshot);
  }

  get name(): string {
    return this._name;
  }

  get op(): "upsert" | "delete" {
    return this._op;
  }

  rename(name: string): void {
    this.assertName(name);
    this._name = name.trim();
  }

  toSnapshot(): NodeRevisionDraftSnapshot {
    return {
      draftId: this.draftId,
      nodeId: this.nodeId,
      name: this._name,
      op: this._op,
    };
  }

  private assertName(name: string): void {
    if (name.trim().length === 0) {
      throw new Error("Model revision draft node name is required");
    }
  }

  private assertOp(op: string): void {
    if (op !== "upsert" && op !== "delete") {
      throw new Error("Model revision draft node op is invalid");
    }
  }
}
