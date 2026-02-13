import { assertUuid } from "../common/uuid";
import {
  NodeRevisionDraftAggregate,
  type NodeRevisionDraftSnapshot,
} from "../node-revision-drafts/node-revision-draft-aggregate";

export type ModelRevisionDraftSnapshot = {
  id: string;
  modelId: string;
  name: string;
  parentRevisionId: string | null;
  secondParentRevisionId: string | null;
  authorId: string;
  message: string;
  createdAt: Date;
  updatedAt: Date;
  nodes: NodeRevisionDraftSnapshot[];
};

export class ModelRevisionDraftAggregate {
  private _name: string;
  private _parentRevisionId: string | null;
  private _secondParentRevisionId: string | null;
  private _authorId: string;
  private _message: string;
  private _createdAt: Date;
  private _updatedAt: Date;
  private _nodes: NodeRevisionDraftAggregate[];

  private constructor(snapshot: ModelRevisionDraftSnapshot) {
    assertUuid(snapshot.id, "id");
    assertUuid(snapshot.modelId, "modelId");
    this.assertRequired(snapshot.name, "name");
    assertUuid(snapshot.authorId, "authorId");
    this.assertRequired(snapshot.message, "message");
    this.assertOptionalUuid(snapshot.parentRevisionId, "parentRevisionId");
    this.assertOptionalUuid(
      snapshot.secondParentRevisionId,
      "secondParentRevisionId",
    );

    this.id = snapshot.id;
    this.modelId = snapshot.modelId;
    this._name = snapshot.name.trim();
    this._parentRevisionId = snapshot.parentRevisionId;
    this._secondParentRevisionId = snapshot.secondParentRevisionId;
    this._authorId = snapshot.authorId;
    this._message = snapshot.message.trim();
    this._createdAt = snapshot.createdAt;
    this._updatedAt = snapshot.updatedAt;
    this._nodes = snapshot.nodes.map((node) => {
      if (node.draftId !== snapshot.id) {
        throw new Error("Node does not belong to draft");
      }

      return NodeRevisionDraftAggregate.rehydrate(node);
    });
  }

  readonly id: string;
  readonly modelId: string;

  static create(
    snapshot: ModelRevisionDraftSnapshot,
  ): ModelRevisionDraftAggregate {
    return new ModelRevisionDraftAggregate(snapshot);
  }

  static rehydrate(
    snapshot: ModelRevisionDraftSnapshot,
  ): ModelRevisionDraftAggregate {
    return new ModelRevisionDraftAggregate(snapshot);
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

  get updatedAt(): Date {
    return this._updatedAt;
  }

  get nodes(): readonly NodeRevisionDraftAggregate[] {
    return this._nodes;
  }

  rename(name: string): void {
    this.assertRequired(name, "name");
    this._name = name.trim();
  }

  updateMessage(message: string): void {
    this.assertRequired(message, "message");
    this._message = message.trim();
  }

  replaceNodes(nodes: NodeRevisionDraftSnapshot[]): void {
    this._nodes = nodes.map((node) => {
      if (node.draftId !== this.id) {
        throw new Error("Node does not belong to draft");
      }

      return NodeRevisionDraftAggregate.rehydrate(node);
    });
  }

  touch(updatedAt = new Date()): void {
    this._updatedAt = updatedAt;
  }

  toSnapshot(): ModelRevisionDraftSnapshot {
    return {
      id: this.id,
      modelId: this.modelId,
      name: this._name,
      parentRevisionId: this._parentRevisionId,
      secondParentRevisionId: this._secondParentRevisionId,
      authorId: this._authorId,
      message: this._message,
      createdAt: this._createdAt,
      updatedAt: this._updatedAt,
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
