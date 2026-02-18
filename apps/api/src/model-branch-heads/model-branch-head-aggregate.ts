import { assertUuid } from "../common/uuid";
import type { ModelRevisionAggregate } from "../model-revisions/model-revision-aggregate";

export type ModelBranchHeadSnapshot = {
  projectId: string;
  branchName: string;
  headRevisionId: string;
  updatedAt: Date;
  headRevision?: ModelRevisionAggregate | null;
};

export class ModelBranchHeadAggregate {
  private _headRevisionId: string;
  private _updatedAt: Date;
  private _headRevision: ModelRevisionAggregate | null;

  private constructor(snapshot: ModelBranchHeadSnapshot) {
    assertUuid(snapshot.projectId, "projectId");
    this.assertRequired(snapshot.branchName, "branchName");
    assertUuid(snapshot.headRevisionId, "headRevisionId");

    this.projectId = snapshot.projectId;
    this.branchName = snapshot.branchName;
    this._headRevisionId = snapshot.headRevisionId;
    this._updatedAt = snapshot.updatedAt;
    this._headRevision = snapshot.headRevision ?? null;
  }

  readonly projectId: string;
  readonly branchName: string;

  static create(snapshot: ModelBranchHeadSnapshot): ModelBranchHeadAggregate {
    return new ModelBranchHeadAggregate(snapshot);
  }

  static rehydrate(
    snapshot: ModelBranchHeadSnapshot,
  ): ModelBranchHeadAggregate {
    return new ModelBranchHeadAggregate(snapshot);
  }

  get headRevisionId(): string {
    return this._headRevisionId;
  }

  get updatedAt(): Date {
    return this._updatedAt;
  }

  get headRevision(): ModelRevisionAggregate | null {
    return this._headRevision;
  }

  moveHead(nextRevisionId: string, at: Date = new Date()): void {
    assertUuid(nextRevisionId, "headRevisionId");
    this._headRevisionId = nextRevisionId;
    this._updatedAt = at;
    this._headRevision = null;
  }

  toSnapshot(): ModelBranchHeadSnapshot {
    return {
      projectId: this.projectId,
      branchName: this.branchName,
      headRevisionId: this._headRevisionId,
      updatedAt: this._updatedAt,
      headRevision: this._headRevision,
    };
  }

  private assertRequired(value: string, field: string): void {
    if (value.trim().length === 0) {
      throw new Error(`${field} is required`);
    }
  }
}
