import { and, eq, inArray } from "drizzle-orm";
import { getDb } from "../db/client";
import { modelBranchHeads, modelRevisions, revisionChanges } from "../db/schema";
import {
    ModelRevisionAggregate,
    type ModelRevisionEntityBuckets,
    type ModelRevisionSingleEntityState,
} from "./model-revision-aggregate";
import { ModelEntityCodec, revisionChangeCodecRegistry } from "./codec-registry";
import { NodeEntity } from "src/nodes/node-entity";
import { MaterialEntity } from "src/materials/material-entity";
import { SectionProfileEntity } from "src/section-profiles/section-profile-entity";
import { Element1dEntity } from "src/element1ds/element1d-entity";
import { LoadCaseEntity } from "src/load-cases/load-case-entity";
import { LoadCombinationEntity } from "src/load-combinations/load-combination-entity";
import { PointLoadEntity } from "src/point-loads/point-load-entity";

type RevisionChangeInsertRow = typeof revisionChanges.$inferInsert;
export type ModelRevisionRepository = {
    load: (projectId: string, branchName: string) => Promise<ModelRevisionAggregate | undefined>;
    save: (revision: ModelRevisionAggregate) => Promise<ModelRevisionAggregate>;
};

export const drizzleModelRevisionRepository: ModelRevisionRepository = {
    async load(projectId, branchName) {
        const branchRows = await getDb()
            .select()
            .from(modelBranchHeads)
            .where(
                and(
                    eq(modelBranchHeads.projectId, projectId),
                    eq(modelBranchHeads.branchName, branchName),
                ),
            )
            .limit(1);

        const branch = branchRows[0];
        if (!branch) {
            return undefined;
        }

        const revisions = await loadRevisionHistory({
            revisionId: branch.headRevisionId,
        });

        const headRevision = revisions.find((revision) => revision.id === branch.headRevisionId);
        if (!headRevision) {
            return undefined;
        }

        const changeRows = await loadOrderedRevisionChanges(revisions);
        const nodesById = new Map<string, NodeEntity>();
        const materialsById = new Map<string, MaterialEntity>();
        const sectionProfilesById = new Map<string, SectionProfileEntity>();
        const element1dsById = new Map<string, Element1dEntity>();
        const loadCasesById = new Map<string, LoadCaseEntity>();
        const loadCombinationsById = new Map<string, LoadCombinationEntity>();
        const pointLoadsById = new Map<string, PointLoadEntity>();
        let modelSettings: ReturnType<
            typeof revisionChangeCodecRegistry.model_settings.toDomain
        > | null = null;

        for (const row of changeRows) {
            if (isDeleteOperation(row.op)) {
                switch (row.entityType) {
                    case revisionChangeCodecRegistry.node.entityType:
                        nodesById.delete(row.entityId);
                        break;
                    case revisionChangeCodecRegistry.material.entityType:
                        materialsById.delete(row.entityId);
                        break;
                    case revisionChangeCodecRegistry.model_settings.entityType:
                        throw new Error("Model settings cannot be deleted");
                    case revisionChangeCodecRegistry.section_profile.entityType:
                        sectionProfilesById.delete(row.entityId);
                        break;
                    case revisionChangeCodecRegistry.element1d.entityType:
                        element1dsById.delete(row.entityId);
                        break;
                    case revisionChangeCodecRegistry.loadcase.entityType:
                        loadCasesById.delete(row.entityId);
                        break;
                    case revisionChangeCodecRegistry.loadcombination.entityType:
                        loadCombinationsById.delete(row.entityId);
                        break;
                    case revisionChangeCodecRegistry.pointload.entityType:
                        pointLoadsById.delete(row.entityId);
                        break;
                }
                continue;
            }

            switch (row.entityType) {
                case revisionChangeCodecRegistry.node.entityType:
                    nodesById.set(
                        row.entityId,
                        revisionChangeCodecRegistry.node.toDomain(
                            row.revisionId,
                            row.entityId,
                            row.schemaVersion,
                            row.payload,
                        ),
                    );
                    break;
                case revisionChangeCodecRegistry.material.entityType:
                    materialsById.set(
                        row.entityId,
                        revisionChangeCodecRegistry.material.toDomain(
                            row.revisionId,
                            row.entityId,
                            row.schemaVersion,
                            row.payload,
                        ),
                    );
                    break;
                case revisionChangeCodecRegistry.model_settings.entityType:
                    modelSettings = revisionChangeCodecRegistry.model_settings.toDomain(
                        row.revisionId,
                        row.entityId,
                        row.schemaVersion,
                        row.payload,
                    );
                    break;
                case revisionChangeCodecRegistry.section_profile.entityType:
                    sectionProfilesById.set(
                        row.entityId,
                        revisionChangeCodecRegistry.section_profile.toDomain(
                            row.revisionId,
                            row.entityId,
                            row.schemaVersion,
                            row.payload,
                        ),
                    );
                    break;
                case revisionChangeCodecRegistry.element1d.entityType:
                    element1dsById.set(
                        row.entityId,
                        revisionChangeCodecRegistry.element1d.toDomain(
                            row.revisionId,
                            row.entityId,
                            row.schemaVersion,
                            row.payload,
                        ),
                    );
                    break;
                case revisionChangeCodecRegistry.loadcase.entityType:
                    loadCasesById.set(
                        row.entityId,
                        revisionChangeCodecRegistry.loadcase.toDomain(
                            row.revisionId,
                            row.entityId,
                            row.schemaVersion,
                            row.payload,
                        ),
                    );
                    break;
                case revisionChangeCodecRegistry.loadcombination.entityType:
                    loadCombinationsById.set(
                        row.entityId,
                        revisionChangeCodecRegistry.loadcombination.toDomain(
                            row.revisionId,
                            row.entityId,
                            row.schemaVersion,
                            row.payload,
                        ),
                    );
                    break;
                case revisionChangeCodecRegistry.pointload.entityType:
                    pointLoadsById.set(
                        row.entityId,
                        revisionChangeCodecRegistry.pointload.toDomain(
                            row.revisionId,
                            row.entityId,
                            row.schemaVersion,
                            row.payload,
                        ),
                    );
                    break;
                default:
                    throw new Error(`Unknown entity type: ${row.entityType}`);
            }
        }

        if (!modelSettings) {
            throw new Error(
                `Model settings were not found in revision history for revision ${headRevision.id}`,
            );
        }

        const rehydrated = ModelRevisionAggregate.rehydrate({
            id: headRevision.id,
            projectId: headRevision.projectId,
            parentRevisionId: headRevision.parentRevisionId,
            secondParentRevisionId: headRevision.secondParentRevisionId,
            authorId: headRevision.authorId,
            message: headRevision.message,
            createdAt: headRevision.createdAt,
            nodes: Array.from(nodesById.values()),
            materials: Array.from(materialsById.values()),
            modelSettings,
            sectionProfiles: Array.from(sectionProfilesById.values()),
            element1ds: Array.from(element1dsById.values()),
            loadCases: Array.from(loadCasesById.values()),
            loadCombinations: Array.from(loadCombinationsById.values()),
            pointLoads: Array.from(pointLoadsById.values()),
        });

        rehydrated.markChangesAsPersisted();
        return rehydrated;
    },

    async save(revision) {
        const snapshot = revision.toSnapshot();
        const changeRows = buildRevisionChangeRowsFromTracking(revision);

        await getDb().transaction(async (tx) => {
            await tx.insert(modelRevisions).values({
                id: snapshot.id,
                projectId: snapshot.projectId,
                parentRevisionId: snapshot.parentRevisionId,
                secondParentRevisionId: snapshot.secondParentRevisionId,
                authorId: snapshot.authorId,
                message: snapshot.message,
                createdAt: snapshot.createdAt,
            });

            if (changeRows.length > 0) {
                await tx.insert(revisionChanges).values(changeRows);
            }
        });

        revision.markChangesAsPersisted();
        return revision;
    },
};

