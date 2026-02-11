import { assertUuid } from "../lib/uuid";

export type NodeSnapshot = {
  id: string;
  modelId: string;
  name: string;
};

export class NodeAggregate {
  private _modelId: string;
  private _name: string;

  private constructor(snapshot: NodeSnapshot) {
    assertUuid(snapshot.id, "id");
    this.assertModelId(snapshot.modelId);
    this.assertName(snapshot.name);

    this.id = snapshot.id;
    this._modelId = snapshot.modelId;
    this._name = snapshot.name.trim();
  }

  readonly id: string;

  static create(snapshot: NodeSnapshot): NodeAggregate {
    return new NodeAggregate(snapshot);
  }

  static rehydrate(snapshot: NodeSnapshot): NodeAggregate {
    return new NodeAggregate(snapshot);
  }

  get modelId(): string {
    return this._modelId;
  }

  get name(): string {
    return this._name;
  }

  rename(name: string): void {
    this.assertName(name);
    this._name = name.trim();
  }

  moveToModel(modelId: string): void {
    this.assertModelId(modelId);
    this._modelId = modelId;
  }

  toSnapshot(): NodeSnapshot {
    return {
      id: this.id,
      modelId: this._modelId,
      name: this._name,
    };
  }

  private assertName(name: string): void {
    if (name.trim().length === 0) {
      throw new Error("Node name is required");
    }
  }

  private assertModelId(modelId: string): void {
    assertUuid(modelId, "modelId");
  }
}
