import { assertUuid } from "../lib/uuid";

export type RevisionChangeSnapshot = {
  id: string;
  revisionId?: string | null;
  draftId?: string | null;
  entityType: string;
  entityId: string;
  op: "upsert" | "delete";
  payload: Record<string, unknown>;
  createdAt: Date;
};

export class RevisionChangeEntity {
  private _payload: Record<string, unknown>;

  private constructor(snapshot: RevisionChangeSnapshot) {
    assertUuid(snapshot.id, "id");
    if (snapshot.revisionId) {
      assertUuid(snapshot.revisionId, "revisionId");
    }
    if (snapshot.draftId) {
      assertUuid(snapshot.draftId, "draftId");
    }
    this.assertRequired(snapshot.entityType, "entityType");
    assertUuid(snapshot.entityId, "entityId");
    this.assertOp(snapshot.op);

    this.id = snapshot.id;
    this.revisionId = snapshot.revisionId ?? null;
    this.draftId = snapshot.draftId ?? null;
    this.entityType = snapshot.entityType;
    this.entityId = snapshot.entityId;
    this.op = snapshot.op;
    this._payload = snapshot.payload;
    this.createdAt = snapshot.createdAt;
  }

  readonly id: string;
  readonly revisionId: string | null;
  readonly draftId: string | null;
  readonly entityType: string;
  readonly entityId: string;
  readonly op: "upsert" | "delete";
  readonly createdAt: Date;

  static create(snapshot: RevisionChangeSnapshot): RevisionChangeEntity {
    return new RevisionChangeEntity(snapshot);
  }

  static rehydrate(snapshot: RevisionChangeSnapshot): RevisionChangeEntity {
    return new RevisionChangeEntity(snapshot);
  }

  get payload(): Record<string, unknown> {
    return this._payload;
  }

  toSnapshot(): RevisionChangeSnapshot {
    return {
      id: this.id,
      revisionId: this.revisionId,
      draftId: this.draftId,
      entityType: this.entityType,
      entityId: this.entityId,
      op: this.op,
      payload: this._payload,
      createdAt: this.createdAt,
    };
  }

  private assertRequired(value: string, field: string): void {
    if (value.trim().length === 0) {
      throw new Error(`${field} is required`);
    }
  }

  private assertOp(op: string): void {
    if (op !== "upsert" && op !== "delete") {
      throw new Error("Revision change op is invalid");
    }
  }
}