const loadRevisionHistory = async (input: {
    revisionId?: string | null;
    secondRevisionId?: string | null;
}): Promise<(typeof modelRevisions.$inferSelect)[]> => {
    const visited = new Set<string>();
    const queue: string[] = [];

    if (input.revisionId) {
        queue.push(input.revisionId);
    }
    if (input.secondRevisionId) {
        queue.push(input.secondRevisionId);
    }

    const revisions: (typeof modelRevisions.$inferSelect)[] = [];

    while (queue.length > 0) {
        const id = queue.shift();
        if (!id || visited.has(id)) {
            continue;
        }
        visited.add(id);

        const rows = await getDb()
            .select()
            .from(modelRevisions)
            .where(eq(modelRevisions.id, id))
            .limit(1);

        if (!rows[0]) {
            continue;
        }

        const revision = rows[0];
        revisions.push(revision);

        if (revision.parentRevisionId) {
            queue.push(revision.parentRevisionId);
        }
        if (revision.secondParentRevisionId) {
            queue.push(revision.secondParentRevisionId);
        }
    }

    return revisions.sort((a, b) => {
        const timeDiff = a.createdAt.getTime() - b.createdAt.getTime();
        if (timeDiff !== 0) {
            return timeDiff;
        }
        return a.id.localeCompare(b.id);
    });
};

