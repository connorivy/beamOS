import { DomainEvent } from "src/common/types";
import { assertUuid, assertUuidV7 } from "../common/uuid";
import { Element1dEntity, type Element1dSnapshot } from "../element1ds/element1d-entity";
import { MaterialEntity, type MaterialSnapshot } from "../materials/material-entity";
import { NodeEntity, type NodeSnapshot } from "../nodes/node-entity";
import {
  SectionProfileEntity,
  type SectionProfileSnapshot,
} from "../section-profiles/section-profile-entity";

export type ModelRevisionSnapshot = {
  id: string;
  modelId: string;
  branchName: string;
  name: string;
  parentRevisionId: string | null;
  secondParentRevisionId: string | null;
  authorId: string;
  message: string;
  createdAt: Date;
  nodes: NodeSnapshot[];
  materials: MaterialSnapshot[];
  sectionProfiles: SectionProfileSnapshot[];
  element1ds: Element1dSnapshot[];
};

export type ModelRevisionCreateSnapshot = Omit<ModelRevisionSnapshot, "id"> & {
  id?: string;
};

export const DEFAULT_MODEL_REVISION_BRANCH_NAME = "detached";

export class ModelRevisionAggregate {
  private _branchName: string;
  private _name: string;
  private _parentRevisionId: string | null;
  private _secondParentRevisionId: string | null;
  private _authorId: string;
  private _message: string;
  private _createdAt: Date;
  private _nodes: NodeEntity[];
  private _materials: MaterialEntity[];
  private _sectionProfiles: SectionProfileEntity[];
  private _element1ds: Element1dEntity[];
  private _domainEvents: DomainEvent[];

  private constructor(snapshot: ModelRevisionSnapshot | ModelRevisionCreateSnapshot) {
    const revisionId = snapshot.id ?? Bun.randomUUIDv7();
    assertUuidV7(revisionId, "id");
    assertUuid(snapshot.modelId, "modelId");
    this.assertRequired(snapshot.branchName, "branchName");
    this.assertRequired(snapshot.name, "name");
    assertUuid(snapshot.authorId, "authorId");
    this.assertRequired(snapshot.message, "message");
    this.assertOptionalUuid(snapshot.parentRevisionId, "parentRevisionId");
    this.assertOptionalUuid(
      snapshot.secondParentRevisionId,
      "secondParentRevisionId",
    );

    this.id = revisionId;
    this.modelId = snapshot.modelId;
    this._branchName = snapshot.branchName.trim();
    this._name = snapshot.name.trim();
    this._parentRevisionId = snapshot.parentRevisionId;
    this._secondParentRevisionId = snapshot.secondParentRevisionId;
    this._authorId = snapshot.authorId;
    this._message = snapshot.message.trim();
    this._createdAt = snapshot.createdAt;
    this._nodes = snapshot.nodes.map((node) => NodeEntity.rehydrate(node));
    this._materials = snapshot.materials.map((material) =>
      MaterialEntity.rehydrate(material),
    );
    this._sectionProfiles = snapshot.sectionProfiles.map((sectionProfile) =>
      SectionProfileEntity.rehydrate(sectionProfile),
    );
    this._element1ds = snapshot.element1ds.map((element1d) =>
      Element1dEntity.rehydrate(element1d),
    );
    this._domainEvents = [];
  }

  readonly id: string;
  readonly modelId: string;

  static create(snapshot: ModelRevisionCreateSnapshot): ModelRevisionAggregate {
    return new ModelRevisionAggregate(snapshot);
  }

  static rehydrate(snapshot: ModelRevisionSnapshot): ModelRevisionAggregate {
    return new ModelRevisionAggregate(snapshot);
  }

  get name(): string {
    return this._name;
  }

  get branchName(): string {
    return this._branchName;
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

  get nodes(): readonly NodeEntity[] {
    return this._nodes;
  }

  get materials(): readonly MaterialEntity[] {
    return this._materials;
  }

  get sectionProfiles(): readonly SectionProfileEntity[] {
    return this._sectionProfiles;
  }

  get element1ds(): readonly Element1dEntity[] {
    return this._element1ds;
  }

  addNode(node: NodeSnapshot): void {
    assertUuid(node.id, "nodeId");
    if (this._nodes.some((existing) => existing.id === node.id)) {
      throw new Error("Node already exists");
    }
    this._nodes.push(NodeEntity.create(node));
  }

  addMaterial(material: MaterialSnapshot): void {
    assertUuid(material.id, "materialId");
    if (this._materials.some((existing) => existing.id === material.id)) {
      throw new Error("Material already exists");
    }
    this._materials.push(MaterialEntity.create(material));
  }

  addSectionProfile(sectionProfile: SectionProfileSnapshot): void {
    assertUuid(sectionProfile.id, "sectionProfileId");
    if (this._sectionProfiles.some((existing) => existing.id === sectionProfile.id)) {
      throw new Error("Section profile already exists");
    }
    this._sectionProfiles.push(SectionProfileEntity.create(sectionProfile));
  }

  addElement1d(element1d: Element1dSnapshot): void {
    assertUuid(element1d.id, "element1dId");
    if (this._element1ds.some((existing) => existing.id === element1d.id)) {
      throw new Error("Element1d already exists");
    }
    this._element1ds.push(Element1dEntity.create(element1d));
  }

  deleteNode(nodeId: string): void {
    assertUuid(nodeId, "nodeId");
    const index = this._nodes.findIndex((node) => node.id === nodeId);
    if (index < 0) {
      throw new Error("Node does not exist");
    }
    const [removed] = this._nodes.splice(index, 1);
    this._domainEvents.push({
      type: "node_deleted",
      payload: removed.toSnapshot(),
    });
  }

  toSnapshot(): ModelRevisionSnapshot {
    return {
      id: this.id,
      modelId: this.modelId,
      branchName: this._branchName,
      name: this._name,
      parentRevisionId: this._parentRevisionId,
      secondParentRevisionId: this._secondParentRevisionId,
      authorId: this._authorId,
      message: this._message,
      createdAt: this._createdAt,
      nodes: this._nodes.map((node) => node.toSnapshot()),
      materials: this._materials.map((material) => material.toSnapshot()),
      sectionProfiles: this._sectionProfiles.map((sectionProfile) =>
        sectionProfile.toSnapshot(),
      ),
      element1ds: this._element1ds.map((element1d) => element1d.toSnapshot()),
    };
  }

  pullDomainEvents(): DomainEvent[] {
    const nodeEvents = this._nodes.flatMap((node) =>
      node.pullDomainEvents(),
    ) as DomainEvent[];
    const materialEvents = this._materials.flatMap((material) =>
      material.pullDomainEvents(),
    );
    const sectionProfileEvents = this._sectionProfiles.flatMap(
      (sectionProfile) => sectionProfile.pullDomainEvents(),
    );
    const element1dEvents = this._element1ds.flatMap((element1d) =>
      element1d.pullDomainEvents(),
    );
    const events = [...this._domainEvents, ...nodeEvents, ...materialEvents, ...sectionProfileEvents, ...element1dEvents];
    this._domainEvents = [];
    return events;
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
