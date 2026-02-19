import {
    AreaMomentOfInertiaUnits,
    AreaUnits,
    ForceUnits,
    Ratio,
    TorqueUnits,
    VolumeUnits,
    WarpingMomentOfInertiaUnits,
} from "unitsnet-js";
import { getDb } from "../db/client";
import { modelRevisions, revisionChanges } from "../db/schema";
import { Element1dEntity } from "../element1ds/element1d-entity";
import { LoadCaseEntity } from "../load-cases/load-case-entity";
import { LoadCombinationEntity } from "../load-combinations/load-combination-entity";
import { ModelSettingsEntity } from "../model-settings/model-settings-entity";
import { NodeEntity } from "../nodes/node-entity";
import { PointLoadEntity } from "../point-loads/point-load-entity";
import type { RevisionChangeInsertRow } from "../revision-changes/revision-change-mapper";
import { SectionProfileEntity } from "../section-profiles/section-profile-entity";
import {
    ModelRevisionAggregate2,
    type ModelRevisionEntityBuckets,
} from "./model-revision-aggregate2";
import { ModelEntityCodec, revisionChangeCodecRegistry } from "./codec-registry";

export type ModelRevisionAggregate2Repository = {
    Save: (revision: ModelRevisionAggregate2) => Promise<ModelRevisionAggregate2>;
};

export const drizzleModelRevisionAggregate2Repository: ModelRevisionAggregate2Repository = {
    async Save(revision) {
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
        entityType: "node",
        bucket: revision.nodeBuckets,
        toPayload: (entity) => toNodeRevisionChangePayload(entity),
        createdAt: now,
    });

    appendBucketChangeRows({
        rows,
        revisionId: revision.id,
        entityType: "material",
        bucket: revision.materialBuckets,
        codec: revisionChangeCodecRegistry.material,
        createdAt: now,
    });

    if (revision.modelSettingsState.status !== "unchanged") {
        rows.push({
            id: crypto.randomUUID(),
            revisionId: revision.id,
            entityType: "model_settings",
            entityId: revision.modelSettings.id,
            schemaVersion: 1,
            op: revision.modelSettingsState.status,
            payload: toModelSettingsRevisionChangePayload(revision.modelSettings),
            createdAt: now,
        });
    }

    appendBucketChangeRows({
        rows,
        revisionId: revision.id,
        entityType: "section_profile",
        bucket: revision.sectionProfileBuckets,
        toPayload: (entity) => toSectionProfileRevisionChangePayload(entity),
        createdAt: now,
    });

    appendBucketChangeRows({
        rows,
        revisionId: revision.id,
        entityType: "element1d",
        bucket: revision.element1dBuckets,
        toPayload: (entity) => toElement1dRevisionChangePayload(entity),
        createdAt: now,
    });

    appendBucketChangeRows({
        rows,
        revisionId: revision.id,
        entityType: "loadcase",
        bucket: revision.loadCaseBuckets,
        toPayload: (entity) => toLoadCaseRevisionChangePayload(entity),
        createdAt: now,
    });

    appendBucketChangeRows({
        rows,
        revisionId: revision.id,
        entityType: "loadcombination",
        bucket: revision.loadCombinationBuckets,
        toPayload: (entity) => toLoadCombinationRevisionChangePayload(entity),
        createdAt: now,
    });

    appendBucketChangeRows({
        rows,
        revisionId: revision.id,
        entityType: "pointload",
        bucket: revision.pointLoadBuckets,
        toPayload: (entity) => toPointLoadRevisionChangePayload(entity),
        createdAt: now,
    });

    return rows;
};

const appendBucketChangeRows = <TEntity extends { id: string }>(input: {
    rows: RevisionChangeInsertRow[];
    revisionId: string;
    entityType: string;
    bucket: ModelRevisionEntityBuckets<TEntity>;
    codec: ModelEntityCodec<TEntity>;
    createdAt: Date;
}): void => {
    for (const entity of input.bucket.created.values()) {
        input.rows.push({
            id: crypto.randomUUID(),
            revisionId: input.revisionId,
            entityType: input.entityType,
            entityId: entity.id,
            schemaVersion: 1,
            op: "created",
            payload: input.codec.toPersistencePayload(entity).payload,
            createdAt: input.createdAt,
        });
    }

    for (const entity of input.bucket.updated.values()) {
        input.rows.push({
            id: crypto.randomUUID(),
            revisionId: input.revisionId,
            entityType: input.entityType,
            entityId: entity.id,
            schemaVersion: 1,
            op: "updated",
            payload: input.codec.toPersistencePayload(entity).payload,
            createdAt: input.createdAt,
        });
    }

    for (const entityId of input.bucket.deleted.values()) {
        input.rows.push({
            id: crypto.randomUUID(),
            revisionId: input.revisionId,
            entityType: input.entityType,
            entityId,
            schemaVersion: 1,
            op: "deleted",
            payload: undefined,
            createdAt: input.createdAt,
        });
    }
};

const toNodeRevisionChangePayload = (node: NodeEntity): Record<string, unknown> => {
    const snapshot = node.toSnapshot();

    return {
        id: snapshot.id,
        modelRevisionId: snapshot.modelRevisionId ?? null,
        nodeType: snapshot.nodeType ?? null,
        nodeTypeDescriminator: snapshot.nodeType === "spatialNode" ? "external" : "internal",
        point: snapshot.point ?? null,
        element1dId: snapshot.element1dId ?? null,
        distanceAlongElement1d:
            snapshot.distanceAlongElement1d instanceof Ratio
                ? snapshot.distanceAlongElement1d.DecimalFractions
                : null,
        restraint: snapshot.restraint ?? null,
    };
};

