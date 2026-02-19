import crypto from "crypto";
import { and, eq, inArray } from "drizzle-orm";
import {
  Area,
  AreaMomentOfInertia,
  AreaMomentOfInertiaUnits,
  AreaUnits,
  Force,
  ForceUnits,
  Pressure,
  PressureUnits,
  Ratio,
  Torque,
  TorqueUnits,
  Volume,
  VolumeUnits,
  WarpingMomentOfInertia,
  WarpingMomentOfInertiaUnits,
} from "unitsnet-js";
import { getDb, type DbTransaction } from "../db/client";
import {
  modelBranchHeads,
  modelRevisions,
  revisionChanges,
} from "../db/schema";
import { modelBranchHeadMapper } from "../model-branch-heads/model-branch-head-mapper";
import { ModelRevisionAggregate } from "./model-revision-aggregate";
import { modelRevisionMapper } from "./model-revision-mapper";
import { RevisionChangeEntity } from "../revision-changes/revision-change-entity";
import { revisionChangeMapper } from "../revision-changes/revision-change-mapper";
import type { RevisionChangeInsertRow } from "../revision-changes/revision-change-mapper";
import type { DomainEvent, ModelRevisionRepository } from "../common/types";
import type { MaterialSnapshot } from "../materials/material-entity";
import type { ModelSettingsSnapshot } from "../model-settings/model-settings-entity";
import type { SectionProfileSnapshot } from "../section-profiles/section-profile-entity";
import type { Element1dSnapshot } from "../element1ds/element1d-entity";
import type { NodeSnapshot } from "../nodes/node-entity";
import { parseRestraint } from "../nodes/node-entity";
import type { LoadCaseSnapshot } from "../load-cases/load-case-entity";
import type { LoadCombinationSnapshot } from "../load-combinations/load-combination-entity";
import type { PointLoadSnapshot } from "../point-loads/point-load-entity";

export const drizzleModelRevisionRepository: ModelRevisionRepository = {
  async getRevisionById(revisionId) {
    const revisionRows = await getDb()
      .select()
      .from(modelRevisions)
      .where(eq(modelRevisions.id, revisionId))
      .limit(1);

    if (!revisionRows[0]) {
      return undefined;
    }

    const revision = revisionRows[0];
    const revisions = await loadRevisionHistory({
      revisionId: revision.id,
    });
    const [
      nodesById,
      materialsById,
      modelSettings,
      sectionProfilesById,
      element1dsById,
      loadCasesById,
      loadCombinationsById,
      pointLoadsById,
    ] = await Promise.all([
      buildNodesFromRevisions({
        revisions,
      }),
      buildMaterialsFromRevisions({ revisions }),
      buildModelSettingsFromRevisions({ revisions }),
      buildSectionProfilesFromRevisions({ revisions }),
      buildElement1dsFromRevisions({ revisions }),
      buildLoadCasesFromRevisions({ revisions }),
      buildLoadCombinationsFromRevisions({ revisions }),
      buildPointLoadsFromRevisions({ revisions }),
    ]);

    return ModelRevisionAggregate.rehydrate({
      id: revision.id,
      projectId: revision.projectId,
      parentRevisionId: revision.parentRevisionId,
      secondParentRevisionId: revision.secondParentRevisionId,
      authorId: revision.authorId,
      message: revision.message,
      createdAt: revision.createdAt,
      nodes: Array.from(nodesById.values()),
      materials: Array.from(materialsById.values()),
      modelSettings,
      sectionProfiles: Array.from(sectionProfilesById.values()),
      element1ds: Array.from(element1dsById.values()),
      loadCases: Array.from(loadCasesById.values()),
      loadCombinations: Array.from(loadCombinationsById.values()),
      pointLoads: Array.from(pointLoadsById.values()),
    });
  },

  async save(input) {
    const { revision, tx: existingTx, branchName } = input;
    const snapshot = revision.toSnapshot();
    const events = revision.pullDomainEvents();

    const persist = async (tx: DbTransaction) => {
      await tx.insert(modelRevisions).values(modelRevisionMapper.toPersistence(revision));

      const changeRows = buildRevisionChangeRowsFromEvents({
        projectId: snapshot.projectId,
        revisionId: snapshot.id,
        events,
      });

      if (changeRows.length > 0) {
        await tx.insert(revisionChanges).values(changeRows);
      }

      if (branchName) {
        const branchHead = modelBranchHeadMapper.fromInput({
          projectId: snapshot.projectId,
          branchName,
          headRevisionId: snapshot.id,
        });
        const branchPersistence =
          modelBranchHeadMapper.toPersistence(branchHead);

        await tx
          .insert(modelBranchHeads)
          .values({
            projectId: branchPersistence.projectId,
            branchName: branchPersistence.branchName,
            headRevisionId: branchPersistence.headRevisionId,
          })
          .onConflictDoUpdate({
            target: [modelBranchHeads.projectId, modelBranchHeads.branchName],
            set: {
              headRevisionId: branchPersistence.headRevisionId,
              updatedAt: new Date(),
            },
          });
      }

      return ModelRevisionAggregate.rehydrate({
        ...snapshot,
      });
    };

    return existingTx ? persist(existingTx) : getDb().transaction(persist);
  },

  async getBranchHead(projectId, branchName) {
    const rows = await getDb()
      .select()
      .from(modelBranchHeads)
      .where(
        and(
          eq(modelBranchHeads.projectId, projectId),
          eq(modelBranchHeads.branchName, branchName),
        ),
      )
      .limit(1);

    if (!rows[0]) {
      return undefined;
    }

    return modelBranchHeadMapper.toDomain(rows[0]);
  },

  async listBranchHeads(projectId) {
    const rows = await getDb()
      .select()
      .from(modelBranchHeads)
      .where(eq(modelBranchHeads.projectId, projectId));

    return rows.map((row) => modelBranchHeadMapper.toDomain(row));
  },

  async createBranch(input) {
    const aggregate = modelBranchHeadMapper.fromInput(input);
    const persistence = modelBranchHeadMapper.toPersistence(aggregate);

    await getDb()
      .insert(modelBranchHeads)
      .values({
        projectId: persistence.projectId,
        branchName: persistence.branchName,
        headRevisionId: persistence.headRevisionId,
      })
      .onConflictDoUpdate({
        target: [modelBranchHeads.projectId, modelBranchHeads.branchName],
        set: {
          headRevisionId: persistence.headRevisionId,
          updatedAt: new Date(),
        },
      });
  },
};

