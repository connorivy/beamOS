import { NodeAggregate, type NodeSnapshot } from "../nodes/node-aggregate";
import { assertUuid } from "../lib/uuid";

export type ModelSnapshot = {
  id: string;
  name: string;
  nodes: NodeSnapshot[];
};

export class ModelAggregate {
  private _name: string;
  private _nodes: NodeAggregate[];

  private constructor(snapshot: ModelSnapshot) {
    assertUuid(snapshot.id, "id");
    this.assertName(snapshot.name);

    this.id = snapshot.id;
    this._name = snapshot.name.trim();
    this._nodes = snapshot.nodes.map((node) => {
      this.assertNodeModelId(node.modelId);
      return NodeAggregate.rehydrate(node);
    });
  }

  readonly id: string;

  static create(snapshot: {
    id: string;
    name: string;
    nodes?: NodeSnapshot[];
  }): ModelAggregate {
    return new ModelAggregate({
      id: snapshot.id,
      name: snapshot.name,
      nodes: snapshot.nodes ?? [],
    });
  }

  static rehydrate(snapshot: {
    id: string;
    name: string;
    nodes?: NodeSnapshot[];
  }): ModelAggregate {
    return new ModelAggregate({
      id: snapshot.id,
      name: snapshot.name,
      nodes: snapshot.nodes ?? [],
    });
  }

  get name(): string {
    return this._name;
  }

  get nodes(): readonly NodeAggregate[] {
    return this._nodes;
  }

  rename(name: string): void {
    this.assertName(name);
    this._name = name.trim();
  }

  replaceNodes(nodes: NodeSnapshot[]): void {
    this._nodes = nodes.map((node) => {
      this.assertNodeModelId(node.modelId);
      return NodeAggregate.rehydrate(node);
    });
  }

  toSnapshot(): ModelSnapshot {
    return {
      id: this.id,
      name: this._name,
      nodes: this._nodes.map((node) => node.toSnapshot()),
    };
  }

  private assertName(name: string): void {
    if (name.trim().length === 0) {
      throw new Error("Model name is required");
    }
  }

  private assertNodeModelId(nodeModelId: string): void {
    if (nodeModelId !== this.id) {
      throw new Error("Node does not belong to this model");
    }
  }
}