const loadOrderedRevisionChanges = async (
    revisions: (typeof modelRevisions.$inferSelect)[],
): Promise<(typeof revisionChanges.$inferSelect)[]> => {
    const revisionIds = revisions.map((revision) => revision.id);
    const revisionTimestamps = new Map(
        revisions.map((revision) => [revision.id, revision.createdAt.getTime()]),
    );
    const revisionOrder = new Map(revisions.map((revision, index) => [revision.id, index]));

    const rows = await getDb()
        .select()
        .from(revisionChanges)
        .where(inArray(revisionChanges.revisionId, revisionIds));

    rows.sort((a, b) => {
        const orderA = revisionOrder.get(a.revisionId) ?? 0;
        const orderB = revisionOrder.get(b.revisionId) ?? 0;
        if (orderA !== orderB) {
            return orderA - orderB;
        }
        const timeDiff =
            (revisionTimestamps.get(a.revisionId) ?? 0) -
            (revisionTimestamps.get(b.revisionId) ?? 0);
        if (timeDiff !== 0) {
            return timeDiff;
        }
        return a.id.localeCompare(b.id);
    });

    return rows;
};

const isDeleteOperation = (op: string): boolean => {
    return op === "delete" || op === "deleted";
};

const buildRevisionChangeRowsFromTracking = (
    revision: ModelRevisionAggregate,
): RevisionChangeInsertRow[] => {
    const now = new Date();
    const rows: RevisionChangeInsertRow[] = [];

    appendBucketChangeRows({
        rows,
        revisionId: revision.id,
        bucket: revision.nodeBuckets,
        codec: revisionChangeCodecRegistry.node,
        createdAt: now,
    });

    appendBucketChangeRows({
        rows,
        revisionId: revision.id,
        bucket: revision.materialBuckets,
        codec: revisionChangeCodecRegistry.material,
        createdAt: now,
    });

    appendSingleEntityChangeRow({
        rows,
        revisionId: revision.id,
        state: revision.modelSettingsState,
        codec: revisionChangeCodecRegistry.model_settings,
        createdAt: now,
    });

    appendBucketChangeRows({
        rows,
        revisionId: revision.id,
        bucket: revision.sectionProfileBuckets,
        codec: revisionChangeCodecRegistry.section_profile,
        createdAt: now,
    });

    appendBucketChangeRows({
        rows,
        revisionId: revision.id,
        bucket: revision.element1dBuckets,
        codec: revisionChangeCodecRegistry.element1d,
        createdAt: now,
    });

    appendBucketChangeRows({
        rows,
        revisionId: revision.id,
        bucket: revision.loadCaseBuckets,
        codec: revisionChangeCodecRegistry.loadcase,
        createdAt: now,
    });

    appendBucketChangeRows({
        rows,
        revisionId: revision.id,
        bucket: revision.loadCombinationBuckets,
        codec: revisionChangeCodecRegistry.loadcombination,
        createdAt: now,
    });

    appendBucketChangeRows({
        rows,
        revisionId: revision.id,
        bucket: revision.pointLoadBuckets,
        codec: revisionChangeCodecRegistry.pointload,
        createdAt: now,
    });

    return rows;
};

const appendBucketChangeRows = <TEntity extends { id: string }, TEntityType extends string>(input: {
    rows: RevisionChangeInsertRow[];
    revisionId: string;
    bucket: ModelRevisionEntityBuckets<TEntity>;
    codec: ModelEntityCodec<TEntity, TEntityType>;
    createdAt: Date;
}): void => {
    for (const entity of input.bucket.created.values()) {
        const { schemaVersion, payload } = input.codec.toPersistencePayload(entity);
        input.rows.push({
            id: crypto.randomUUID(),
            revisionId: input.revisionId,
            entityType: input.codec.entityType,
            entityId: entity.id,
            schemaVersion,
            op: "created",
            payload,
        });
    }

    for (const entity of input.bucket.updated.values()) {
        const { schemaVersion, payload } = input.codec.toPersistencePayload(entity);
        input.rows.push({
            id: crypto.randomUUID(),
            revisionId: input.revisionId,
            entityType: input.codec.entityType,
            entityId: entity.id,
            schemaVersion,
            op: "updated",
            payload,
        });
    }

    for (const entityId of input.bucket.deleted.values()) {
        input.rows.push({
            id: crypto.randomUUID(),
            revisionId: input.revisionId,
            entityType: input.codec.entityType,
            entityId,
            schemaVersion: input.codec.currentSchemaVersion,
            op: "deleted",
            payload: undefined,
        });
    }
};

const appendSingleEntityChangeRow = <
    TEntity extends { id: string },
    TEntityType extends string,
>(input: {
    rows: RevisionChangeInsertRow[];
    revisionId: string;
    state: ModelRevisionSingleEntityState<TEntity>;
    codec: ModelEntityCodec<TEntity, TEntityType>;
    createdAt: Date;
}): void => {
    if (input.state.status === "unchanged") {
        return;
    }

    const { schemaVersion, payload } = input.codec.toPersistencePayload(input.state.entity);

    input.rows.push({
        id: crypto.randomUUID(),
        revisionId: input.revisionId,
        entityType: input.codec.entityType,
        entityId: input.state.entity.id,
        schemaVersion,
        op: input.state.status,
        payload,
    });
};
