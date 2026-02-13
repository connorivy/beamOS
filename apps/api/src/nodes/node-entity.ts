import { assertUuid } from "../common/uuid";

export type NodeSnapshot = {
  id: string;
  modelId: string;
  nodeTypeDescriminator: "external" | "internal";
};

export type NodeLocationDefinition = {};

export class NodeEntity {
  private constructor(snapshot: NodeSnapshot) {
    assertUuid(snapshot.id, "id");
    assertUuid(snapshot.modelId, "modelId");
    this.assertName(snapshot.name);

    this.id = snapshot.id;
    this.modelId = snapshot.modelId;
    this.name = snapshot.name.trim();
  }

  readonly id: string;
  readonly modelId: string;

  static create(snapshot: NodeSnapshot): NodeEntity {
    return new NodeEntity(snapshot);
  }

  static rehydrate(snapshot: NodeSnapshot): NodeEntity {
    return new NodeEntity(snapshot);
  }

  rename(name: string): void {
    this.assertName(name);
    this.name = name.trim();
  }

  toSnapshot(): NodeSnapshot {
    return {
      id: this.id,
      modelId: this.modelId,
      name: this.name,
    };
  }
}
