import { assertUuid } from "../lib/uuid";

export type ModelRevisionNodeSnapshot = {
  revisionId: string;
  nodeId: string;
  name: string;
};

export class ModelRevisionNodeEntity {
  private _name: string;

  private constructor(snapshot: ModelRevisionNodeSnapshot) {
    assertUuid(snapshot.revisionId, "revisionId");
    assertUuid(snapshot.nodeId, "nodeId");
    this.assertName(snapshot.name);

    this.revisionId = snapshot.revisionId;
    this.nodeId = snapshot.nodeId;
    this._name = snapshot.name.trim();
  }

  readonly revisionId: string;
  readonly nodeId: string;

  static create(snapshot: ModelRevisionNodeSnapshot): ModelRevisionNodeEntity {
    return new ModelRevisionNodeEntity(snapshot);
  }

  static rehydrate(snapshot: ModelRevisionNodeSnapshot): ModelRevisionNodeEntity {
    return new ModelRevisionNodeEntity(snapshot);
  }

  get name(): string {
    return this._name;
  }

  rename(name: string): void {
    this.assertName(name);
    this._name = name.trim();
  }

  toSnapshot(): ModelRevisionNodeSnapshot {
    return {
      revisionId: this.revisionId,
      nodeId: this.nodeId,
      name: this._name,
    };
  }

  private assertName(name: string): void {
    if (name.trim().length === 0) {
      throw new Error("Model revision node name is required");
    }
  }
}