const buildRevisionChangeRowsFromNodeOps = (input: {
  projectId: string;
  revisionId: string | null;
  nodes: {
    nodeId: string;
    name?: string;
    nodeTypeDescriminator?: "external" | "internal";
    op: "insert" | "update" | "delete";
  }[];
}): RevisionChangeInsertRow[] => {
  const now = new Date();

  return input.nodes.map((node) => {
    const entity = RevisionChangeEntity.create({
      id: crypto.randomUUID(),
      revisionId: input.revisionId,
      entityType: "node",
      entityId: node.nodeId,
      schemaVersion: 1,
      op: node.op,
      payload: {
        id: node.nodeId,
        projectId: input.projectId,
        name: node.name ?? "",
        nodeTypeDescriminator: node.nodeTypeDescriminator ?? "internal",
      },
      createdAt: now,
    });

    return revisionChangeMapper.toPersistence(entity);
  });
};

const buildRevisionChangeRowsFromEvents = (input: {
  projectId: string;
  revisionId: string | null;
  events: DomainEvent[];
}): RevisionChangeInsertRow[] => {
  const now = new Date();

  const nodeChanges = input.events
    .filter(
      (event): event is Extract<DomainEvent, { type: "node_created" }> =>
        event.type === "node_created",
    )
    .map((event) => {
      const snapshot = event.payload;
      return revisionChangeMapper.toPersistence(
        RevisionChangeEntity.create({
          id: crypto.randomUUID(),
          revisionId: input.revisionId,
          entityType: "node",
          entityId: snapshot.id,
          schemaVersion: 1,
          op: "insert",
          payload: {
            id: snapshot.id,
            modelRevisionId: snapshot.modelRevisionId,
            nodeType: snapshot.nodeType,
            nodeTypeDescriminator: snapshot.nodeTypeDescriminator,
            point: snapshot.point ?? null,
            element1dId: snapshot.element1dId ?? null,
            distanceAlongElement1d:
              snapshot.distanceAlongElement1d instanceof Ratio
                ? snapshot.distanceAlongElement1d.DecimalFractions
                : null,
            restraint: snapshot.restraint,
          },
          createdAt: now,
        }),
      );
    });

  const nodeDeleteChanges = buildRevisionChangeRowsFromNodeOps({
    projectId: input.projectId,
    revisionId: input.revisionId,
    nodes: input.events
      .filter(
        (event): event is Extract<DomainEvent, { type: "node_deleted" }> =>
          event.type === "node_deleted",
      )
      .map((event) => ({
        nodeId: event.payload.id,
        name: extractNodeName(event.payload),
        nodeTypeDescriminator: extractNodeTypeDescriminator(event.payload),
        op: "delete",
      })),
  });

  const materialChanges = input.events
    .filter(
      (event): event is Extract<DomainEvent, { type: "material_created" }> =>
        event.type === "material_created",
    )
    .map((event) =>
      revisionChangeMapper.toPersistence(
        RevisionChangeEntity.create({
          id: crypto.randomUUID(),
          revisionId: input.revisionId,
          entityType: "material",
          entityId: event.payload.id,
          schemaVersion: 1,
          op: "insert",
          payload: {
            id: event.payload.id,
            revisionId: event.payload.revisionId,
            name: event.payload.name,
            pressureE: {
              value: event.payload.pressureE.Pascals,
              unit: PressureUnits.Pascals,
            },
            pressureG: {
              value: event.payload.pressureG.Pascals,
              unit: PressureUnits.Pascals,
            },
          },
          createdAt: now,
        }),
      ),
    );

  const sectionProfileChanges = input.events
    .filter(
      (
        event,
      ): event is Extract<DomainEvent, { type: "section_profile_created" }> =>
        event.type === "section_profile_created",
    )
    .map((event) =>
      revisionChangeMapper.toPersistence(
        RevisionChangeEntity.create({
          id: crypto.randomUUID(),
          revisionId: input.revisionId,
          entityType: "section_profile",
          entityId: event.payload.id,
          schemaVersion: 1,
          op: "insert",
          payload: {
            id: event.payload.id,
            revisionId: event.payload.revisionId,
            name: event.payload.name,
            discriminator: event.payload.discriminator,
            area: {
              value: event.payload.area.SquareMeters,
              unit: AreaUnits.SquareMeters,
            },
            strongAxisMomentOfInertia: {
              value: event.payload.strongAxisMomentOfInertia.MetersToTheFourth,
              unit: AreaMomentOfInertiaUnits.MetersToTheFourth,
            },
            weakAxisMomentOfInertia: {
              value: event.payload.weakAxisMomentOfInertia.MetersToTheFourth,
              unit: AreaMomentOfInertiaUnits.MetersToTheFourth,
            },
            torsionalConstant: {
              value: event.payload.torsionalConstant.MetersToTheFourth,
              unit: AreaMomentOfInertiaUnits.MetersToTheFourth,
            },
            warpingConstant: {
              value: event.payload.warpingConstant.MetersToTheSixth,
              unit: WarpingMomentOfInertiaUnits.MetersToTheSixth,
            },
            strongAxisPlasticSectionModulus: {
              value: event.payload.strongAxisPlasticSectionModulus.CubicMeters,
              unit: VolumeUnits.CubicMeters,
            },
            weakAxisPlasticSectionModulus: {
              value: event.payload.weakAxisPlasticSectionModulus.CubicMeters,
              unit: VolumeUnits.CubicMeters,
            },
            strongAxisElasticSectionModulus: {
              value: event.payload.strongAxisElasticSectionModulus.CubicMeters,
              unit: VolumeUnits.CubicMeters,
            },
            weakAxisElasticSectionModulus: {
              value: event.payload.weakAxisElasticSectionModulus.CubicMeters,
              unit: VolumeUnits.CubicMeters,
            },
            ...(event.payload.strongAxisShearArea
              ? {
                  strongAxisShearArea: {
                    value: event.payload.strongAxisShearArea.SquareMeters,
                    unit: AreaUnits.SquareMeters,
                  },
                }
              : {}),
            ...(event.payload.weakAxisShearArea
              ? {
                  weakAxisShearArea: {
                    value: event.payload.weakAxisShearArea.SquareMeters,
                    unit: AreaUnits.SquareMeters,
                  },
                }
              : {}),
          },
          createdAt: now,
        }),
      ),
    );

  const modelSettingsChanges = input.events
    .filter(
      (
        event,
      ): event is Extract<DomainEvent, { type: "model_settings_created" }> =>
        event.type === "model_settings_created",
    )
    .map((event) =>
      revisionChangeMapper.toPersistence(
        RevisionChangeEntity.create({
          id: crypto.randomUUID(),
          revisionId: input.revisionId,
          entityType: "model_settings",
          entityId: event.payload.id,
          schemaVersion: 1,
          op: "insert",
          payload: {
            id: event.payload.id,
            revisionId: event.payload.revisionId,
            units: event.payload.units,
            yAxisUp: event.payload.yAxisUp,
          },
          createdAt: now,
        }),
      ),
    );

  const element1dChanges = input.events
    .filter(
      (event): event is Extract<DomainEvent, { type: "element1d_created" }> =>
        event.type === "element1d_created",
    )
    .map((event) =>
      revisionChangeMapper.toPersistence(
        RevisionChangeEntity.create({
          id: crypto.randomUUID(),
          revisionId: input.revisionId,
          entityType: "element1d",
          entityId: event.payload.id,
          schemaVersion: 1,
          op: "insert",
          payload: {
            id: event.payload.id,
            revisionId: event.payload.revisionId,
            startNodeId: event.payload.startNodeId,
            endNodeId: event.payload.endNodeId,
            materialId: event.payload.materialId,
            sectionProfileId: event.payload.sectionProfileId,
          },
          createdAt: now,
        }),
      ),
    );

  const loadCaseChanges = input.events
    .filter(
      (event): event is Extract<DomainEvent, { type: "load_case_created" }> =>
        event.type === "load_case_created",
    )
    .map((event) =>
      revisionChangeMapper.toPersistence(
        RevisionChangeEntity.create({
          id: crypto.randomUUID(),
          revisionId: input.revisionId,
          entityType: "loadcase",
          entityId: event.payload.id,
          schemaVersion: 1,
          op: "insert",
          payload: {
            id: event.payload.id,
            revisionId: event.payload.revisionId,
            name: event.payload.name,
          },
          createdAt: now,
        }),
      ),
    );

  const loadCombinationChanges = input.events
    .filter(
      (
        event,
      ): event is Extract<DomainEvent, { type: "load_combination_created" }> =>
        event.type === "load_combination_created",
    )
    .map((event) =>
      revisionChangeMapper.toPersistence(
        RevisionChangeEntity.create({
          id: crypto.randomUUID(),
          revisionId: input.revisionId,
          entityType: "loadcombination",
          entityId: event.payload.id,
          schemaVersion: 1,
          op: "insert",
          payload: {
            id: event.payload.id,
            revisionId: event.payload.revisionId,
            loadCaseFactors: { ...event.payload.loadCaseFactors },
          },
          createdAt: now,
        }),
      ),
    );

  const pointLoadChanges = input.events
    .filter(
      (event): event is Extract<DomainEvent, { type: "point_load_created" }> =>
        event.type === "point_load_created",
    )
    .map((event) =>
      revisionChangeMapper.toPersistence(
        RevisionChangeEntity.create({
          id: crypto.randomUUID(),
          revisionId: input.revisionId,
          entityType: "pointload",
          entityId: event.payload.id,
          schemaVersion: 1,
          op: "insert",
          payload: {
            id: event.payload.id,
            revisionId: event.payload.revisionId,
            nodeId: event.payload.nodeId,
            loadCaseId: event.payload.loadCaseId,
            force: {
              forceAlongX: {
                value: event.payload.force.forceAlongX.Newtons,
                unit: ForceUnits.Newtons,
              },
              forceAlongY: {
                value: event.payload.force.forceAlongY.Newtons,
                unit: ForceUnits.Newtons,
              },
              forceAlongZ: {
                value: event.payload.force.forceAlongZ.Newtons,
                unit: ForceUnits.Newtons,
              },
              momentAboutX: {
                value: event.payload.force.momentAboutX.NewtonMeters,
                unit: TorqueUnits.NewtonMeters,
              },
              momentAboutY: {
                value: event.payload.force.momentAboutY.NewtonMeters,
                unit: TorqueUnits.NewtonMeters,
              },
              momentAboutZ: {
                value: event.payload.force.momentAboutZ.NewtonMeters,
                unit: TorqueUnits.NewtonMeters,
              },
            },
            direction: event.payload.direction,
          },
          createdAt: now,
        }),
      ),
    );

  return [
    ...nodeChanges,
    ...nodeDeleteChanges,
    ...materialChanges,
    ...modelSettingsChanges,
    ...sectionProfileChanges,
    ...element1dChanges,
    ...loadCaseChanges,
    ...loadCombinationChanges,
    ...pointLoadChanges,
  ];
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

const buildNodesFromRevisions = async (input: {
  revisions: (typeof modelRevisions.$inferSelect)[];
}): Promise<Map<string, NodeSnapshot>> => {
  if (input.revisions.length === 0) {
    return new Map();
  }

  const rows = await loadOrderedRevisionChanges(input.revisions);

  const nodesById = new Map<string, NodeSnapshot>();
  for (const row of rows) {
    if (row.entityType !== "node") {
      continue;
    }
    if (row.op === "delete") {
      nodesById.delete(row.entityId);
      continue;
    }

    nodesById.set(
      row.entityId,
      toNodeSnapshotFromRevisionChange({
        row,
      }),
    );
  }

  return nodesById;
};

const toNodeSnapshotFromRevisionChange = (input: {
  row: typeof revisionChanges.$inferSelect;
}): NodeSnapshot => {
  const payload = toObject(input.row.payload);
  const nodeTypeDescriminator = extractNodeTypeDescriminator(payload);
  const modelRevisionId =
    typeof payload.modelRevisionId === "string"
      ? payload.modelRevisionId
      : (input.row.revisionId ?? "");
  const restraint = parseRestraint(payload.restraint);

  if (nodeTypeDescriminator === "internal") {
    const distanceAlongElement1d = toFiniteNumber(
      payload.distanceAlongElement1d,
    );
    return {
      id: input.row.entityId,
      modelRevisionId,
      nodeType: "internalNode",
      nodeTypeDescriminator: "internal",
      element1dId:
        typeof payload.element1dId === "string"
          ? payload.element1dId
          : input.row.entityId,
      distanceAlongElement1d: Ratio.FromDecimalFractions(
        distanceAlongElement1d ?? 0,
      ),
      restraint,
    };
  }

  const point = toObject(payload.point);
  return {
    id: input.row.entityId,
    modelRevisionId,
    nodeType: "spatialNode",
    nodeTypeDescriminator: "external",
    point: {
      x: toFiniteNumber(point.x) ?? 0,
      y: toFiniteNumber(point.y) ?? 0,
      z: toFiniteNumber(point.z) ?? 0,
    },
    restraint,
  };
};

const loadOrderedRevisionChanges = async (
  revisions: (typeof modelRevisions.$inferSelect)[],
): Promise<(typeof revisionChanges.$inferSelect)[]> => {
  const revisionIds = revisions.map((revision) => revision.id);
  const revisionOrder = new Map(
    revisions.map((revision, index) => [revision.id, index]),
  );

  const rows = await getDb()
    .select()
    .from(revisionChanges)
    .where(inArray(revisionChanges.revisionId, revisionIds));

  rows.sort((a, b) => {
    const orderA = revisionOrder.get(a.revisionId ?? "") ?? 0;
    const orderB = revisionOrder.get(b.revisionId ?? "") ?? 0;
    if (orderA !== orderB) {
      return orderA - orderB;
    }
    const timeDiff = a.createdAt.getTime() - b.createdAt.getTime();
    if (timeDiff !== 0) {
      return timeDiff;
    }
    return a.id.localeCompare(b.id);
  });

  return rows;
};

const toObject = (value: unknown): Record<string, unknown> => {
  if (!value || typeof value !== "object") {
    return {};
  }
  return value as Record<string, unknown>;
};

const toFiniteNumber = (value: unknown): number | undefined => {
  return typeof value === "number" && Number.isFinite(value)
    ? value
    : undefined;
};

const buildMaterialsFromRevisions = async (input: {
  revisions: (typeof modelRevisions.$inferSelect)[];
}): Promise<Map<string, MaterialSnapshot>> => {
  if (input.revisions.length === 0) {
    return new Map();
  }

  const rows = await loadOrderedRevisionChanges(input.revisions);
  const materialsById = new Map<string, MaterialSnapshot>();
  for (const row of rows) {
    if (row.entityType !== "material") {
      continue;
    }
    if (row.op === "delete") {
      materialsById.delete(row.entityId);
      continue;
    }

    const payload = toObject(row.payload);
    const pressureE = toObject(payload.pressureE);
    const pressureG = toObject(payload.pressureG);
    const pressureEValue = toFiniteNumber(pressureE.value);
    const pressureGValue = toFiniteNumber(pressureG.value);
    const name =
      typeof payload.name === "string" && payload.name.length > 0
        ? payload.name
        : "Unnamed Material";
    if (pressureEValue === undefined || pressureGValue === undefined) {
      continue;
    }

    materialsById.set(row.entityId, {
      id: row.entityId,
      revisionId:
        typeof payload.revisionId === "string"
          ? payload.revisionId
          : (row.revisionId ?? ""),
      name,
      pressureE: Pressure.FromPascals(pressureEValue),
      pressureG: Pressure.FromPascals(pressureGValue),
    });
  }

  return materialsById;
};

const buildModelSettingsFromRevisions = async (input: {
  revisions: (typeof modelRevisions.$inferSelect)[];
}): Promise<ModelSettingsSnapshot> => {
  if (input.revisions.length === 0) {
    throw new Error("Cannot build model settings without revision history");
  }

  const rows = await loadOrderedRevisionChanges(input.revisions);
  let latestModelSettings: ModelSettingsSnapshot | null = null;
  for (const row of rows) {
    if (row.entityType !== "model_settings") {
      continue;
    }
    if (row.op === "delete") {
      latestModelSettings = null;
      continue;
    }

    const payload = toObject(row.payload);
    const units = toObject(payload.units);
    const pressure =
      typeof units.pressure === "string" &&
      Object.values(PressureUnits).includes(units.pressure as PressureUnits)
        ? (units.pressure as PressureUnits)
        : PressureUnits.Pascals;
    const area =
      typeof units.area === "string" &&
      Object.values(AreaUnits).includes(units.area as AreaUnits)
        ? (units.area as AreaUnits)
        : AreaUnits.SquareMeters;
    const areaMomentOfInertia =
      typeof units.areaMomentOfInertia === "string" &&
      Object.values(AreaMomentOfInertiaUnits).includes(
        units.areaMomentOfInertia as AreaMomentOfInertiaUnits,
      )
        ? (units.areaMomentOfInertia as AreaMomentOfInertiaUnits)
        : AreaMomentOfInertiaUnits.MetersToTheFourth;
    const warpingMomentOfInertia =
      typeof units.warpingMomentOfInertia === "string" &&
      Object.values(WarpingMomentOfInertiaUnits).includes(
        units.warpingMomentOfInertia as WarpingMomentOfInertiaUnits,
      )
        ? (units.warpingMomentOfInertia as WarpingMomentOfInertiaUnits)
        : WarpingMomentOfInertiaUnits.MetersToTheSixth;
    const volume =
      typeof units.volume === "string" &&
      Object.values(VolumeUnits).includes(units.volume as VolumeUnits)
        ? (units.volume as VolumeUnits)
        : VolumeUnits.CubicMeters;

    latestModelSettings = {
      id: row.entityId,
      revisionId:
        typeof payload.revisionId === "string"
          ? payload.revisionId
          : (row.revisionId ?? ""),
      units: {
        pressure,
        area,
        areaMomentOfInertia,
        warpingMomentOfInertia,
        volume,
      },
      yAxisUp: payload.yAxisUp === false ? false : true,
    };
  }

  if (!latestModelSettings) {
    throw new Error(
      `Model settings were not found in revision history for revision ${input.revisions[input.revisions.length - 1]?.id ?? "unknown"}`,
    );
  }

  return latestModelSettings;
};

const buildSectionProfilesFromRevisions = async (input: {
  revisions: (typeof modelRevisions.$inferSelect)[];
}): Promise<Map<string, SectionProfileSnapshot>> => {
  if (input.revisions.length === 0) {
    return new Map();
  }

  const rows = await loadOrderedRevisionChanges(input.revisions);
  const sectionProfilesById = new Map<string, SectionProfileSnapshot>();
  for (const row of rows) {
    if (
      row.entityType !== "sectionprofile" &&
      row.entityType !== "section_profile"
    ) {
      continue;
    }
    if (row.op === "delete") {
      sectionProfilesById.delete(row.entityId);
      continue;
    }

    const payload = toObject(row.payload);
    const discriminator =
      payload.discriminator === "WITH_SHEAR_AREAS"
        ? "WITH_SHEAR_AREAS"
        : payload.discriminator === "STANDARD"
          ? "STANDARD"
          : undefined;
    if (!discriminator || typeof payload.name !== "string") {
      continue;
    }

    const area = toObject(payload.area);
    const strongAxisMomentOfInertia = toObject(
      payload.strongAxisMomentOfInertia,
    );
    const weakAxisMomentOfInertia = toObject(payload.weakAxisMomentOfInertia);
    const torsionalConstant = toObject(payload.torsionalConstant);
    const warpingConstant = toObject(payload.warpingConstant);
    const strongAxisPlasticSectionModulus = toObject(
      payload.strongAxisPlasticSectionModulus,
    );
    const weakAxisPlasticSectionModulus = toObject(
      payload.weakAxisPlasticSectionModulus,
    );
    const strongAxisElasticSectionModulus = toObject(
      payload.strongAxisElasticSectionModulus,
    );
    const weakAxisElasticSectionModulus = toObject(
      payload.weakAxisElasticSectionModulus,
    );

    const areaValue = toFiniteNumber(area.value);
    const strongIValue = toFiniteNumber(strongAxisMomentOfInertia.value);
    const weakIValue = toFiniteNumber(weakAxisMomentOfInertia.value);
    const torsionalValue = toFiniteNumber(torsionalConstant.value);
    const warpingValue = toFiniteNumber(warpingConstant.value);
    const strongPlasticValue = toFiniteNumber(
      strongAxisPlasticSectionModulus.value,
    );
    const weakPlasticValue = toFiniteNumber(
      weakAxisPlasticSectionModulus.value,
    );
    const strongElasticValue = toFiniteNumber(
      strongAxisElasticSectionModulus.value,
    );
    const weakElasticValue = toFiniteNumber(
      weakAxisElasticSectionModulus.value,
    );

    if (
      areaValue === undefined ||
      strongIValue === undefined ||
      weakIValue === undefined ||
      torsionalValue === undefined ||
      warpingValue === undefined ||
      strongPlasticValue === undefined ||
      weakPlasticValue === undefined ||
      strongElasticValue === undefined ||
      weakElasticValue === undefined
    ) {
      continue;
    }

    const strongAxisShearArea = toFiniteNumber(
      toObject(payload.strongAxisShearArea).value,
    );
    const weakAxisShearArea = toFiniteNumber(
      toObject(payload.weakAxisShearArea).value,
    );

    sectionProfilesById.set(row.entityId, {
      id: row.entityId,
      revisionId:
        typeof payload.revisionId === "string"
          ? payload.revisionId
          : (row.revisionId ?? ""),
      name: payload.name,
      discriminator,
      area: Area.FromSquareMeters(areaValue),
      strongAxisMomentOfInertia:
        AreaMomentOfInertia.FromMetersToTheFourth(strongIValue),
      weakAxisMomentOfInertia:
        AreaMomentOfInertia.FromMetersToTheFourth(weakIValue),
      torsionalConstant:
        AreaMomentOfInertia.FromMetersToTheFourth(torsionalValue),
      warpingConstant:
        WarpingMomentOfInertia.FromMetersToTheSixth(warpingValue),
      strongAxisPlasticSectionModulus:
        Volume.FromCubicMeters(strongPlasticValue),
      weakAxisPlasticSectionModulus: Volume.FromCubicMeters(weakPlasticValue),
      strongAxisElasticSectionModulus:
        Volume.FromCubicMeters(strongElasticValue),
      weakAxisElasticSectionModulus: Volume.FromCubicMeters(weakElasticValue),
      ...(strongAxisShearArea !== undefined
        ? { strongAxisShearArea: Area.FromSquareMeters(strongAxisShearArea) }
        : {}),
      ...(weakAxisShearArea !== undefined
        ? { weakAxisShearArea: Area.FromSquareMeters(weakAxisShearArea) }
        : {}),
    });
  }

  return sectionProfilesById;
};

const buildElement1dsFromRevisions = async (input: {
  revisions: (typeof modelRevisions.$inferSelect)[];
}): Promise<Map<string, Element1dSnapshot>> => {
  if (input.revisions.length === 0) {
    return new Map();
  }

  const rows = await loadOrderedRevisionChanges(input.revisions);
  const element1dsById = new Map<string, Element1dSnapshot>();
  for (const row of rows) {
    if (row.entityType !== "element1d") {
      continue;
    }
    if (row.op === "delete") {
      element1dsById.delete(row.entityId);
      continue;
    }

    const payload = toObject(row.payload);
    if (
      typeof payload.startNodeId !== "string" ||
      typeof payload.endNodeId !== "string" ||
      typeof payload.materialId !== "string" ||
      typeof payload.sectionProfileId !== "string"
    ) {
      continue;
    }

    element1dsById.set(row.entityId, {
      id: row.entityId,
      revisionId:
        typeof payload.revisionId === "string"
          ? payload.revisionId
          : (row.revisionId ?? ""),
      startNodeId: payload.startNodeId,
      endNodeId: payload.endNodeId,
      materialId: payload.materialId,
      sectionProfileId: payload.sectionProfileId,
    });
  }

  return element1dsById;
};

const buildLoadCasesFromRevisions = async (input: {
  revisions: (typeof modelRevisions.$inferSelect)[];
}): Promise<Map<string, LoadCaseSnapshot>> => {
  if (input.revisions.length === 0) {
    return new Map();
  }

  const rows = await loadOrderedRevisionChanges(input.revisions);
  const loadCasesById = new Map<string, LoadCaseSnapshot>();
  for (const row of rows) {
    if (row.entityType !== "loadcase" && row.entityType !== "load_case") {
      continue;
    }
    if (row.op === "delete") {
      loadCasesById.delete(row.entityId);
      continue;
    }

    const payload = toObject(row.payload);
    if (typeof payload.name !== "string" || payload.name.trim().length === 0) {
      continue;
    }

    loadCasesById.set(row.entityId, {
      id: row.entityId,
      revisionId:
        typeof payload.revisionId === "string"
          ? payload.revisionId
          : (row.revisionId ?? ""),
      name: payload.name,
    });
  }

  return loadCasesById;
};

const buildLoadCombinationsFromRevisions = async (input: {
  revisions: (typeof modelRevisions.$inferSelect)[];
}): Promise<Map<string, LoadCombinationSnapshot>> => {
  if (input.revisions.length === 0) {
    return new Map();
  }

  const rows = await loadOrderedRevisionChanges(input.revisions);
  const loadCombinationsById = new Map<string, LoadCombinationSnapshot>();
  for (const row of rows) {
    if (
      row.entityType !== "loadcombination" &&
      row.entityType !== "load_combination"
    ) {
      continue;
    }
    if (row.op === "delete") {
      loadCombinationsById.delete(row.entityId);
      continue;
    }

    const payload = toObject(row.payload);
    const loadCaseFactorsPayload = toObject(payload.loadCaseFactors);
    const loadCaseFactors: Record<string, number> = {};
    for (const [loadCaseId, factor] of Object.entries(loadCaseFactorsPayload)) {
      if (typeof factor === "number" && Number.isFinite(factor)) {
        loadCaseFactors[loadCaseId] = factor;
      }
    }

    loadCombinationsById.set(row.entityId, {
      id: row.entityId,
      revisionId:
        typeof payload.revisionId === "string"
          ? payload.revisionId
          : (row.revisionId ?? ""),
      loadCaseFactors,
    });
  }

  return loadCombinationsById;
};

const buildPointLoadsFromRevisions = async (input: {
  revisions: (typeof modelRevisions.$inferSelect)[];
}): Promise<Map<string, PointLoadSnapshot>> => {
  if (input.revisions.length === 0) {
    return new Map();
  }

  const rows = await loadOrderedRevisionChanges(input.revisions);
  const pointLoadsById = new Map<string, PointLoadSnapshot>();
  for (const row of rows) {
    if (row.entityType !== "pointload" && row.entityType !== "point_load") {
      continue;
    }
    if (row.op === "delete") {
      pointLoadsById.delete(row.entityId);
      continue;
    }

    const payload = toObject(row.payload);
    const forcePayload = toObject(payload.force);
    const directionPayload = toObject(payload.direction);
    const forceAlongX = toFiniteNumber(toObject(forcePayload.forceAlongX).value);
    const forceAlongY = toFiniteNumber(toObject(forcePayload.forceAlongY).value);
    const forceAlongZ = toFiniteNumber(toObject(forcePayload.forceAlongZ).value);
    const momentAboutX = toFiniteNumber(toObject(forcePayload.momentAboutX).value);
    const momentAboutY = toFiniteNumber(toObject(forcePayload.momentAboutY).value);
    const momentAboutZ = toFiniteNumber(toObject(forcePayload.momentAboutZ).value);
    const directionX = toFiniteNumber(directionPayload.x);
    const directionY = toFiniteNumber(directionPayload.y);
    const directionZ = toFiniteNumber(directionPayload.z);

    if (
      typeof payload.nodeId !== "string" ||
      typeof payload.loadCaseId !== "string" ||
      forceAlongX === undefined ||
      forceAlongY === undefined ||
      forceAlongZ === undefined ||
      momentAboutX === undefined ||
      momentAboutY === undefined ||
      momentAboutZ === undefined ||
      directionX === undefined ||
      directionY === undefined ||
      directionZ === undefined
    ) {
      continue;
    }

    pointLoadsById.set(row.entityId, {
      id: row.entityId,
      revisionId:
        typeof payload.revisionId === "string"
          ? payload.revisionId
          : (row.revisionId ?? ""),
      nodeId: payload.nodeId,
      loadCaseId: payload.loadCaseId,
      force: {
        forceAlongX: Force.FromNewtons(forceAlongX),
        forceAlongY: Force.FromNewtons(forceAlongY),
        forceAlongZ: Force.FromNewtons(forceAlongZ),
        momentAboutX: Torque.FromNewtonMeters(momentAboutX),
        momentAboutY: Torque.FromNewtonMeters(momentAboutY),
        momentAboutZ: Torque.FromNewtonMeters(momentAboutZ),
      },
      direction: {
        x: directionX,
        y: directionY,
        z: directionZ,
      },
    });
  }

  return pointLoadsById;
};

const extractNodeName = (payload: unknown): string => {
  if (payload && typeof payload === "object") {
    const name = (payload as { name?: unknown }).name;
    if (typeof name === "string") {
      return name;
    }
  }
  return "";
};

const extractNodeTypeDescriminator = (
  payload: unknown,
): "external" | "internal" => {
  if (payload && typeof payload === "object") {
    const nodeTypeDescriminator = (
      payload as { nodeTypeDescriminator?: unknown }
    ).nodeTypeDescriminator;
    if (nodeTypeDescriminator === "external") {
      return "external";
    }
  }

  return "internal";
};
