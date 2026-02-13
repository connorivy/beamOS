import { assertUuid } from "../common/uuid";

export type NodeSnapshot = {
  id: string;
  modelId: string;
  name: string;
};

export class NodeEntity {
  private _name: string;

  private constructor(snapshot: NodeSnapshot) {
    assertUuid(snapshot.id, "id");
    assertUuid(snapshot.modelId, "modelId");
    this.assertName(snapshot.name);

    this.id = snapshot.id;
    this.modelId = snapshot.modelId;
    this._name = snapshot.name.trim();
  }

  readonly id: string;
  readonly modelId: string;

  static create(snapshot: NodeSnapshot): NodeEntity {
    return new NodeEntity(snapshot);
  }

  static rehydrate(snapshot: NodeSnapshot): NodeEntity {
    return new NodeEntity(snapshot);
  }

  get name(): string {
    return this._name;
  }

  rename(name: string): void {
    this.assertName(name);
    this._name = name.trim();
  }

  toSnapshot(): NodeSnapshot {
    return {
      id: this.id,
      modelId: this.modelId,
      name: this._name,
    };
  }

  private assertName(name: string): void {
    if (name.trim().length === 0) {
      throw new Error("Node name is required");
    }
  }
}
