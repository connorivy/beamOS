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

export type ModelRevisionChangeOp = "created" | "updated" | "deleted";

export type ModelRevisionEntityType =
  | "node"
  | "material"
  | "model_settings"
  | "section_profile"
  | "element1d"
  | "loadcase"
  | "loadcombination"
  | "pointload";

type ModelRevisionEntitySnapshot =
  | NodeSnapshot
  | MaterialSnapshot
  | ModelSettingsSnapshot
  | SectionProfileSnapshot
  | Element1dSnapshot
  | LoadCaseSnapshot
  | LoadCombinationSnapshot
  | PointLoadSnapshot;

type ModelRevisionEntityState = {
  entityType: ModelRevisionEntityType;
  entityId: string;
  snapshot: ModelRevisionEntitySnapshot;
  signature: string;
};

export type ModelRevisionEntityChange = {
  entityType: ModelRevisionEntityType;
  entityId: string;
  op: ModelRevisionChangeOp;
  snapshot: ModelRevisionEntitySnapshot;
};

type AddEntityInput<TSnapshot extends { id: string }> = Omit<
  TSnapshot,
  "id"
> & {
  id: string | null;
};

type EntityBucket<TEntity> = {
  unchanged: Map<string, TEntity>;
  created: Map<string, TEntity>;
  updated: Map<string, TEntity>;
  deleted: Set<string>;
};

export type ModelRevisionEntityBuckets<TEntity> = {
  unchanged: ReadonlyMap<string, TEntity>;
  created: ReadonlyMap<string, TEntity>;
  updated: ReadonlyMap<string, TEntity>;
  deleted: ReadonlySet<string>;
};

export type ModelRevisionSingleEntityState<TEntity> = {
  status: "unchanged" | "created" | "updated";
  entity: TEntity;
};

