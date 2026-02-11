import { NodeAggregate, type NodeSnapshot } from "../nodes/node-aggregate";
import { assertUuid } from "../lib/uuid";

export type ModelRevisionSnapshot = {
  id: string;
  modelId: string;
  name: string;
  parentRevisionId: string | null;
  secondParentRevisionId: string | null;
  authorId: string;
  message: string;
  createdAt: Date;
  nodes: NodeSnapshot[];
};

export class ModelRevisionAggregate {
  private _name: string;
  private _parentRevisionId: string | null;
  private _secondParentRevisionId: string | null;
  private _authorId: string;
  private _message: string;
  private _createdAt: Date;
  private _nodes: NodeAggregate[];

  private constructor(snapshot: ModelRevisionSnapshot) {
    assertUuid(snapshot.id, "id");
    assertUuid(snapshot.modelId, "modelId");
    this.assertRequired(snapshot.name, "name");
    assertUuid(snapshot.authorId, "authorId");
    this.assertRequired(snapshot.message, "message");
    this.assertOptionalUuid(snapshot.parentRevisionId, "parentRevisionId");
    this.assertOptionalUuid(snapshot.secondParentRevisionId, "secondParentRevisionId");

    this.id = snapshot.id;
    this.modelId = snapshot.modelId;
    this._name = snapshot.name.trim();
    this._parentRevisionId = snapshot.parentRevisionId;
    this._secondParentRevisionId = snapshot.secondParentRevisionId;
    this._authorId = snapshot.authorId;
    this._message = snapshot.message.trim();
    this._createdAt = snapshot.createdAt;
    this._nodes = snapshot.nodes.map((node) => {
      if (node.modelId !== snapshot.modelId) {
        throw new Error("Node does not belong to revision model");
      }

      return NodeAggregate.rehydrate(node);
    });
  }

  readonly id: string;
  readonly modelId: string;

  static create(snapshot: ModelRevisionSnapshot): ModelRevisionAggregate {
    return new ModelRevisionAggregate(snapshot);
  }

  static rehydrate(snapshot: ModelRevisionSnapshot): ModelRevisionAggregate {
    return new ModelRevisionAggregate(snapshot);
  }

  get name(): string {
    return this._name;
  }

  get parentRevisionId(): string | null {
    return this._parentRevisionId;
  }

  get secondParentRevisionId(): string | null {
    return this._secondParentRevisionId;
  }

  get authorId(): string {
    return this._authorId;
  }

  get message(): string {
    return this._message;
  }

  get createdAt(): Date {
    return this._createdAt;
  }

  get nodes(): readonly NodeAggregate[] {
    return this._nodes;
  }

  toSnapshot(): ModelRevisionSnapshot {
    return {
      id: this.id,
      modelId: this.modelId,
      name: this._name,
      parentRevisionId: this._parentRevisionId,
      secondParentRevisionId: this._secondParentRevisionId,
      authorId: this._authorId,
      message: this._message,
      createdAt: this._createdAt,
      nodes: this._nodes.map((node) => node.toSnapshot()),
    };
  }

  private assertRequired(value: string, field: string): void {
    if (value.trim().length === 0) {
      throw new Error(`${field} is required`);
    }
  }

  private assertOptionalUuid(value: string | null, field: string): void {
    if (value !== null) {
      assertUuid(value, field);
    }
  }
}