const toModelSettingsRevisionChangePayload = (
    modelSettings: ModelSettingsEntity,
): Record<string, unknown> => {
    const snapshot = modelSettings.toSnapshot();

    return {
        id: snapshot.id,
        revisionId: snapshot.revisionId,
        units: snapshot.units,
        yAxisUp: snapshot.yAxisUp,
    };
};

const toSectionProfileRevisionChangePayload = (
    sectionProfile: SectionProfileEntity,
): Record<string, unknown> => {
    const snapshot = sectionProfile.toSnapshot();

    return {
        id: snapshot.id,
        revisionId: snapshot.revisionId,
        name: snapshot.name,
        discriminator: snapshot.discriminator,
        area: {
            value: snapshot.area.SquareMeters,
            unit: AreaUnits.SquareMeters,
        },
        strongAxisMomentOfInertia: {
            value: snapshot.strongAxisMomentOfInertia.MetersToTheFourth,
            unit: AreaMomentOfInertiaUnits.MetersToTheFourth,
        },
        weakAxisMomentOfInertia: {
            value: snapshot.weakAxisMomentOfInertia.MetersToTheFourth,
            unit: AreaMomentOfInertiaUnits.MetersToTheFourth,
        },
        torsionalConstant: {
            value: snapshot.torsionalConstant.MetersToTheFourth,
            unit: AreaMomentOfInertiaUnits.MetersToTheFourth,
        },
        warpingConstant: {
            value: snapshot.warpingConstant.MetersToTheSixth,
            unit: WarpingMomentOfInertiaUnits.MetersToTheSixth,
        },
        strongAxisPlasticSectionModulus: {
            value: snapshot.strongAxisPlasticSectionModulus.CubicMeters,
            unit: VolumeUnits.CubicMeters,
        },
        weakAxisPlasticSectionModulus: {
            value: snapshot.weakAxisPlasticSectionModulus.CubicMeters,
            unit: VolumeUnits.CubicMeters,
        },
        strongAxisElasticSectionModulus: {
            value: snapshot.strongAxisElasticSectionModulus.CubicMeters,
            unit: VolumeUnits.CubicMeters,
        },
        weakAxisElasticSectionModulus: {
            value: snapshot.weakAxisElasticSectionModulus.CubicMeters,
            unit: VolumeUnits.CubicMeters,
        },
        ...(snapshot.strongAxisShearArea
            ? {
                  strongAxisShearArea: {
                      value: snapshot.strongAxisShearArea.SquareMeters,
                      unit: AreaUnits.SquareMeters,
                  },
              }
            : {}),
        ...(snapshot.weakAxisShearArea
            ? {
                  weakAxisShearArea: {
                      value: snapshot.weakAxisShearArea.SquareMeters,
                      unit: AreaUnits.SquareMeters,
                  },
              }
            : {}),
    };
};

const toElement1dRevisionChangePayload = (element1d: Element1dEntity): Record<string, unknown> => {
    const snapshot = element1d.toSnapshot();

    return {
        id: snapshot.id,
        revisionId: snapshot.revisionId,
        startNodeId: snapshot.startNodeId,
        endNodeId: snapshot.endNodeId,
        materialId: snapshot.materialId,
        sectionProfileId: snapshot.sectionProfileId,
    };
};

const toLoadCaseRevisionChangePayload = (loadCase: LoadCaseEntity): Record<string, unknown> => {
    const snapshot = loadCase.toSnapshot();

    return {
        id: snapshot.id,
        revisionId: snapshot.revisionId,
        name: snapshot.name,
    };
};

const toLoadCombinationRevisionChangePayload = (
    loadCombination: LoadCombinationEntity,
): Record<string, unknown> => {
    const snapshot = loadCombination.toSnapshot();

    return {
        id: snapshot.id,
        revisionId: snapshot.revisionId,
        loadCaseFactors: { ...snapshot.loadCaseFactors },
    };
};

const toPointLoadRevisionChangePayload = (pointLoad: PointLoadEntity): Record<string, unknown> => {
    const snapshot = pointLoad.toSnapshot();

    return {
        id: snapshot.id,
        revisionId: snapshot.revisionId,
        nodeId: snapshot.nodeId,
        loadCaseId: snapshot.loadCaseId,
        force: {
            forceAlongX: {
                value: snapshot.force.forceAlongX.Newtons,
                unit: ForceUnits.Newtons,
            },
            forceAlongY: {
                value: snapshot.force.forceAlongY.Newtons,
                unit: ForceUnits.Newtons,
            },
            forceAlongZ: {
                value: snapshot.force.forceAlongZ.Newtons,
                unit: ForceUnits.Newtons,
            },
            momentAboutX: {
                value: snapshot.force.momentAboutX.NewtonMeters,
                unit: TorqueUnits.NewtonMeters,
            },
            momentAboutY: {
                value: snapshot.force.momentAboutY.NewtonMeters,
                unit: TorqueUnits.NewtonMeters,
            },
            momentAboutZ: {
                value: snapshot.force.momentAboutZ.NewtonMeters,
                unit: TorqueUnits.NewtonMeters,
            },
        },
        direction: snapshot.direction,
    };
};
