import { desc, eq, inArray } from "drizzle-orm";
import { getDb } from "../db/client";
import { modelBranchHeads, modelRevisions, projects, revisionChanges } from "../db/schema";
import { modelBranchHeadMapper } from "../model-branch-heads/model-branch-head-mapper";
import { ProjectEntity } from "./project-aggregate";
import { projectMapper } from "./project-mapper";
import {
    ModelSettingsEntity,
    type ModelSettingsUnitsSnapshot,
} from "../model-settings/model-settings-entity";
import {
    AreaMomentOfInertiaUnits,
    AreaUnits,
    PressureUnits,
    VolumeUnits,
    WarpingMomentOfInertiaUnits,
} from "unitsnet-js";
import { ModelRevisionAggregate } from "src/model-revisions/model-revision-aggregate";

export type ProjectRepository = {
    getProjects: () => Promise<ProjectEntity[]>;
    getById: (input: {
        projectId: string;
        loadModelBranchHeadAggregates?: boolean;
    }) => Promise<ProjectEntity | undefined>;
    create: (input: {
        model: ProjectEntity;
        message: string;
        modelSettings: {
            units: ModelSettingsUnitsSnapshot;
            yAxisUp: boolean;
        };
    }) => Promise<ProjectEntity>;
    update: (input: { model: ProjectEntity }) => Promise<ProjectEntity>;
};

export const drizzleProjectRepository: ProjectRepository = {
    async getProjects() {
        const projectRows = await getDb().select().from(projects);

        if (projectRows.length === 0) {
            return [];
        }

        const projectIds = projectRows.map((row) => row.id);
        const branchHeadRows = await getDb()
            .selectDistinctOn([modelBranchHeads.projectId])
            .from(modelBranchHeads)
            .where(inArray(modelBranchHeads.projectId, projectIds))
            .orderBy(
                modelBranchHeads.projectId,
                desc(modelBranchHeads.updatedAt),
            );

        const latestRevisionIds = branchHeadRows.map((h) => h.headRevisionId);
        const latestRevisionById = new Map<string, typeof modelRevisions.$inferSelect>();
        if (latestRevisionIds.length > 0) {
            const revisionRows = await getDb()
                .select()
                .from(modelRevisions)
                .where(inArray(modelRevisions.id, latestRevisionIds));
            for (const row of revisionRows) {
                latestRevisionById.set(row.id, row);
            }
        }

        const latestRevisionByProjectId = new Map(
            branchHeadRows.map((h) => [h.projectId, latestRevisionById.get(h.headRevisionId)]),
        );

        return projectRows.map((row) => {
            const latestRevision = latestRevisionByProjectId.get(row.id);
            return ProjectEntity.rehydrate({
                ...projectMapper.toDomain(row).toSnapshot(),
                modelRevisions: latestRevision
                    ? [toLightweightModelRevisionAggregate({ row: latestRevision })]
                    : null,
            });
        });
    },

    async getById(input) {
        const projectRows = await getDb()
            .select()
            .from(projects)
            .where(eq(projects.id, input.projectId))
            .limit(1);

        if (!projectRows[0]) {
            return undefined;
        }
        const loadedModelBranchHeads = input.loadModelBranchHeadAggregates
            ? await listModelBranchHeads(input.projectId)
            : null;

        return ProjectEntity.rehydrate({
            ...projectMapper.toDomain(projectRows[0]).toSnapshot(),
            modelBranchHeads: loadedModelBranchHeads,
        });
    },

    async create(input) {
        const persistence = projectMapper.toPersistence(input.model);
        const initialRevisionId = Bun.randomUUIDv7();
        const modelSettingsId = Bun.randomUUIDv7();
        input.model.pullDomainEvents();

        const model = await getDb().transaction(async (tx) => {
            const row = await tx.insert(projects).values(persistence).returning();

            const createdAt = new Date();
            const insertedRevisions = await tx
                .insert(modelRevisions)
                .values({
                    id: initialRevisionId,
                    parentRevisionId: null,
                    secondParentRevisionId: null,
                    authorId: Bun.randomUUIDv7(),
                    message: input.message,
                    createdAt,
                })
                .returning();
            await tx.insert(revisionChanges).values({
                id: Bun.randomUUIDv7(),
                revisionId: initialRevisionId,
                entityType: "model_settings",
                entityId: modelSettingsId,
                schemaVersion: 1,
                op: "insert",
                payload: {
                    id: modelSettingsId,
                    revisionId: initialRevisionId,
                    units: input.modelSettings.units,
                    yAxisUp: input.modelSettings.yAxisUp,
                },
            });
            const insertedModelBranchHead = await tx
                .insert(modelBranchHeads)
                .values({
                    projectId: input.model.id,
                    branchName: "main",
                    headRevisionId: initialRevisionId,
                })
                .returning();

            return ProjectEntity.rehydrate({
                id: row[0].id,
                name: row[0].name,
                description: row[0].description,
                modelRevisions: [
                    toLightweightModelRevisionAggregate({
                        row: insertedRevisions[0],
                        modelSettings: {
                            id: modelSettingsId,
                            revisionId: initialRevisionId,
                            units: input.modelSettings.units,
                            yAxisUp: input.modelSettings.yAxisUp,
                        },
                    }),
                ],
                modelBranchHeads: [modelBranchHeadMapper.toDomain(insertedModelBranchHead[0])],
            });
        });

        return model;
    },

    async update(input) {
        const persistence = projectMapper.toPersistence(input.model);
        input.model.pullDomainEvents();

        const savedModel = await getDb().transaction(async (tx) => {
            const row = await tx
                .insert(projects)
                .values(persistence)
                .onConflictDoUpdate({
                    target: projects.id,
                    set: {
                        name: persistence.name,
                        description: persistence.description,
                    },
                })
                .returning();

            return ProjectEntity.rehydrate({
                id: row[0].id,
                name: row[0].name,
                description: row[0].description,
                modelBranchHeads: input.model.modelBranchHeads
                    ? [...input.model.modelBranchHeads]
                    : input.model.modelBranchHeads,
            });
        });

        return savedModel;
    },
};

