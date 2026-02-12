import { assertUuid } from "../lib/uuid";
import { NodeEntity, type NodeSnapshot } from "./node-entity";
import type { ModelDomainEvent } from "./model-events";

export type ModelSnapshot = {
  id: string;
  name: string;
  nodes: NodeSnapshot[];
};

export class ModelAggregate {
  private _name: string;
  private _nodes: NodeEntity[];
  private _domainEvents: ModelDomainEvent[];
  private _sourceRevisionId: string | null;
  private _sourceDraftId: string | null;

  private constructor(snapshot: {
    id: string;
    name: string;
    nodes: NodeSnapshot[];
    sourceRevisionId?: string | null;
    sourceDraftId?: string | null;
  }) {
    assertUuid(snapshot.id, "id");
    this.assertName(snapshot.name);

    this.id = snapshot.id;
    this._name = snapshot.name.trim();
    this._nodes = snapshot.nodes.map((node) => {
      this.assertNodeModelId(node.modelId);
      return NodeEntity.rehydrate(node);
    });
    this._domainEvents = [];
    this._sourceRevisionId = snapshot.sourceRevisionId ?? null;
    this._sourceDraftId = snapshot.sourceDraftId ?? null;
  }

  readonly id: string;

  static create(snapshot: {
    id: string;
    name: string;
    nodes?: NodeSnapshot[];
    sourceRevisionId?: string | null;
    sourceDraftId?: string | null;
  }): ModelAggregate {
    return new ModelAggregate({
      id: snapshot.id,
      name: snapshot.name,
      nodes: snapshot.nodes ?? [],
      sourceRevisionId: snapshot.sourceRevisionId ?? null,
      sourceDraftId: snapshot.sourceDraftId ?? null,
    });
  }

  static rehydrate(snapshot: {
    id: string;
    name: string;
    nodes?: NodeSnapshot[];
    sourceRevisionId?: string | null;
    sourceDraftId?: string | null;
  }): ModelAggregate {
    return new ModelAggregate({
      id: snapshot.id,
      name: snapshot.name,
      nodes: snapshot.nodes ?? [],
      sourceRevisionId: snapshot.sourceRevisionId ?? null,
      sourceDraftId: snapshot.sourceDraftId ?? null,
    });
  }

  get name(): string {
    return this._name;
  }

  get nodes(): readonly NodeEntity[] {
    return this._nodes;
  }

  get sourceRevisionId(): string | null {
    return this._sourceRevisionId;
  }

  get sourceDraftId(): string | null {
    return this._sourceDraftId;
  }

  rename(name: string): void {
    this.assertName(name);
    this._name = name.trim();
  }

  addNode(node: NodeSnapshot): void {
    this.assertNodeModelId(node.modelId);
    if (this._nodes.some((existing) => existing.id === node.id)) {
      throw new Error("Node already exists");
    }
    const entity = NodeEntity.create(node);
    this._nodes.push(entity);
    this._domainEvents.push({
      type: "node_added",
      node: entity.toSnapshot(),
    });
  }

  replaceNodes(nodes: NodeSnapshot[]): void {
    this._nodes = nodes.map((node) => {
      this.assertNodeModelId(node.modelId);
      return NodeEntity.rehydrate(node);
    });
  }

  toSnapshot(): ModelSnapshot {
    return {
      id: this.id,
      name: this._name,
      nodes: this._nodes.map((node) => node.toSnapshot()),
    };
  }

  pullDomainEvents(): ModelDomainEvent[] {
    const events = [...this._domainEvents];
    this._domainEvents = [];
    return events;
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
