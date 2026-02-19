import { assertUuidV7 } from "src/common/uuid";
import { Element1dEntity } from "src/element1ds/element1d-entity";
import { LoadCaseEntity } from "src/load-cases/load-case-entity";
import { LoadCombinationEntity } from "src/load-combinations/load-combination-entity";
import { MaterialEntity } from "src/materials/material-entity";
import { ModelSettingsEntity } from "src/model-settings/model-settings-entity";
import { NodeEntity } from "src/nodes/node-entity";
import { PointLoadEntity } from "src/point-loads/point-load-entity";
import { SectionProfileEntity } from "src/section-profiles/section-profile-entity";
import {
    MaterialOperationsRequest,
    NodeOperationsRequest,
} from "./create-model-revision-request-schema";

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

// export type ModelRevisionCreateSnapshot = {
//   id?: string;
//   projectId: string;
//   parentRevisionId: string | null;
//   secondParentRevisionId: string | null;
//   authorId: string;
//   message: string;
//   createdAt: Date;
//   nodes: NodeSnapshot[];
//   materials: MaterialSnapshot[];
//   modelSettings: ModelSettingsSnapshot;
//   sectionProfiles: SectionProfileSnapshot[];
//   element1ds: Element1dSnapshot[];
//   loadCases: LoadCaseSnapshot[];
//   loadCombinations: LoadCombinationSnapshot[];
//   pointLoads: PointLoadSnapshot[];
// };

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

// type ModelRevisionEntitySnapshot =
//   | NodeSnapshot
//   | MaterialSnapshot
//   | ModelSettingsSnapshot
//   | SectionProfileSnapshot
//   | Element1dSnapshot
//   | LoadCaseSnapshot
//   | LoadCombinationSnapshot
//   | PointLoadSnapshot;

// type ModelRevisionEntityState = {
//   entityType: ModelRevisionEntityType;
//   entityId: string;
//   snapshot: ModelRevisionEntitySnapshot;
//   signature: string;
// };

// export type ModelRevisionEntityChange = {
//   entityType: ModelRevisionEntityType;
//   entityId: string;
//   op: ModelRevisionChangeOp;
//   snapshot: ModelRevisionEntitySnapshot;
// };

