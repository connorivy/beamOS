import { DomainEvent } from "src/common/types";
import { assertUuid, assertUuidV7 } from "../common/uuid";
import {
  Element1dEntity,
  type Element1dSnapshot,
} from "../element1ds/element1d-entity";
import {
  MaterialEntity,
  type MaterialSnapshot,
} from "../materials/material-entity";
import {
  LoadCaseEntity,
  type LoadCaseSnapshot,
} from "../load-cases/load-case-entity";
import {
  LoadCombinationEntity,
  type LoadCombinationSnapshot,
} from "../load-combinations/load-combination-entity";
import {
  PointLoadEntity,
  type PointLoadSnapshot,
} from "../point-loads/point-load-entity";
import {
  ModelSettingsEntity,
  type ModelSettingsSnapshot,
} from "../model-settings/model-settings-entity";
import { NodeEntity, type NodeSnapshot } from "../nodes/node-entity";
import {
  SectionProfileEntity,
  type SectionProfileSnapshot,
} from "../section-profiles/section-profile-entity";

export type ModelRevisionSnapshot = {
  id: string;
  projectId: string;
  parentRevisionId: string | null;
  secondParentRevisionId: string | null;
  authorId: string;
  message: string;
  createdAt: Date;
  nodes: NodeEntity[];
  materials: MaterialEntity[];
  modelSettings: ModelSettingsEntity;
  sectionProfiles: SectionProfileEntity[];
  element1ds: Element1dEntity[];
  loadCases: LoadCaseEntity[];
  loadCombinations: LoadCombinationEntity[];
  pointLoads: PointLoadEntity[];
};

export type ModelRevisionCreateSnapshot = {
  id?: string;
  projectId: string;
  parentRevisionId: string | null;
  secondParentRevisionId: string | null;
  authorId: string;
  message: string;
  createdAt: Date;
  nodes: NodeSnapshot[];
  materials: MaterialSnapshot[];
  modelSettings: ModelSettingsSnapshot;
  sectionProfiles: SectionProfileSnapshot[];
  element1ds: Element1dSnapshot[];
  loadCases: LoadCaseSnapshot[];
  loadCombinations: LoadCombinationSnapshot[];
  pointLoads: PointLoadSnapshot[];
};

export class ModelRevisionAggregate {
  private _parentRevisionId: string | null;
  private _secondParentRevisionId: string | null;
  private _authorId: string;
  private _message: string;
  private _createdAt: Date;
  private _nodes: NodeEntity[];
  private _materials: MaterialEntity[];
  private _modelSettings: ModelSettingsEntity;
  private _sectionProfiles: SectionProfileEntity[];
  private _element1ds: Element1dEntity[];
  private _loadCases: LoadCaseEntity[];
  private _loadCombinations: LoadCombinationEntity[];
  private _pointLoads: PointLoadEntity[];
  private _domainEvents: DomainEvent[];

  private constructor(
    snapshot: ModelRevisionSnapshot | ModelRevisionCreateSnapshot,
  ) {
    const revisionId = snapshot.id ?? Bun.randomUUIDv7();
    assertUuidV7(revisionId, "id");
    assertUuid(snapshot.projectId, "projectId");
    assertUuid(snapshot.authorId, "authorId");
    this.assertRequired(snapshot.message, "message");
    this.assertOptionalUuid(snapshot.parentRevisionId, "parentRevisionId");
    this.assertOptionalUuid(
      snapshot.secondParentRevisionId,
      "secondParentRevisionId",
    );

    this.id = revisionId;
    this.projectId = snapshot.projectId;
    this._parentRevisionId = snapshot.parentRevisionId;
    this._secondParentRevisionId = snapshot.secondParentRevisionId;
    this._authorId = snapshot.authorId;
    this._message = snapshot.message.trim();
    this._createdAt = snapshot.createdAt;
    this._nodes = snapshot.nodes.map((node) =>
      node instanceof NodeEntity ? node : NodeEntity.rehydrate(node),
    );
    this._materials = snapshot.materials.map((material) =>
      material instanceof MaterialEntity
        ? material
        : MaterialEntity.rehydrate(material),
    );
    this._modelSettings =
      snapshot.modelSettings instanceof ModelSettingsEntity
        ? snapshot.modelSettings
        : ModelSettingsEntity.rehydrate(snapshot.modelSettings);
    this._sectionProfiles = snapshot.sectionProfiles.map((sectionProfile) =>
      sectionProfile instanceof SectionProfileEntity
        ? sectionProfile
        : SectionProfileEntity.rehydrate(sectionProfile),
    );
    this._element1ds = snapshot.element1ds.map((element1d) =>
      element1d instanceof Element1dEntity
        ? element1d
        : Element1dEntity.rehydrate(element1d),
    );
    this._loadCases = snapshot.loadCases.map((loadCase) =>
      loadCase instanceof LoadCaseEntity
        ? loadCase
        : LoadCaseEntity.rehydrate(loadCase),
    );
    this._loadCombinations = snapshot.loadCombinations.map((loadCombination) =>
      loadCombination instanceof LoadCombinationEntity
        ? loadCombination
        : LoadCombinationEntity.rehydrate(loadCombination),
    );
    this._pointLoads = snapshot.pointLoads.map((pointLoad) =>
      pointLoad instanceof PointLoadEntity
        ? pointLoad
        : PointLoadEntity.rehydrate(pointLoad),
    );
    this._domainEvents = [];
  }

