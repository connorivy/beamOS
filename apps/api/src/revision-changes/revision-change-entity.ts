import { assertUuid } from "../common/uuid";

export type RevisionChangeSnapshot = {
  id: string;
  revisionId?: string | null;
  draftId?: string | null;
  entityType: string;
  entityId: string;
  schemaVersion: number;
  op: "insert" | "update" | "delete";
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
    this.assertSchemaVersion(snapshot.schemaVersion);
    this.assertOp(snapshot.op);

    this.id = snapshot.id;
    this.revisionId = snapshot.revisionId ?? null;
    this.draftId = snapshot.draftId ?? null;
    this.entityType = snapshot.entityType;
    this.entityId = snapshot.entityId;
    this.schemaVersion = snapshot.schemaVersion;
    this.op = snapshot.op;
    this._payload = snapshot.payload;
    this.createdAt = snapshot.createdAt;
  }

  readonly id: string;
  readonly revisionId: string | null;
  readonly draftId: string | null;
  readonly entityType: string;
  readonly entityId: string;
  readonly schemaVersion: number;
  readonly op: "insert" | "update" | "delete";
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
      schemaVersion: this.schemaVersion,
      op: this.op,
      payload: this._payload,
      createdAt: this.createdAt,
    };
  }

  private assertSchemaVersion(value: number): void {
    if (!Number.isInteger(value) || value < 1) {
      throw new Error("schemaVersion must be a positive integer");
    }
  }

  private assertRequired(value: string, field: string): void {
    if (value.trim().length === 0) {
      throw new Error(`${field} is required`);
    }
  }

  private assertOp(op: string): void {
    if (op !== "insert" && op !== "update" && op !== "delete") {
      throw new Error("Revision change op is invalid");
    }
  }
}