const listModelBranchHeads = async (projectId: string) => {
    const rows = await getDb()
        .select()
        .from(modelBranchHeads)
        .where(eq(modelBranchHeads.projectId, projectId));
    return rows.map((row) => modelBranchHeadMapper.toDomain(row));
};

const toLightweightModelRevisionAggregate = (input: {
    row: typeof modelRevisions.$inferSelect;
    modelSettings?: {
        id: string;
        revisionId: string;
        units: ModelSettingsUnitsSnapshot;
        yAxisUp: boolean;
    };
}): ModelRevisionAggregate => {
    const row = input.row;
    const modelSettingsParams =
        input.modelSettings ??
        createDefaultModelSettingsSnapshot({
            revisionId: row.id,
        });
    const modelSettings = ModelSettingsEntity.create(modelSettingsParams);
    return ModelRevisionAggregate.create({
        id: row.id,
        parentRevisionId: row.parentRevisionId,
        secondParentRevisionId: row.secondParentRevisionId,
        authorId: row.authorId,
        message: row.message,
        createdAt: row.createdAt,
        nodes: [],
        materials: [],
        modelSettings,
        sectionProfiles: [],
        element1ds: [],
        loadCases: [],
        loadCombinations: [],
        pointLoads: [],
    });
};

const createDefaultModelSettingsSnapshot = (input: { revisionId: string }) => ({
    id: Bun.randomUUIDv7(),
    revisionId: input.revisionId,
    units: {
        pressure: PressureUnits.Pascals as const,
        area: AreaUnits.SquareMeters as const,
        areaMomentOfInertia: AreaMomentOfInertiaUnits.MetersToTheFourth as const,
        warpingMomentOfInertia: WarpingMomentOfInertiaUnits.MetersToTheSixth as const,
        volume: VolumeUnits.CubicMeters as const,
    },
    yAxisUp: true,
});