  readonly id: string;
  readonly projectId: string;

  static create(snapshot: ModelRevisionCreateSnapshot): ModelRevisionAggregate {
    return new ModelRevisionAggregate(snapshot);
  }

  static rehydrate(snapshot: ModelRevisionSnapshot): ModelRevisionAggregate {
    return new ModelRevisionAggregate(snapshot);
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

  get modelSettings(): ModelSettingsEntity {
    return this._modelSettings;
  }

  get sectionProfiles(): readonly SectionProfileEntity[] {
    return this._sectionProfiles;
  }

  get element1ds(): readonly Element1dEntity[] {
    return this._element1ds;
  }

  get loadCases(): readonly LoadCaseEntity[] {
    return this._loadCases;
  }

  get loadCombinations(): readonly LoadCombinationEntity[] {
    return this._loadCombinations;
  }

  get pointLoads(): readonly PointLoadEntity[] {
    return this._pointLoads;
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
    if (this._materials.some((existing) => existing.name === material.name)) {
      throw new Error(`Material name "${material.name}" already exists`);
    }
    this._materials.push(MaterialEntity.create(material));
  }

  setModelSettings(modelSettings: ModelSettingsSnapshot): void {
    assertUuid(modelSettings.id, "modelSettingsId");
    this._modelSettings = ModelSettingsEntity.create(modelSettings);
  }

  addSectionProfile(sectionProfile: SectionProfileSnapshot): void {
    assertUuid(sectionProfile.id, "sectionProfileId");
    if (
      this._sectionProfiles.some(
        (existing) => existing.id === sectionProfile.id,
      )
    ) {
      throw new Error("Section profile already exists");
    }
    if (
      this._sectionProfiles.some(
        (existing) => existing.name === sectionProfile.name,
      )
    ) {
      throw new Error(
        `Section profile name "${sectionProfile.name}" already exists`,
      );
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

  addLoadCase(loadCase: LoadCaseSnapshot): void {
    assertUuid(loadCase.id, "loadCaseId");
    if (this._loadCases.some((existing) => existing.id === loadCase.id)) {
      throw new Error("Load case already exists");
    }
    this._loadCases.push(LoadCaseEntity.create(loadCase));
  }

  addLoadCombination(loadCombination: LoadCombinationSnapshot): void {
    assertUuid(loadCombination.id, "loadCombinationId");
    if (
      this._loadCombinations.some(
        (existing) => existing.id === loadCombination.id,
      )
    ) {
      throw new Error("Load combination already exists");
    }
    this._loadCombinations.push(LoadCombinationEntity.create(loadCombination));
  }

  addPointLoad(pointLoad: PointLoadSnapshot): void {
    assertUuid(pointLoad.id, "pointLoadId");
    if (this._pointLoads.some((existing) => existing.id === pointLoad.id)) {
      throw new Error("Point load already exists");
    }
    this._pointLoads.push(PointLoadEntity.create(pointLoad));
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
      projectId: this.projectId,
      parentRevisionId: this._parentRevisionId,
      secondParentRevisionId: this._secondParentRevisionId,
      authorId: this._authorId,
      message: this._message,
      createdAt: this._createdAt,
      nodes: [...this._nodes],
      materials: [...this._materials],
      modelSettings: this._modelSettings,
      sectionProfiles: [...this._sectionProfiles],
      element1ds: [...this._element1ds],
      loadCases: [...this._loadCases],
      loadCombinations: [...this._loadCombinations],
      pointLoads: [...this._pointLoads],
    };
  }

  pullDomainEvents(): DomainEvent[] {
    const nodeEvents = this._nodes.flatMap((node) =>
      node.pullDomainEvents(),
    ) as DomainEvent[];
    const materialEvents = this._materials.flatMap((material) =>
      material.pullDomainEvents(),
    );
    const modelSettingsEvents = this._modelSettings.pullDomainEvents();
    const sectionProfileEvents = this._sectionProfiles.flatMap(
      (sectionProfile) => sectionProfile.pullDomainEvents(),
    );
    const element1dEvents = this._element1ds.flatMap((element1d) =>
      element1d.pullDomainEvents(),
    );
    const loadCaseEvents = this._loadCases.flatMap((loadCase) =>
      loadCase.pullDomainEvents(),
    );
    const loadCombinationEvents = this._loadCombinations.flatMap(
      (loadCombination) => loadCombination.pullDomainEvents(),
    );
    const pointLoadEvents = this._pointLoads.flatMap((pointLoad) =>
      pointLoad.pullDomainEvents(),
    );
    const events = [
      ...this._domainEvents,
      ...nodeEvents,
      ...materialEvents,
      ...modelSettingsEvents,
      ...sectionProfileEvents,
      ...element1dEvents,
      ...loadCaseEvents,
      ...loadCombinationEvents,
      ...pointLoadEvents,
    ];
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
