import { assertUuidV7 } from "../common/uuid";
import type { LoadCombinationDomainEvent } from "./load-combination-events";

export type LoadCombinationSnapshot = {
  id: string;
  revisionId: string;
  loadCaseFactors: Record<string, number>;
};

export class LoadCombinationEntity {
  private _domainEvents: LoadCombinationDomainEvent[];

  private constructor(snapshot: LoadCombinationSnapshot) {
    assertUuidV7(snapshot.id, "id");
    assertUuidV7(snapshot.revisionId, "revisionId");

    for (const [loadCaseId, factor] of Object.entries(snapshot.loadCaseFactors)) {
      assertUuidV7(loadCaseId, "loadCaseId");
      if (!Number.isFinite(factor)) {
        throw new Error("loadCaseFactors must contain finite numbers");
      }
    }

    this.id = snapshot.id;
    this.revisionId = snapshot.revisionId;
    this.loadCaseFactors = { ...snapshot.loadCaseFactors };
    this._domainEvents = [];
  }

  readonly id: string;
  readonly revisionId: string;
  readonly loadCaseFactors: Record<string, number>;

  static create(snapshot: LoadCombinationSnapshot): LoadCombinationEntity {
    const entity = new LoadCombinationEntity(snapshot);
    entity._domainEvents.push({
      type: "load_combination_created",
      payload: entity.toSnapshot(),
    });
    return entity;
  }

  static rehydrate(snapshot: LoadCombinationSnapshot): LoadCombinationEntity {
    return new LoadCombinationEntity(snapshot);
  }

  toSnapshot(): LoadCombinationSnapshot {
    return {
      id: this.id,
      revisionId: this.revisionId,
      loadCaseFactors: { ...this.loadCaseFactors },
    };
  }

  pullDomainEvents(): LoadCombinationDomainEvent[] {
    const events = [...this._domainEvents];
    this._domainEvents = [];
    return events;
  }
}