// type AddEntityInput<TSnapshot extends { id: string }> = Omit<
//   TSnapshot,
//   "id"
// > & {
//   id: string | null;
// };

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
    readonly id: string;
    readonly projectId: string;
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
    //   private _domainEvents: DomainEvent[];
    //   private _baselineEntities: Map<string, ModelRevisionEntityState>;

    private constructor(snapshot: ModelRevisionSnapshot) {
        const revisionId = snapshot.id ?? Bun.randomUUIDv7();
        assertUuidV7(revisionId, "id");
        assertUuidV7(snapshot.projectId, "projectId");
        assertUuidV7(snapshot.authorId, "authorId");
        // this.assertRequired(snapshot.message, "message");
        // this.assertOptionalUuid(snapshot.parentRevisionId, "parentRevisionId");
        // this.assertOptionalUuid(
        //   snapshot.secondParentRevisionId,
        //   "secondParentRevisionId",
        // );

        this.id = revisionId;
        this.projectId = snapshot.projectId;
        this._parentRevisionId = snapshot.parentRevisionId;
        this._secondParentRevisionId = snapshot.secondParentRevisionId;
        this._authorId = snapshot.authorId;
        this._message = snapshot.message.trim();
        this._createdAt = snapshot.createdAt;
        this._nodes = this.createInitialBucket(snapshot.nodes);
        this._materials = this.createInitialBucket(snapshot.materials);
        this._modelSettings = {
            status:
                snapshot.parentRevisionId === null && snapshot.secondParentRevisionId === null
                    ? "created"
                    : "unchanged",
            entity: snapshot.modelSettings,
        };
        this._sectionProfiles = this.createInitialBucket(snapshot.sectionProfiles);
        this._element1ds = this.createInitialBucket(snapshot.element1ds);
        this._loadCases = this.createInitialBucket(snapshot.loadCases);
        this._loadCombinations = this.createInitialBucket(snapshot.loadCombinations);
        this._pointLoads = this.createInitialBucket(snapshot.pointLoads);
    }

    static create(snapshot: ModelRevisionSnapshot): ModelRevisionAggregate {
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
        return this.toBucketValues(this._nodes);
    }

    get materials(): readonly MaterialEntity[] {
        return this.toBucketValues(this._materials);
    }

    get modelSettings(): ModelSettingsEntity {
        return this._modelSettings.entity;
    }

    get sectionProfiles(): readonly SectionProfileEntity[] {
        return this.toBucketValues(this._sectionProfiles);
    }

    get element1ds(): readonly Element1dEntity[] {
        return this.toBucketValues(this._element1ds);
    }

    get loadCases(): readonly LoadCaseEntity[] {
        return this.toBucketValues(this._loadCases);
    }

    get loadCombinations(): readonly LoadCombinationEntity[] {
        return this.toBucketValues(this._loadCombinations);
    }

    get pointLoads(): readonly PointLoadEntity[] {
        return this.toBucketValues(this._pointLoads);
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

    private tempIdToRealIdMap = new Map<string, string>();

    public applyNodeChanges(ops: NodeOperationsRequest) {
        for (const nodeId of ops.delete ?? []) {
            this.deleteById(this._nodes, nodeId, "node");
        }

        for (const createOp of ops.create ?? []) {
            this.applyCreateOp(
                createOp,
                this._nodes,
                (op, revisionId) => NodeEntity.create(op, revisionId),
                "node",
            );
        }

        for (const updateOp of ops.update ?? []) {
            this.applyUpdateOp(
                updateOp,
                this._nodes,
                (op, revisionId) => NodeEntity.create(op, revisionId, op.id),
                "node",
            );
        }
    }

    public applyMaterialChanges(ops: MaterialOperationsRequest) {
        for (const materialId of ops.delete ?? []) {
            this.deleteById(this._materials, materialId, "material");
        }

        for (const createOp of ops.create ?? []) {
            if (
                Array.from(this._materials.unchanged.values())
                    .concat(Array.from(this._materials.created.values()))
                    .some((material) => material.name.toLowerCase() === createOp.name.toLowerCase())
            ) {
                throw new Error(
                    `Cannot create material with name "${createOp.name}" - a material with that name already exists`,
                );
            }
            this.applyCreateOp(
                { ...createOp, tempId: undefined },
                this._materials,
                (op, revisionId) => MaterialEntity.create(op, revisionId),
                "material",
            );
        }

        for (const updateOp of ops.update ?? []) {
            const existingId = Array.from(this._materials.unchanged.values()).find(
                (material) => material.name.toLowerCase() === updateOp.name.toLowerCase(),
            )?.id;
            if (!existingId) {
                throw new Error(
                    `Cannot find existing material with name "${updateOp.name}" for update operation`,
                );
            }
            this.applyUpdateOp(
                { ...updateOp, id: existingId },
                this._materials,
                (op, revisionId) =>
                    MaterialEntity.create(
                        { ...op, name: updateOp.newName ?? updateOp.name },
                        revisionId,
                        op.id,
                    ),
                "material",
            );
        }
    }

    private deleteById<TEntity>(
        bucket: EntityBucket<TEntity>,
        entityId: string,
        entityTypeName: string,
    ) {
        if (
            !bucket.unchanged.has(entityId) &&
            !bucket.updated.has(entityId) &&
            !bucket.created.has(entityId)
        ) {
            throw new Error(`Cannot delete ${entityTypeName} with id ${entityId} - not found`);
        }
        bucket.unchanged.delete(entityId);
        bucket.updated.delete(entityId);
        bucket.created.delete(entityId);
        bucket.deleted.add(entityId);
    }

    private applyCreateOp<TCreateOp extends { tempId?: string }, TEntity extends { id: string }>(
        createOp: TCreateOp,
        bucket: EntityBucket<TEntity>,
        createFunction: (createOp: TCreateOp, revisionId: string) => TEntity,
        entityTypeName: string,
    ): void {
        const entity = createFunction(createOp, this.id);
        bucket.created.set(entity.id, entity);

        if (createOp.tempId) {
            if (this.tempIdToRealIdMap.has(createOp.tempId)) {
                throw new Error(
                    `Duplicate tempId "${createOp.tempId}" in ${entityTypeName} create operations`,
                );
            }
            this.tempIdToRealIdMap.set(createOp.tempId, entity.id);
        }
    }

    private applyUpdateOp<TUpdateOp extends { id: string }, TEntity extends { id: string }>(
        updateOp: TUpdateOp,
        bucket: EntityBucket<TEntity>,
        updateFunction: (updateOp: TUpdateOp, revisionId: string) => TEntity,
        entityTypeName: string,
    ): void {
        if (bucket.deleted.has(updateOp.id)) {
            throw new Error(
                `Cannot update ${entityTypeName} with id ${updateOp.id} - it is marked for deletion`,
            );
        }

        if (!bucket.unchanged.has(updateOp.id) && !bucket.updated.has(updateOp.id)) {
            throw new Error(`Cannot update ${entityTypeName} with id ${updateOp.id} - not found`);
        }

        const entity = updateFunction(updateOp, this.id);
        bucket.updated.set(entity.id, entity);
        bucket.unchanged.delete(updateOp.id);
    }
}
