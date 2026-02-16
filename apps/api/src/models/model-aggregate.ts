import { assertUuid } from "../common/uuid";
import type { ModelBranchHeadAggregate } from "../model-branch-heads/model-branch-head-aggregate";
import { NodeEntity, type NodeSnapshot } from "../nodes/node-entity";
import type { ModelDomainEvent } from "./model-events";

export type ModelSnapshot = {
  id: string;
  name: string;
  description: string;
};

export class ModelAggregate {
  private _name: string;
  private _description: string;
  private _nodes: NodeEntity[];
  private _modelBranchHeads: ModelBranchHeadAggregate[] | null;
  private _domainEvents: ModelDomainEvent[];
  private _sourceRevisionId: string | null;

  private constructor(snapshot: {
    id: string;
    name: string;
    nodes: NodeSnapshot[];
    description?: string;
    modelBranchHeads?: ModelBranchHeadAggregate[] | null;
    sourceRevisionId?: string | null;
  }) {
    assertUuid(snapshot.id, "id");
    this.assertName(snapshot.name);

    this.id = snapshot.id;
    this._name = snapshot.name.trim();
    this._description = snapshot.description?.trim() ?? "";
    this._nodes = snapshot.nodes.map((node) => NodeEntity.rehydrate(node));
    this._modelBranchHeads = snapshot.modelBranchHeads ?? null;
    this._domainEvents = [];
    this._sourceRevisionId = snapshot.sourceRevisionId ?? null;
  }

  readonly id: string;

  static create(snapshot: {
    name: string;
    nodes?: NodeSnapshot[];
    description?: string;
    modelBranchHeads?: ModelBranchHeadAggregate[] | null;
    sourceRevisionId?: string | null;
  }): ModelAggregate {
    const model = new ModelAggregate({
      id: Bun.randomUUIDv7(),
      name: snapshot.name,
      nodes: snapshot.nodes ?? [],
      description: snapshot.description ?? "",
      modelBranchHeads: snapshot.modelBranchHeads ?? null,
      sourceRevisionId: snapshot.sourceRevisionId ?? null,
    });
    model._domainEvents.push({
      type: "model_created",
      payload: model.toSnapshot(),
    });
    return model;
  }

  static rehydrate(snapshot: {
    id: string;
    name: string;
    nodes?: NodeSnapshot[];
    description?: string;
    modelBranchHeads?: ModelBranchHeadAggregate[] | null;
    sourceRevisionId?: string | null;
  }): ModelAggregate {
    return new ModelAggregate({
      id: snapshot.id,
      name: snapshot.name,
      nodes: snapshot.nodes ?? [],
      description: snapshot.description ?? "",
      modelBranchHeads: snapshot.modelBranchHeads ?? null,
      sourceRevisionId: snapshot.sourceRevisionId ?? null,
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

  get description(): string {
    return this._description;
  }

  get modelBranchHeads(): readonly ModelBranchHeadAggregate[] | null {
    return this._modelBranchHeads;
  }

  rename(name: string): void {
    this.assertName(name);
    const next = name.trim();
    if (next === this._name) {
      return;
    }

    this._name = next;
    this._domainEvents.push({
      type: "model_renamed",
      payload: this.toSnapshot(),
    });
  }

  addNode(node: NodeSnapshot): void {
    if (this._nodes.some((existing) => existing.id === node.id)) {
      throw new Error("Node already exists");
    }
    const entity = NodeEntity.create(node);
    this._nodes.push(entity);
    this._domainEvents.push({
      type: "node_added",
      payload: entity.toSnapshot(),
    });
  }

  updateNode(input: { nodeId: string }): void {
    const node = this._nodes.find((current) => current.id === input.nodeId);
    if (!node) {
      throw new Error("Node does not exist");
    }
  }

  replaceNodes(nodes: NodeSnapshot[]): void {
    this._nodes = nodes.map((node) => NodeEntity.rehydrate(node));
  }

  toSnapshot(): ModelSnapshot {
    return {
      id: this.id,
      name: this._name,
      description: this._description,
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

}
