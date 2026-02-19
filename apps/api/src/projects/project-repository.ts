import { desc, eq, inArray } from "drizzle-orm";
import { getDb } from "../db/client";
import { ModelRevisionAggregate } from "../model-revisions/model-revision-aggregate";
import {
  modelBranchHeads,
  modelRevisions,
  projects,
  revisionChanges,
} from "../db/schema";
import { modelBranchHeadMapper } from "../model-branch-heads/model-branch-head-mapper";
import { ProjectEntity } from "./project-aggregate";
import { projectMapper } from "./project-mapper";
import type { ModelSettingsUnitsSnapshot } from "../model-settings/model-settings-entity";
import {
  AreaMomentOfInertiaUnits,
  AreaUnits,
  PressureUnits,
  VolumeUnits,
  WarpingMomentOfInertiaUnits,
} from "unitsnet-js";

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
    const revisionRows = await getDb()
      .selectDistinctOn([modelRevisions.projectId])
      .from(modelRevisions)
      .where(inArray(modelRevisions.projectId, projectIds))
      .orderBy(
        modelRevisions.projectId,
        desc(modelRevisions.createdAt),
        desc(modelRevisions.id),
      );
    const latestRevisionByProjectId = new Map(
      revisionRows.map((row) => [row.projectId, row]),
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
          projectId: input.model.id,
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
        createdAt,
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
        modelBranchHeads: [
          modelBranchHeadMapper.toDomain(insertedModelBranchHead[0]),
        ],
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

const toLightweightModelRevisionAggregate = (
  input: {
    row: typeof modelRevisions.$inferSelect;
    modelSettings?: {
      id: string;
      revisionId: string;
      units: ModelSettingsUnitsSnapshot;
      yAxisUp: boolean;
    };
  },
): ModelRevisionAggregate => {
  const row = input.row;
  const modelSettings =
    input.modelSettings ??
    createDefaultModelSettingsSnapshot({
      revisionId: row.id,
    });
  return ModelRevisionAggregate.create({
    id: row.id,
    projectId: row.projectId,
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
    warpingMomentOfInertia:
      WarpingMomentOfInertiaUnits.MetersToTheSixth as const,
    volume: VolumeUnits.CubicMeters as const,
  },
  yAxisUp: true,
});
