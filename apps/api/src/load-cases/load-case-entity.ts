import { assertUuidV7 } from "../common/uuid";
import type { LoadCaseDomainEvent } from "./load-case-events";

export type LoadCaseSnapshot = {
  id: string;
  revisionId: string;
  name: string;
};

export class LoadCaseEntity {
  private _domainEvents: LoadCaseDomainEvent[];

  private constructor(snapshot: LoadCaseSnapshot) {
    assertUuidV7(snapshot.id, "id");
    assertUuidV7(snapshot.revisionId, "revisionId");
    if (snapshot.name.trim().length === 0) {
      throw new Error("name must be a non-empty string");
    }

    this.id = snapshot.id;
    this.revisionId = snapshot.revisionId;
    this.name = snapshot.name;
    this._domainEvents = [];
  }

  readonly id: string;
  readonly revisionId: string;
  readonly name: string;

  static create(snapshot: LoadCaseSnapshot): LoadCaseEntity {
    const entity = new LoadCaseEntity(snapshot);
    entity._domainEvents.push({
      type: "load_case_created",
      payload: entity.toSnapshot(),
    });
    return entity;
  }

  static rehydrate(snapshot: LoadCaseSnapshot): LoadCaseEntity {
    return new LoadCaseEntity(snapshot);
  }

  toSnapshot(): LoadCaseSnapshot {
    return {
      id: this.id,
      revisionId: this.revisionId,
      name: this.name,
    };
  }

  pullDomainEvents(): LoadCaseDomainEvent[] {
    const events = [...this._domainEvents];
    this._domainEvents = [];
    return events;
  }
}