export class ModelRevisionAggregate {
  private _parentRevisionId: string | null;
  private _secondParentRevisionId: string | null;
  private _authorId: string;
  private _message: string;
  private _createdAt: Date;
  private _nodes: EntityBucket<NodeEntity>;
  private _materials: EntityBucket<MaterialEntity>;
  private _modelSettings: ModelRevisionSingleEntityState<ModelSettingsEntity>;
  private _sectionProfiles: EntityBucket<SectionProfileEntity>;
  private _element1ds: EntityBucket<Element1dEntity>;
  private _loadCases: EntityBucket<LoadCaseEntity>;
  private _loadCombinations: EntityBucket<LoadCombinationEntity>;
  private _pointLoads: EntityBucket<PointLoadEntity>;
  private _domainEvents: DomainEvent[];
  private _baselineEntities: Map<string, ModelRevisionEntityState>;

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
    this._nodes = this.createInitialBucket(
      snapshot.nodes.map((node) =>
        node instanceof NodeEntity ? node : NodeEntity.rehydrate(node),
      ),
    );
    this._materials = this.createInitialBucket(
      snapshot.materials.map((material) =>
        material instanceof MaterialEntity
          ? material
          : MaterialEntity.rehydrate(material),
      ),
    );
    const normalizedModelSettings =
      snapshot.modelSettings instanceof ModelSettingsEntity
        ? snapshot.modelSettings
        : ModelSettingsEntity.rehydrate(snapshot.modelSettings);
    this._modelSettings = {
      status:
        snapshot.parentRevisionId === null &&
        snapshot.secondParentRevisionId === null
          ? "created"
          : "unchanged",
      entity: normalizedModelSettings,
    };
    this._sectionProfiles = this.createInitialBucket(
      snapshot.sectionProfiles.map((sectionProfile) =>
        sectionProfile instanceof SectionProfileEntity
          ? sectionProfile
          : SectionProfileEntity.rehydrate(sectionProfile),
      ),
    );
    this._element1ds = this.createInitialBucket(
      snapshot.element1ds.map((element1d) =>
        element1d instanceof Element1dEntity
          ? element1d
          : Element1dEntity.rehydrate(element1d),
      ),
    );
    this._loadCases = this.createInitialBucket(
      snapshot.loadCases.map((loadCase) =>
        loadCase instanceof LoadCaseEntity
          ? loadCase
          : LoadCaseEntity.rehydrate(loadCase),
      ),
    );
    this._loadCombinations = this.createInitialBucket(
      snapshot.loadCombinations.map((loadCombination) =>
        loadCombination instanceof LoadCombinationEntity
          ? loadCombination
          : LoadCombinationEntity.rehydrate(loadCombination),
      ),
    );
    this._pointLoads = this.createInitialBucket(
      snapshot.pointLoads.map((pointLoad) =>
        pointLoad instanceof PointLoadEntity
          ? pointLoad
          : PointLoadEntity.rehydrate(pointLoad),
      ),
    );
    this._domainEvents = [];
    this._baselineEntities = this.buildCurrentEntityState();
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
    return [
      ...this._nodes.unchanged.values(),
      ...this._nodes.updated.values(),
      ...this._nodes.created.values(),
    ];
  }

  get nodeBuckets(): ModelRevisionEntityBuckets<NodeEntity> {
    return {
      unchanged: this._nodes.unchanged,
      created: this._nodes.created,
      updated: this._nodes.updated,
      deleted: this._nodes.deleted,
    };
  }

  get materials(): readonly MaterialEntity[] {
    return [
      ...this._materials.unchanged.values(),
      ...this._materials.updated.values(),
      ...this._materials.created.values(),
    ];
  }

  get materialBuckets(): ModelRevisionEntityBuckets<MaterialEntity> {
    return {
      unchanged: this._materials.unchanged,
      created: this._materials.created,
      updated: this._materials.updated,
      deleted: this._materials.deleted,
    };
  }

  get modelSettings(): ModelSettingsEntity {
    return this._modelSettings.entity;
  }

  get modelSettingsState(): ModelRevisionSingleEntityState<ModelSettingsEntity> {
    return this._modelSettings;
  }

  get sectionProfiles(): readonly SectionProfileEntity[] {
    return this.toBucketValues(this._sectionProfiles);
  }

  get sectionProfileBuckets(): ModelRevisionEntityBuckets<SectionProfileEntity> {
    return {
      unchanged: this._sectionProfiles.unchanged,
      created: this._sectionProfiles.created,
      updated: this._sectionProfiles.updated,
      deleted: this._sectionProfiles.deleted,
    };
  }

  get element1ds(): readonly Element1dEntity[] {
    return this.toBucketValues(this._element1ds);
  }

  get element1dBuckets(): ModelRevisionEntityBuckets<Element1dEntity> {
    return {
      unchanged: this._element1ds.unchanged,
      created: this._element1ds.created,
      updated: this._element1ds.updated,
      deleted: this._element1ds.deleted,
    };
  }

  get loadCases(): readonly LoadCaseEntity[] {
    return this.toBucketValues(this._loadCases);
  }

  get loadCaseBuckets(): ModelRevisionEntityBuckets<LoadCaseEntity> {
    return {
      unchanged: this._loadCases.unchanged,
      created: this._loadCases.created,
      updated: this._loadCases.updated,
      deleted: this._loadCases.deleted,
    };
  }

  get loadCombinations(): readonly LoadCombinationEntity[] {
    return this.toBucketValues(this._loadCombinations);
  }

  get loadCombinationBuckets(): ModelRevisionEntityBuckets<LoadCombinationEntity> {
    return {
      unchanged: this._loadCombinations.unchanged,
      created: this._loadCombinations.created,
      updated: this._loadCombinations.updated,
      deleted: this._loadCombinations.deleted,
    };
  }

  get pointLoads(): readonly PointLoadEntity[] {
    return this.toBucketValues(this._pointLoads);
  }

  get pointLoadBuckets(): ModelRevisionEntityBuckets<PointLoadEntity> {
    return {
      unchanged: this._pointLoads.unchanged,
      created: this._pointLoads.created,
      updated: this._pointLoads.updated,
      deleted: this._pointLoads.deleted,
    };
  }

  addNode(node: AddEntityInput<NodeSnapshot>): void {
    const nodeId = this.resolveEntityId(node.id, "nodeId");
    const normalizedNode: NodeSnapshot = {
      ...node,
      id: nodeId,
    };
    const next =
      node.id === null
        ? NodeEntity.create(normalizedNode)
        : NodeEntity.rehydrate(normalizedNode);

    if (this._nodes.unchanged.delete(nodeId)) {
      this._nodes.updated.delete(nodeId);
      this._nodes.created.delete(nodeId);
      this._nodes.deleted.delete(nodeId);
      this._nodes.updated.set(nodeId, next);
    } else {
      this._nodes.updated.delete(nodeId);
      this._nodes.created.set(nodeId, next);
      this._nodes.deleted.delete(nodeId);
    }
  }

  addMaterial(material: AddEntityInput<MaterialSnapshot>): void {
    const materialId = this.resolveEntityId(material.id, "materialId");
    const normalizedMaterial: MaterialSnapshot = {
      ...material,
      id: materialId,
    };
    if (
      this.materials.some(
        (existing) =>
          existing.name === normalizedMaterial.name &&
          existing.id !== materialId,
      )
    ) {
      throw new Error(
        `Material name "${normalizedMaterial.name}" already exists`,
      );
    }

    const next =
      material.id === null
        ? MaterialEntity.create(normalizedMaterial)
        : MaterialEntity.rehydrate(normalizedMaterial);

    if (this._materials.unchanged.delete(materialId)) {
      this._materials.updated.delete(materialId);
      this._materials.created.delete(materialId);
      this._materials.deleted.delete(materialId);
      this._materials.updated.set(materialId, next);
      return;
    }

    this._materials.updated.delete(materialId);
    this._materials.created.set(materialId, next);
    this._materials.deleted.delete(materialId);
  }

  setModelSettings(modelSettings: ModelSettingsSnapshot): void {
    assertUuid(modelSettings.id, "modelSettingsId");
    const currentModelSettingsId = this._modelSettings.entity.id;
    const normalizedSnapshot: ModelSettingsSnapshot = {
      ...modelSettings,
      id: currentModelSettingsId,
    };
    this._modelSettings = {
      status: this._modelSettings.status === "created" ? "created" : "updated",
      entity: ModelSettingsEntity.create(normalizedSnapshot),
    };
  }

  addSectionProfile(
    sectionProfile: AddEntityInput<SectionProfileSnapshot>,
  ): void {
    const sectionProfileId = this.resolveEntityId(
      sectionProfile.id,
      "sectionProfileId",
    );
    const normalizedSectionProfile: SectionProfileSnapshot = {
      ...sectionProfile,
      id: sectionProfileId,
    };
    if (
      this.sectionProfiles.some(
        (existing) =>
          existing.name === normalizedSectionProfile.name &&
          existing.id !== sectionProfileId,
      )
    ) {
      throw new Error(
        `Section profile name "${normalizedSectionProfile.name}" already exists`,
      );
    }

    const next =
      sectionProfile.id === null
        ? SectionProfileEntity.create(normalizedSectionProfile)
        : SectionProfileEntity.rehydrate(normalizedSectionProfile);

    if (this._sectionProfiles.unchanged.delete(sectionProfileId)) {
      this._sectionProfiles.updated.delete(sectionProfileId);
      this._sectionProfiles.created.delete(sectionProfileId);
      this._sectionProfiles.deleted.delete(sectionProfileId);
      this._sectionProfiles.updated.set(sectionProfileId, next);
      return;
    }

    this._sectionProfiles.updated.delete(sectionProfileId);
    this._sectionProfiles.created.set(sectionProfileId, next);
    this._sectionProfiles.deleted.delete(sectionProfileId);
  }

  addElement1d(element1d: AddEntityInput<Element1dSnapshot>): void {
    const element1dId = this.resolveEntityId(element1d.id, "element1dId");
    const normalizedElement1d: Element1dSnapshot = {
      ...element1d,
      id: element1dId,
    };
    const next =
      element1d.id === null
        ? Element1dEntity.create(normalizedElement1d)
        : Element1dEntity.rehydrate(normalizedElement1d);

    if (this._element1ds.unchanged.delete(element1dId)) {
      this._element1ds.updated.delete(element1dId);
      this._element1ds.created.delete(element1dId);
      this._element1ds.deleted.delete(element1dId);
      this._element1ds.updated.set(element1dId, next);
      return;
    }

    this._element1ds.updated.delete(element1dId);
    this._element1ds.created.set(element1dId, next);
    this._element1ds.deleted.delete(element1dId);
  }

  addLoadCase(loadCase: AddEntityInput<LoadCaseSnapshot>): void {
    const loadCaseId = this.resolveEntityId(loadCase.id, "loadCaseId");
    const normalizedLoadCase: LoadCaseSnapshot = {
      ...loadCase,
      id: loadCaseId,
    };
    const next =
      loadCase.id === null
        ? LoadCaseEntity.create(normalizedLoadCase)
        : LoadCaseEntity.rehydrate(normalizedLoadCase);

    if (this._loadCases.unchanged.delete(loadCaseId)) {
      this._loadCases.updated.delete(loadCaseId);
      this._loadCases.created.delete(loadCaseId);
      this._loadCases.deleted.delete(loadCaseId);
      this._loadCases.updated.set(loadCaseId, next);
      return;
    }

    this._loadCases.updated.delete(loadCaseId);
    this._loadCases.created.set(loadCaseId, next);
    this._loadCases.deleted.delete(loadCaseId);
  }

  addLoadCombination(
    loadCombination: AddEntityInput<LoadCombinationSnapshot>,
  ): void {
    const loadCombinationId = this.resolveEntityId(
      loadCombination.id,
      "loadCombinationId",
    );
    const normalizedLoadCombination: LoadCombinationSnapshot = {
      ...loadCombination,
      id: loadCombinationId,
    };
    const next =
      loadCombination.id === null
        ? LoadCombinationEntity.create(normalizedLoadCombination)
        : LoadCombinationEntity.rehydrate(normalizedLoadCombination);

    if (this._loadCombinations.unchanged.delete(loadCombinationId)) {
      this._loadCombinations.updated.delete(loadCombinationId);
      this._loadCombinations.created.delete(loadCombinationId);
      this._loadCombinations.deleted.delete(loadCombinationId);
      this._loadCombinations.updated.set(loadCombinationId, next);
      return;
    }

    this._loadCombinations.updated.delete(loadCombinationId);
    this._loadCombinations.created.set(loadCombinationId, next);
    this._loadCombinations.deleted.delete(loadCombinationId);
  }

  addPointLoad(pointLoad: AddEntityInput<PointLoadSnapshot>): void {
    const pointLoadId = this.resolveEntityId(pointLoad.id, "pointLoadId");
    const normalizedPointLoad: PointLoadSnapshot = {
      ...pointLoad,
      id: pointLoadId,
    };
    const next =
      pointLoad.id === null
        ? PointLoadEntity.create(normalizedPointLoad)
        : PointLoadEntity.rehydrate(normalizedPointLoad);

    if (this._pointLoads.unchanged.delete(pointLoadId)) {
      this._pointLoads.updated.delete(pointLoadId);
      this._pointLoads.created.delete(pointLoadId);
      this._pointLoads.deleted.delete(pointLoadId);
      this._pointLoads.updated.set(pointLoadId, next);
      return;
    }

    this._pointLoads.updated.delete(pointLoadId);
    this._pointLoads.created.set(pointLoadId, next);
    this._pointLoads.deleted.delete(pointLoadId);
  }

  deleteNode(nodeId: string): void {
    assertUuid(nodeId, "nodeId");

    const createdNode = this._nodes.created.get(nodeId);
    if (createdNode) {
      this._nodes.created.delete(nodeId);
      return;
    }

    const updatedNode = this._nodes.updated.get(nodeId);
    if (updatedNode) {
      this._nodes.updated.delete(nodeId);
      this._nodes.deleted.add(nodeId);
      this._domainEvents.push({
        type: "node_deleted",
        payload: updatedNode.toSnapshot(),
      });
      return;
    }

    const unchangedNode = this._nodes.unchanged.get(nodeId);
    if (unchangedNode) {
      this._nodes.unchanged.delete(nodeId);
      this._nodes.deleted.add(nodeId);
      this._domainEvents.push({
        type: "node_deleted",
        payload: unchangedNode.toSnapshot(),
      });
      return;
    }

    if (this._nodes.deleted.has(nodeId)) {
      return;
    }

    if (!createdNode && !updatedNode && !unchangedNode) {
      throw new Error("Node does not exist");
    }
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
      nodes: [...this.nodes],
      materials: [...this.materials],
      modelSettings: this.modelSettings,
      sectionProfiles: [...this.sectionProfiles],
      element1ds: [...this.element1ds],
      loadCases: [...this.loadCases],
      loadCombinations: [...this.loadCombinations],
      pointLoads: [...this.pointLoads],
    };
  }

  pullDomainEvents(): DomainEvent[] {
    const nodeEvents = this.nodes.flatMap((node) =>
      node.pullDomainEvents(),
    ) as DomainEvent[];
    const materialEvents = this.materials.flatMap((material) =>
      material.pullDomainEvents(),
    );
    const modelSettingsEvents = this.modelSettings.pullDomainEvents();
    const sectionProfileEvents = this.sectionProfiles.flatMap(
      (sectionProfile) => sectionProfile.pullDomainEvents(),
    );
    const element1dEvents = this.element1ds.flatMap((element1d) =>
      element1d.pullDomainEvents(),
    );
    const loadCaseEvents = this.loadCases.flatMap((loadCase) =>
      loadCase.pullDomainEvents(),
    );
    const loadCombinationEvents = this.loadCombinations.flatMap(
      (loadCombination) => loadCombination.pullDomainEvents(),
    );
    const pointLoadEvents = this.pointLoads.flatMap((pointLoad) =>
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

  pullRevisionChanges(): ModelRevisionEntityChange[] {
    const currentEntities = this.buildCurrentEntityState();
    const changes: ModelRevisionEntityChange[] = [];

    for (const [key, current] of currentEntities.entries()) {
      const baseline = this._baselineEntities.get(key);
      if (!baseline) {
        changes.push({
          entityType: current.entityType,
          entityId: current.entityId,
          op: "created",
          snapshot: current.snapshot,
        });
        continue;
      }

      if (baseline.signature !== current.signature) {
        changes.push({
          entityType: current.entityType,
          entityId: current.entityId,
          op: "updated",
          snapshot: current.snapshot,
        });
      }
    }

    for (const [key, baseline] of this._baselineEntities.entries()) {
      if (!currentEntities.has(key)) {
        changes.push({
          entityType: baseline.entityType,
          entityId: baseline.entityId,
          op: "deleted",
          snapshot: baseline.snapshot,
        });
      }
    }

    this._baselineEntities = currentEntities;

    return changes.sort((a, b) => {
      const typeComparison = a.entityType.localeCompare(b.entityType);
      if (typeComparison !== 0) {
        return typeComparison;
      }

      const idComparison = a.entityId.localeCompare(b.entityId);
      if (idComparison !== 0) {
        return idComparison;
      }

      return a.op.localeCompare(b.op);
    });
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

  private buildCurrentEntityState(): Map<string, ModelRevisionEntityState> {
    const entities = new Map<string, ModelRevisionEntityState>();

    for (const node of this.nodes) {
      this.setEntityState(entities, {
        entityType: "node",
        entityId: node.id,
        snapshot: node.toSnapshot(),
      });
    }

    for (const material of this.materials) {
      this.setEntityState(entities, {
        entityType: "material",
        entityId: material.id,
        snapshot: material.toSnapshot(),
      });
    }

    this.setEntityState(entities, {
      entityType: "model_settings",
      entityId: this.modelSettings.id,
      snapshot: this.modelSettings.toSnapshot(),
    });

    for (const sectionProfile of this.sectionProfiles) {
      this.setEntityState(entities, {
        entityType: "section_profile",
        entityId: sectionProfile.id,
        snapshot: sectionProfile.toSnapshot(),
      });
    }

    for (const element1d of this.element1ds) {
      this.setEntityState(entities, {
        entityType: "element1d",
        entityId: element1d.id,
        snapshot: element1d.toSnapshot(),
      });
    }

    for (const loadCase of this.loadCases) {
      this.setEntityState(entities, {
        entityType: "loadcase",
        entityId: loadCase.id,
        snapshot: loadCase.toSnapshot(),
      });
    }

    for (const loadCombination of this.loadCombinations) {
      this.setEntityState(entities, {
        entityType: "loadcombination",
        entityId: loadCombination.id,
        snapshot: loadCombination.toSnapshot(),
      });
    }

    for (const pointLoad of this.pointLoads) {
      this.setEntityState(entities, {
        entityType: "pointload",
        entityId: pointLoad.id,
        snapshot: pointLoad.toSnapshot(),
      });
    }

    return entities;
  }

  private setEntityState(
    states: Map<string, ModelRevisionEntityState>,
    input: {
      entityType: ModelRevisionEntityType;
      entityId: string;
      snapshot: ModelRevisionEntitySnapshot;
    },
  ): void {
    states.set(this.toEntityStateKey(input.entityType, input.entityId), {
      entityType: input.entityType,
      entityId: input.entityId,
      snapshot: input.snapshot,
      signature: this.toEntitySignature(input.snapshot),
    });
  }

  private toEntityStateKey(
    entityType: ModelRevisionEntityType,
    entityId: string,
  ): string {
    return `${entityType}:${entityId}`;
  }

  private toEntitySignature(value: unknown): string {
    return JSON.stringify(this.normalizeForSignature(value));
  }

  private normalizeForSignature(value: unknown): unknown {
    if (value === null || value === undefined) {
      return value;
    }

    if (Array.isArray(value)) {
      return value.map((item) => this.normalizeForSignature(item));
    }

    if (typeof value === "object") {
      if (this.hasFiniteBaseValue(value)) {
        return {
          __unitsType: "base_value",
          baseValue: value.BaseValue,
        };
      }

      if (value instanceof Date) {
        return {
          __date: value.toISOString(),
        };
      }

      const normalized: Record<string, unknown> = {};
      for (const key of Object.keys(value).sort()) {
        normalized[key] = this.normalizeForSignature(
          (value as Record<string, unknown>)[key],
        );
      }
      return normalized;
    }

    return value;
  }

  private createInitialBucket<TEntity extends { id: string }>(
    entities: TEntity[],
  ): EntityBucket<TEntity> {
    return {
      unchanged: new Map(entities.map((entity) => [entity.id, entity])),
      created: new Map(),
      updated: new Map(),
      deleted: new Set(),
    };
  }

  private toBucketValues<TEntity>(bucket: EntityBucket<TEntity>): TEntity[] {
    return [
      ...bucket.unchanged.values(),
      ...bucket.updated.values(),
      ...bucket.created.values(),
    ];
  }

  private hasFiniteBaseValue(value: unknown): value is { BaseValue: number } {
    if (!value || typeof value !== "object") {
      return false;
    }

    const baseValue = (value as { BaseValue?: unknown }).BaseValue;
    return typeof baseValue === "number" && Number.isFinite(baseValue);
  }

  private resolveEntityId(id: string | null, field: string): string {
    if (id !== null) {
      assertUuid(id, field);
      return id;
    }

    return Bun.randomUUIDv7();
  }
}
