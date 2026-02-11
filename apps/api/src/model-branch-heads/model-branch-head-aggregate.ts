import { assertUuid } from "../lib/uuid";

export type ModelBranchHeadSnapshot = {
  modelId: string;
  branchName: string;
  headRevisionId: string;
  updatedAt: Date;
};

export class ModelBranchHeadAggregate {
  private _headRevisionId: string;
  private _updatedAt: Date;

  private constructor(snapshot: ModelBranchHeadSnapshot) {
    assertUuid(snapshot.modelId, "modelId");
    this.assertRequired(snapshot.branchName, "branchName");
    assertUuid(snapshot.headRevisionId, "headRevisionId");

    this.modelId = snapshot.modelId;
    this.branchName = snapshot.branchName;
    this._headRevisionId = snapshot.headRevisionId;
    this._updatedAt = snapshot.updatedAt;
  }

  readonly modelId: string;
  readonly branchName: string;

  static create(snapshot: ModelBranchHeadSnapshot): ModelBranchHeadAggregate {
    return new ModelBranchHeadAggregate(snapshot);
  }

  static rehydrate(snapshot: ModelBranchHeadSnapshot): ModelBranchHeadAggregate {
    return new ModelBranchHeadAggregate(snapshot);
  }

  get headRevisionId(): string {
    return this._headRevisionId;
  }

  get updatedAt(): Date {
    return this._updatedAt;
  }

  moveHead(nextRevisionId: string, at: Date = new Date()): void {
    assertUuid(nextRevisionId, "headRevisionId");
    this._headRevisionId = nextRevisionId;
    this._updatedAt = at;
  }

  toSnapshot(): ModelBranchHeadSnapshot {
    return {
      modelId: this.modelId,
      branchName: this.branchName,
      headRevisionId: this._headRevisionId,
      updatedAt: this._updatedAt,
    };
  }

  private assertRequired(value: string, field: string): void {
    if (value.trim().length === 0) {
      throw new Error(`${field} is required`);
    }
  }
}
