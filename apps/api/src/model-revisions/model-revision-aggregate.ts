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
    Area,
    AreaMomentOfInertia,
    Force,
    Torque,
    Volume,
    WarpingMomentOfInertia,
} from "unitsnet-js";
import {
    Element1dOperationsRequest,
    LoadCaseOperationsRequest,
    LoadCombinationOperationsRequest,
    MaterialOperationsRequest,
    ModelSettingsUpdateRequest,
    NodeOperationsRequest,
    PointLoadOperationsRequest,
    SectionProfileOperationsRequest,
} from "./create-model-revision-request-schema";
import { httpError } from "src/common/http-utils";

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

type EntityBucket<TEntity> = {
    unchanged: Map<string, TEntity>;
    created: Map<string, TEntity>;
    updated: Map<string, TEntity>;
    deleted: Set<string>;
    applicationIdToEntityIds: Map<string, string>;
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

    get nodeBuckets(): ModelRevisionEntityBuckets<NodeEntity> {
        return {
            unchanged: this._nodes.unchanged,
            created: this._nodes.created,
            updated: this._nodes.updated,
            deleted: this._nodes.deleted,
        };
    }

    get materials(): readonly MaterialEntity[] {
        return this.toBucketValues(this._materials);
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

    public getSectionProfileById(id: string): SectionProfileEntity | undefined {
        let sectionProfile = this._sectionProfiles.unchanged.get(id);
        if (!sectionProfile) {
            sectionProfile = this._sectionProfiles.created.get(id);
        }
        if (!sectionProfile) {
            sectionProfile = this._sectionProfiles.updated.get(id);
        }
        return sectionProfile;
    }

    public getMaterialById(id: string): MaterialEntity | undefined {
        let material = this._materials.unchanged.get(id);
        if (!material) {
            material = this._materials.created.get(id);
        }
        if (!material) {
            material = this._materials.updated.get(id);
        }
        return material;
    }

    public getElement1dById(id: string): Element1dEntity | undefined {
        let element1d = this._element1ds.unchanged.get(id);
        if (!element1d) {
            element1d = this._element1ds.created.get(id);
        }
        if (!element1d) {
            element1d = this._element1ds.updated.get(id);
        }
        return element1d;
    }

    public toSnapshot(): ModelRevisionSnapshot {
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

    public markChangesAsPersisted(): void {
        this._nodes = this.flushBucket(this._nodes);
        this._materials = this.flushBucket(this._materials);
        this._sectionProfiles = this.flushBucket(this._sectionProfiles);
        this._element1ds = this.flushBucket(this._element1ds);
        this._loadCases = this.flushBucket(this._loadCases);
        this._loadCombinations = this.flushBucket(this._loadCombinations);
        this._pointLoads = this.flushBucket(this._pointLoads);
        this._modelSettings = {
            status: "unchanged",
            entity: this._modelSettings.entity,
        };
    }

    private createInitialBucket<TEntity extends { id: string; applicationId?: string }>(
        entities: TEntity[],
    ): EntityBucket<TEntity> {
        return {
            unchanged: new Map(entities.map((entity) => [entity.id, entity])),
            created: new Map(),
            updated: new Map(),
            deleted: new Set(),
            applicationIdToEntityIds: new Map(
                entities
                    .filter((entity) => entity.applicationId !== undefined)
                    .map((entity) => [entity.applicationId!, entity.id]),
            ),
        };
    }

    private toBucketValues<TEntity>(bucket: EntityBucket<TEntity>): TEntity[] {
        return [
            ...bucket.unchanged.values(),
            ...bucket.updated.values(),
            ...bucket.created.values(),
        ];
    }

    private flushBucket<TEntity extends { id: string }>(
        bucket: EntityBucket<TEntity>,
    ): EntityBucket<TEntity> {
        return {
            unchanged: new Map(this.toBucketValues(bucket).map((entity) => [entity.id, entity])),
            created: new Map(),
            updated: new Map(),
            deleted: new Set(),
            applicationIdToEntityIds: new Map(bucket.applicationIdToEntityIds),
        };
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
            const existingId = this.getExistingEntityIdByIdOrApplicationId(
                this._nodes,
                updateOp.id,
                updateOp.applicationId,
            );
            this.applyUpdateOp(
                updateOp,
                existingId,
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
                throw httpError(
                    `Cannot create material with name "${createOp.name}" - a material with that name already exists`,
                    409,
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
            const existingId = this.getExistingEntityIdByNameOrApplicationId(
                this._materials,
                updateOp.name,
                updateOp.applicationId,
            );
            this.applyUpdateOp(
                updateOp,
                existingId,
                this._materials,
                (op, revisionId, existingEntityId) =>
                    MaterialEntity.create(
                        { ...op, name: updateOp.newName ?? updateOp.name },
                        revisionId,
                        existingEntityId,
                    ),
                "material",
            );
        }
    }

    public applySectionProfileChanges(ops: SectionProfileOperationsRequest) {
        for (const sectionProfileName of ops.delete ?? []) {
            const sectionProfileId = this.getExistingEntityIdByNameOrApplicationId(
                this._sectionProfiles,
                sectionProfileName,
                undefined,
            );
            if (!sectionProfileId) {
                throw httpError(
                    `Cannot find existing section profile with name "${sectionProfileName}"`,
                    404,
                );
            }
            this.deleteById(this._sectionProfiles, sectionProfileId, "section profile");
        }

        for (const createOp of ops.create ?? []) {
            const hasStrong = createOp.strongAxisShearArea !== undefined;
            const hasWeak = createOp.weakAxisShearArea !== undefined;
            if (hasStrong !== hasWeak) {
                throw httpError(
                    "strongAxisShearArea and weakAxisShearArea must both be provided or both be omitted",
                    400,
                );
            }

            if (
                this.sectionProfiles.some(
                    (sectionProfile) =>
                        sectionProfile.name.toLowerCase() === createOp.name.toLowerCase(),
                )
            ) {
                throw httpError(`Duplicate section profile name "${createOp.name}"`, 409);
            }

            this.applyCreateOp(
                createOp,
                this._sectionProfiles,
                (op, revisionId) =>
                    SectionProfileEntity.create({
                        id: Bun.randomUUIDv7(),
                        revisionId,
                        name: op.name,
                        discriminator: hasStrong && hasWeak ? "WITH_SHEAR_AREAS" : "STANDARD",
                        area: new Area(op.area, op.units.area),
                        strongAxisMomentOfInertia: new AreaMomentOfInertia(
                            op.strongAxisMomentOfInertia,
                            op.units.areaMomentOfInertia,
                        ),
                        weakAxisMomentOfInertia: new AreaMomentOfInertia(
                            op.weakAxisMomentOfInertia,
                            op.units.areaMomentOfInertia,
                        ),
                        torsionalConstant: new AreaMomentOfInertia(
                            op.torsionalConstant,
                            op.units.areaMomentOfInertia,
                        ),
                        warpingConstant: new WarpingMomentOfInertia(
                            op.warpingConstant,
                            op.units.warpingMomentOfInertia,
                        ),
                        strongAxisPlasticSectionModulus: new Volume(
                            op.strongAxisPlasticSectionModulus,
                            op.units.volume,
                        ),
                        weakAxisPlasticSectionModulus: new Volume(
                            op.weakAxisPlasticSectionModulus,
                            op.units.volume,
                        ),
                        strongAxisElasticSectionModulus: new Volume(
                            op.strongAxisElasticSectionModulus,
                            op.units.volume,
                        ),
                        weakAxisElasticSectionModulus: new Volume(
                            op.weakAxisElasticSectionModulus,
                            op.units.volume,
                        ),
                        ...(hasStrong && hasWeak
                            ? {
                                  strongAxisShearArea: new Area(
                                      op.strongAxisShearArea!,
                                      op.units.area,
                                  ),
                                  weakAxisShearArea: new Area(op.weakAxisShearArea!, op.units.area),
                              }
                            : {}),
                    }),
                "section profile",
            );
        }

        for (const updateOp of ops.update ?? []) {
            const existingId = this.sectionProfiles.find(
                (sectionProfile) =>
                    sectionProfile.name.toLowerCase() === updateOp.name.toLowerCase(),
            )?.id;
            if (!existingId) {
                throw httpError(`Section profile "${updateOp.name}" not found`, 404);
            }

            const targetName = updateOp.newName ?? updateOp.name;
            const existingTargetId = this.sectionProfiles.find(
                (sectionProfile) => sectionProfile.name.toLowerCase() === targetName.toLowerCase(),
            )?.id;
            if (existingTargetId && existingTargetId !== existingId) {
                throw httpError(`Duplicate section profile name "${targetName}"`, 409);
            }

            const hasStrong = updateOp.strongAxisShearArea !== undefined;
            const hasWeak = updateOp.weakAxisShearArea !== undefined;
            if (hasStrong !== hasWeak) {
                throw httpError(
                    "strongAxisShearArea and weakAxisShearArea must both be provided or both be omitted",
                    400,
                );
            }

            this.applyUpdateOp(
                updateOp,
                existingId,
                this._sectionProfiles,
                (op, revisionId, existingEntityId) =>
                    SectionProfileEntity.create({
                        id: existingEntityId,
                        revisionId,
                        name: op.newName ?? op.name,
                        discriminator: hasStrong && hasWeak ? "WITH_SHEAR_AREAS" : "STANDARD",
                        area: new Area(op.area, op.units.area),
                        strongAxisMomentOfInertia: new AreaMomentOfInertia(
                            op.strongAxisMomentOfInertia,
                            op.units.areaMomentOfInertia,
                        ),
                        weakAxisMomentOfInertia: new AreaMomentOfInertia(
                            op.weakAxisMomentOfInertia,
                            op.units.areaMomentOfInertia,
                        ),
                        torsionalConstant: new AreaMomentOfInertia(
                            op.torsionalConstant,
                            op.units.areaMomentOfInertia,
                        ),
                        warpingConstant: new WarpingMomentOfInertia(
                            op.warpingConstant,
                            op.units.warpingMomentOfInertia,
                        ),
                        strongAxisPlasticSectionModulus: new Volume(
                            op.strongAxisPlasticSectionModulus,
                            op.units.volume,
                        ),
                        weakAxisPlasticSectionModulus: new Volume(
                            op.weakAxisPlasticSectionModulus,
                            op.units.volume,
                        ),
                        strongAxisElasticSectionModulus: new Volume(
                            op.strongAxisElasticSectionModulus,
                            op.units.volume,
                        ),
                        weakAxisElasticSectionModulus: new Volume(
                            op.weakAxisElasticSectionModulus,
                            op.units.volume,
                        ),
                        ...(hasStrong && hasWeak
                            ? {
                                  strongAxisShearArea: new Area(
                                      op.strongAxisShearArea!,
                                      op.units.area,
                                  ),
                                  weakAxisShearArea: new Area(op.weakAxisShearArea!, op.units.area),
                              }
                            : {}),
                    }),
                "section profile",
            );
        }
    }

    private resolveId(value: string): string {
        if (this.tempIdToRealIdMap.has(value)) {
            return this.tempIdToRealIdMap.get(value)!;
        }
        return value;
    }

    public applyElement1dChanges(ops: Element1dOperationsRequest) {
        for (const element1dId of ops.delete ?? []) {
            this.deleteById(this._element1ds, element1dId, "element1d");
        }

        for (const createOp of ops.create ?? []) {
            const material = this.materials.find((entry) => entry.name === createOp.materialName);
            if (!material) {
                throw httpError(`Material "${createOp.materialName}" not found`, 404);
            }
            const sectionProfile = this.sectionProfiles.find(
                (entry) => entry.name === createOp.sectionProfileName,
            );
            if (!sectionProfile) {
                throw httpError(`Section profile "${createOp.sectionProfileName}" not found`, 404);
            }

            const resolvedStartNodeId = this.resolveId(createOp.startNodeId);
            const resolvedEndNodeId = this.resolveId(createOp.endNodeId);

            this.applyCreateOp(
                {
                    ...createOp,
                    startNodeId: resolvedStartNodeId,
                    endNodeId: resolvedEndNodeId,
                    materialId: material.id,
                    sectionProfileId: sectionProfile.id,
                },
                this._element1ds,
                (op, revisionId) =>
                    Element1dEntity.create({
                        id: Bun.randomUUIDv7(),
                        revisionId,
                        startNodeId: op.startNodeId,
                        endNodeId: op.endNodeId,
                        materialId: op.materialId,
                        sectionProfileId: op.sectionProfileId,
                    }),
                "element1d",
            );
        }

        for (const updateOp of ops.update ?? []) {
            const existingId = this.getExistingEntityIdByIdOrApplicationId(
                this._element1ds,
                updateOp.id,
            );
            this.applyUpdateOp(
                updateOp,
                existingId,
                this._element1ds,
                (op, revisionId, existingEntityId) =>
                    Element1dEntity.create({
                        id: existingEntityId,
                        revisionId,
                        startNodeId: op.startNodeId,
                        endNodeId: op.endNodeId,
                        materialId: op.materialId,
                        sectionProfileId: op.sectionProfileId,
                    }),
                "element1d",
            );
        }
    }

    public applyLoadCaseChanges(ops: LoadCaseOperationsRequest) {
        for (const loadCaseId of ops.delete ?? []) {
            this.deleteById(this._loadCases, loadCaseId, "load case");
        }

        for (const createOp of ops.create ?? []) {
            this.applyCreateOp(
                createOp,
                this._loadCases,
                (op, revisionId) =>
                    LoadCaseEntity.create({
                        id: Bun.randomUUIDv7(),
                        revisionId,
                        name: op.name,
                    }),
                "load case",
            );
        }

        for (const updateOp of ops.update ?? []) {
            const existingId = this.getExistingEntityIdByIdOrApplicationId(
                this._loadCases,
                updateOp.id,
            );
            this.applyUpdateOp(
                updateOp,
                existingId,
                this._loadCases,
                (op, revisionId, existingEntityId) =>
                    LoadCaseEntity.create({
                        id: existingEntityId,
                        revisionId,
                        name: op.name,
                    }),
                "load case",
            );
        }
    }

    public applyLoadCombinationChanges(ops: LoadCombinationOperationsRequest) {
        for (const loadCombinationId of ops.delete ?? []) {
            this.deleteById(this._loadCombinations, loadCombinationId, "load combination");
        }

        for (const createOp of ops.create ?? []) {
            const resolvedLoadCaseFactors = Object.fromEntries(
                Object.entries(createOp.loadCaseFactors).map(([key, value]) => [
                    this.resolveId(key),
                    value,
                ]),
            );

            this.applyCreateOp(
                { ...createOp, loadCaseFactors: resolvedLoadCaseFactors },
                this._loadCombinations,
                (op, revisionId) =>
                    LoadCombinationEntity.create({
                        id: Bun.randomUUIDv7(),
                        revisionId,
                        loadCaseFactors: { ...op.loadCaseFactors },
                    }),
                "load combination",
            );
        }

        for (const updateOp of ops.update ?? []) {
            const existingId = this.getExistingEntityIdByIdOrApplicationId(
                this._loadCombinations,
                updateOp.id,
            );
            this.applyUpdateOp(
                updateOp,
                existingId,
                this._loadCombinations,
                (op, revisionId, existingEntityId) =>
                    LoadCombinationEntity.create({
                        id: existingEntityId,
                        revisionId,
                        loadCaseFactors: { ...op.loadCaseFactors },
                    }),
                "load combination",
            );
        }
    }

    public applyPointLoadChanges(ops: PointLoadOperationsRequest) {
        for (const pointLoadId of ops.delete ?? []) {
            this.deleteById(this._pointLoads, pointLoadId, "point load");
        }

        for (const createOp of ops.create ?? []) {
            const resolvedNodeId = this.resolveId(createOp.nodeId);
            const resolvedLoadCaseId = this.resolveId(createOp.loadCaseId);

            this.applyCreateOp(
                { ...createOp, nodeId: resolvedNodeId, loadCaseId: resolvedLoadCaseId },
                this._pointLoads,
                (op, revisionId) =>
                    PointLoadEntity.create({
                        id: Bun.randomUUIDv7(),
                        revisionId,
                        nodeId: op.nodeId,
                        loadCaseId: op.loadCaseId,
                        force: {
                            forceAlongX: new Force(op.force.forceAlongX, op.units.force),
                            forceAlongY: new Force(op.force.forceAlongY, op.units.force),
                            forceAlongZ: new Force(op.force.forceAlongZ, op.units.force),
                            momentAboutX: new Torque(op.force.momentAboutX, op.units.torque),
                            momentAboutY: new Torque(op.force.momentAboutY, op.units.torque),
                            momentAboutZ: new Torque(op.force.momentAboutZ, op.units.torque),
                        },
                        direction: op.direction,
                    }),
                "point load",
            );
        }

        for (const updateOp of ops.update ?? []) {
            const existingId = this.getExistingEntityIdByIdOrApplicationId(
                this._pointLoads,
                updateOp.id,
            );
            this.applyUpdateOp(
                updateOp,
                existingId,
                this._pointLoads,
                (op, revisionId, existingEntityId) =>
                    PointLoadEntity.create({
                        id: existingEntityId,
                        revisionId,
                        nodeId: op.nodeId,
                        loadCaseId: op.loadCaseId,
                        force: {
                            forceAlongX: new Force(op.force.forceAlongX, op.units.force),
                            forceAlongY: new Force(op.force.forceAlongY, op.units.force),
                            forceAlongZ: new Force(op.force.forceAlongZ, op.units.force),
                            momentAboutX: new Torque(op.force.momentAboutX, op.units.torque),
                            momentAboutY: new Torque(op.force.momentAboutY, op.units.torque),
                            momentAboutZ: new Torque(op.force.momentAboutZ, op.units.torque),
                        },
                        direction: op.direction,
                    }),
                "point load",
            );
        }
    }

    public applyModelSettingsUpdate(settings: ModelSettingsUpdateRequest) {
        this._modelSettings = {
            status: "updated",
            entity: ModelSettingsEntity.create({
                id: this._modelSettings.entity.id,
                revisionId: this.id,
                units: settings.units,
                yAxisUp: settings.yAxisUp,
            }),
        };
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
            throw httpError(`Cannot delete ${entityTypeName} with id ${entityId} - not found`, 404);
        }
        bucket.unchanged.delete(entityId);
        bucket.updated.delete(entityId);
        bucket.created.delete(entityId);
        bucket.deleted.add(entityId);
    }

    private getExistingEntityIdByIdOrApplicationId<
        TEntity extends { id?: string; applicationId?: string },
    >(bucket: EntityBucket<TEntity>, id?: string, applicationId?: string): string {
        if (!id && !applicationId) {
            throw httpError("Either id or applicationId must be provided", 400);
        }

        const existingIdByAppId = applicationId
            ? bucket.applicationIdToEntityIds.get(applicationId)
            : undefined;

        if (id && applicationId) {
            if (existingIdByAppId !== id) {
                throw httpError(
                    `id "${id}" and applicationId "${applicationId}" resolved to different entities"`,
                    409,
                );
            }
        }
        id = id ?? existingIdByAppId;
        if (!id) {
            throw httpError(
                `Cannot find existing entity with id "${id}" or applicationId "${applicationId}"`,
                404,
            );
        }
        return id;
    }

    private getExistingEntityIdByNameOrApplicationId<TEntity extends { id: string; name: string }>(
        bucket: EntityBucket<TEntity>,
        name: string,
        applicationId?: string,
    ): string {
        const existingEntity =
            Array.from(bucket.unchanged.values()).find(
                (entity) => entity.name.toLowerCase() === name.toLowerCase(),
            ) ??
            Array.from(bucket.updated.values()).find(
                (entity) => entity.name.toLowerCase() === name.toLowerCase(),
            ) ??
            Array.from(bucket.created.values()).find(
                (entity) => entity.name.toLowerCase() === name.toLowerCase(),
            );

        let existingId: string | undefined;
        if (applicationId) {
            existingId = bucket.applicationIdToEntityIds.get(applicationId);
        }

        if (name && applicationId) {
            if (existingId !== existingEntity?.id) {
                throw httpError(
                    `Name "${name}" and applicationId "${applicationId}" resolved to different entities"`,
                    409,
                );
            }
        }
        existingId = existingId ?? existingEntity?.id;

        if (!existingId) {
            throw httpError(
                `Cannot find existing entity with name "${name}" or applicationId "${applicationId}"`,
                404,
            );
        }
        return existingId;
    }

    private applyCreateOp<
        TCreateOp extends object,
        TEntity extends { id: string; applicationId?: string },
    >(
        createOp: TCreateOp,
        bucket: EntityBucket<TEntity>,
        createFunction: (createOp: TCreateOp, revisionId: string) => TEntity,
        entityTypeName: string,
    ): void {
        const entity = createFunction(createOp, this.id);
        if (entity.applicationId) {
            if (bucket.applicationIdToEntityIds.has(entity.applicationId)) {
                throw httpError(
                    `Duplicate applicationId "${entity.applicationId}" in ${entityTypeName}`,
                    409,
                );
            }
            bucket.applicationIdToEntityIds.set(entity.applicationId, entity.id);
        }

        bucket.created.set(entity.id, entity);

        const tempId = "tempId" in createOp ? (createOp as { tempId?: unknown }).tempId : undefined;

        if (typeof tempId === "string" && tempId.length > 0) {
            if (this.tempIdToRealIdMap.has(tempId)) {
                throw httpError(`Duplicate tempId "${tempId}" in ${entityTypeName}`, 409);
            }
            this.tempIdToRealIdMap.set(tempId, entity.id);
        }
    }

    private applyUpdateOp<TUpdateOp, TEntity extends { id: string }>(
        updateOp: TUpdateOp,
        existingEntityId: string,
        bucket: EntityBucket<TEntity>,
        updateFunction: (
            updateOp: TUpdateOp,
            revisionId: string,
            existingEntityId: string,
        ) => TEntity,
        entityTypeName: string,
    ): void {
        if (bucket.deleted.has(existingEntityId)) {
            throw httpError(
                `Cannot update ${entityTypeName} with id ${existingEntityId} - it is marked for deletion`,
                409,
            );
        }

        if (!bucket.unchanged.has(existingEntityId) && !bucket.updated.has(existingEntityId)) {
            throw httpError(
                `Cannot update ${entityTypeName} with id ${existingEntityId} - not found`,
                404,
            );
        }

        const entity = updateFunction(updateOp, this.id, existingEntityId);
        bucket.updated.set(entity.id, entity);
        bucket.unchanged.delete(existingEntityId);
    }
}
