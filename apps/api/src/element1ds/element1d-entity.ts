import { assertUuidV7 } from "../common/uuid";

export type Element1dSnapshot = {
  id: string;
  revisionId: string;
  startNodeId: string;
  endNodeId: string;
  materialId: string;
  sectionProfileId: string;
};

export class Element1dEntity {
  private constructor(snapshot: Element1dSnapshot) {
    assertUuidV7(snapshot.id, "id");
    assertUuidV7(snapshot.revisionId, "revisionId");
    assertUuidV7(snapshot.startNodeId, "startNodeId");
    assertUuidV7(snapshot.endNodeId, "endNodeId");
    assertUuidV7(snapshot.materialId, "materialId");
    assertUuidV7(snapshot.sectionProfileId, "sectionProfileId");

    this.id = snapshot.id;
    this.revisionId = snapshot.revisionId;
    this.startNodeId = snapshot.startNodeId;
    this.endNodeId = snapshot.endNodeId;
    this.materialId = snapshot.materialId;
    this.sectionProfileId = snapshot.sectionProfileId;
  }

  readonly id: string;
  readonly revisionId: string;
  readonly startNodeId: string;
  readonly endNodeId: string;
  readonly materialId: string;
  readonly sectionProfileId: string;

  static create(snapshot: Element1dSnapshot): Element1dEntity {
    return new Element1dEntity(snapshot);
  }

  static rehydrate(snapshot: Element1dSnapshot): Element1dEntity {
    return new Element1dEntity(snapshot);
  }

  toSnapshot(): Element1dSnapshot {
    return {
      id: this.id,
      revisionId: this.revisionId,
      startNodeId: this.startNodeId,
      endNodeId: this.endNodeId,
      materialId: this.materialId,
      sectionProfileId: this.sectionProfileId,
    };
  }
}
