import { assertUuid } from "../common/uuid";
import type { ModelBranchHeadAggregate } from "../model-branch-heads/model-branch-head-aggregate";
import type { ModelRevisionAggregate } from "../model-revisions/model-revision-aggregate";
import type { ProjectDomainEvent } from "./project-events";

export type ProjectSnapshot = {
  id: string;
  name: string;
  description: string;
};

export class ProjectEntity {
  private _name: string;
  private _description: string;
  private _modelBranchHeads: ModelBranchHeadAggregate[] | null;
  private _modelRevisions: ModelRevisionAggregate[] | null;
  private _domainEvents: ProjectDomainEvent[];

  private constructor(snapshot: {
    id: string;
    name: string;
    description?: string;
    modelBranchHeads?: ModelBranchHeadAggregate[] | null;
    modelRevisions?: ModelRevisionAggregate[] | null;
  }) {
    assertUuid(snapshot.id, "id");
    this.assertName(snapshot.name);

    this.id = snapshot.id;
    this._name = snapshot.name.trim();
    this._description = snapshot.description?.trim() ?? "";
    this._modelBranchHeads = snapshot.modelBranchHeads ?? null;
    this._modelRevisions = snapshot.modelRevisions ?? null;
    this._domainEvents = [];
  }

  readonly id: string;

  static create(snapshot: {
    id?: string;
    name: string;
    description?: string;
    modelBranchHeads?: ModelBranchHeadAggregate[] | null;
    modelRevisions?: ModelRevisionAggregate[] | null;
  }): ProjectEntity {
    const model = new ProjectEntity({
      id: snapshot.id ?? Bun.randomUUIDv7(),
      name: snapshot.name,
      description: snapshot.description ?? "",
      modelBranchHeads: snapshot.modelBranchHeads ?? null,
      modelRevisions: snapshot.modelRevisions ?? null,
    });
    model._domainEvents.push({
      type: "project_created",
      payload: model.toSnapshot(),
    });
    return model;
  }

  static rehydrate(snapshot: {
    id: string;
    name: string;
    description?: string;
    modelBranchHeads?: ModelBranchHeadAggregate[] | null;
    modelRevisions?: ModelRevisionAggregate[] | null;
  }): ProjectEntity {
    return new ProjectEntity({
      id: snapshot.id,
      name: snapshot.name,
      description: snapshot.description ?? "",
      modelBranchHeads: snapshot.modelBranchHeads ?? null,
      modelRevisions: snapshot.modelRevisions ?? null,
    });
  }

  get name(): string {
    return this._name;
  }

  get description(): string {
    return this._description;
  }

  get modelBranchHeads(): readonly ModelBranchHeadAggregate[] | null {
    return this._modelBranchHeads;
  }

  get modelRevisions(): readonly ModelRevisionAggregate[] | null {
    return this._modelRevisions;
  }

  rename(name: string): void {
    this.assertName(name);
    const next = name.trim();
    if (next === this._name) {
      return;
    }

    this._name = next;
    this._domainEvents.push({
      type: "project_renamed",
      payload: this.toSnapshot(),
    });
  }

  toSnapshot(): ProjectSnapshot {
    return {
      id: this.id,
      name: this._name,
      description: this._description,
    };
  }

  pullDomainEvents(): ProjectDomainEvent[] {
    const events = [...this._domainEvents];
    this._domainEvents = [];
    return events;
  }

  private assertName(name: string): void {
    if (name.trim().length === 0) {
      throw new Error("Project name is required");
    }
  }
}
