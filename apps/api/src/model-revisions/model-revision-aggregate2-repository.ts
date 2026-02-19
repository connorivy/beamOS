import { getDb } from "../db/client";
import { modelRevisions, revisionChanges } from "../db/schema";
import type { RevisionChangeInsertRow } from "../revision-changes/revision-change-mapper";
import {
    ModelRevisionAggregate2,
    type ModelRevisionEntityBuckets,
    type ModelRevisionSingleEntityState,
} from "./model-revision-aggregate2";
import { ModelEntityCodec, revisionChangeCodecRegistry } from "./codec-registry";

export type ModelRevisionAggregate2Repository = {
    save: (revision: ModelRevisionAggregate2) => Promise<ModelRevisionAggregate2>;
};

export const drizzleModelRevisionAggregate2Repository: ModelRevisionAggregate2Repository = {
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

const buildRevisionChangeRowsFromTracking = (
    revision: ModelRevisionAggregate2,
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
            createdAt: input.createdAt,
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
            createdAt: input.createdAt,
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
            createdAt: input.createdAt,
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
        createdAt: input.createdAt,
    });
};
