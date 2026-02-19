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
import {
  ModelRevisionAggregate,
  type ModelRevisionEntityChange,
} from "./model-revision-aggregate";
import { modelRevisionMapper } from "./model-revision-mapper";
import type { RevisionChangeInsertRow } from "../revision-changes/revision-change-mapper";
import type { ModelRevisionRepository } from "../common/types";
import {
  MaterialEntity,
  type MaterialSnapshot,
} from "../materials/material-entity";
import {
  ModelSettingsEntity,
  type ModelSettingsSnapshot,
} from "../model-settings/model-settings-entity";
import {
  SectionProfileEntity,
  type SectionProfileSnapshot,
} from "../section-profiles/section-profile-entity";
import {
  Element1dEntity,
  type Element1dSnapshot,
} from "../element1ds/element1d-entity";
import { NodeEntity, type NodeSnapshot } from "../nodes/node-entity";
import { parseRestraint } from "../nodes/node-entity";
import { LoadCaseEntity, type LoadCaseSnapshot } from "../load-cases/load-case-entity";
import {
  LoadCombinationEntity,
  type LoadCombinationSnapshot,
} from "../load-combinations/load-combination-entity";
import { PointLoadEntity, type PointLoadSnapshot } from "../point-loads/point-load-entity";

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
    const changes = revision.pullRevisionChanges();
    revision.pullDomainEvents();

    const persist = async (tx: DbTransaction) => {
      await tx
        .insert(modelRevisions)
        .values(modelRevisionMapper.toPersistence(revision));

      const changeRows = buildRevisionChangeRowsFromRevisionChanges({
        revisionId: snapshot.id,
        changes,
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

const toRevisionChangeOp = (
  op: ModelRevisionEntityChange["op"],
): "created" | "updated" | "deleted" => {
  return op;
};

const toRevisionChangePayload = (
  change: ModelRevisionEntityChange,
): Record<string, unknown> => {
  switch (change.entityType) {
    case "node":
      return toNodeRevisionChangePayload(change.snapshot as NodeSnapshot);
    case "material":
      return toMaterialRevisionChangePayload(change.snapshot as MaterialSnapshot);
    case "model_settings":
      return toModelSettingsRevisionChangePayload(
        change.snapshot as ModelSettingsSnapshot,
      );
    case "section_profile":
      return toSectionProfileRevisionChangePayload(
        change.snapshot as SectionProfileSnapshot,
      );
    case "element1d":
      return toElement1dRevisionChangePayload(
        change.snapshot as Element1dSnapshot,
      );
    case "loadcase":
      return toLoadCaseRevisionChangePayload(change.snapshot as LoadCaseSnapshot);
    case "loadcombination":
      return toLoadCombinationRevisionChangePayload(
        change.snapshot as LoadCombinationSnapshot,
      );
    case "pointload":
      return toPointLoadRevisionChangePayload(change.snapshot as PointLoadSnapshot);
  }
};

const buildRevisionChangeRowsFromRevisionChanges = (input: {
  revisionId: string | null;
  changes: ModelRevisionEntityChange[];
}): RevisionChangeInsertRow[] => {
  const now = new Date();

  return input.changes.map((change) => ({
    id: crypto.randomUUID(),
    revisionId: input.revisionId,
    entityType: change.entityType,
    entityId: change.entityId,
    schemaVersion: 1,
    op: toRevisionChangeOp(change.op),
    payload: toRevisionChangePayload(change),
    createdAt: now,
  }));
};

const toNodeRevisionChangePayload = (
  snapshot: NodeSnapshot,
): Record<string, unknown> => ({
  id: snapshot.id,
  modelRevisionId: snapshot.modelRevisionId ?? null,
  nodeType: snapshot.nodeType ?? null,
  nodeTypeDescriminator: snapshot.nodeTypeDescriminator ?? null,
  point: snapshot.point ?? null,
  element1dId: snapshot.element1dId ?? null,
  distanceAlongElement1d:
    snapshot.distanceAlongElement1d instanceof Ratio
      ? snapshot.distanceAlongElement1d.DecimalFractions
      : null,
  restraint: snapshot.restraint ?? null,
});

const toMaterialRevisionChangePayload = (
  snapshot: MaterialSnapshot,
): Record<string, unknown> => ({
  id: snapshot.id,
  revisionId: snapshot.revisionId,
  name: snapshot.name,
  pressureE: {
    value: snapshot.pressureE.Pascals,
    unit: PressureUnits.Pascals,
  },
  pressureG: {
    value: snapshot.pressureG.Pascals,
    unit: PressureUnits.Pascals,
  },
});

const toModelSettingsRevisionChangePayload = (
  snapshot: ModelSettingsSnapshot,
): Record<string, unknown> => ({
  id: snapshot.id,
  revisionId: snapshot.revisionId,
  units: snapshot.units,
  yAxisUp: snapshot.yAxisUp,
});

const toSectionProfileRevisionChangePayload = (
  snapshot: SectionProfileSnapshot,
): Record<string, unknown> => ({
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
});

const toElement1dRevisionChangePayload = (
  snapshot: Element1dSnapshot,
): Record<string, unknown> => ({
  id: snapshot.id,
  revisionId: snapshot.revisionId,
  startNodeId: snapshot.startNodeId,
  endNodeId: snapshot.endNodeId,
  materialId: snapshot.materialId,
  sectionProfileId: snapshot.sectionProfileId,
});

const toLoadCaseRevisionChangePayload = (
  snapshot: LoadCaseSnapshot,
): Record<string, unknown> => ({
  id: snapshot.id,
  revisionId: snapshot.revisionId,
  name: snapshot.name,
});

const toLoadCombinationRevisionChangePayload = (
  snapshot: LoadCombinationSnapshot,
): Record<string, unknown> => ({
  id: snapshot.id,
  revisionId: snapshot.revisionId,
  loadCaseFactors: { ...snapshot.loadCaseFactors },
});

const toPointLoadRevisionChangePayload = (
  snapshot: PointLoadSnapshot,
): Record<string, unknown> => ({
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
});

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
}): Promise<Map<string, NodeEntity>> => {
  if (input.revisions.length === 0) {
    return new Map();
  }

  const rows = await loadOrderedRevisionChanges(input.revisions);

  const nodesById = new Map<string, NodeEntity>();
  for (const row of rows) {
    if (row.entityType !== "node") {
      continue;
    }
    if (isDeleteOperation(row.op)) {
      nodesById.delete(row.entityId);
      continue;
    }

    nodesById.set(
      row.entityId,
      NodeEntity.rehydrate(
        toNodeSnapshotFromRevisionChange({
          row,
        }),
      ),
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

const isDeleteOperation = (op: string): boolean => {
  return op === "delete" || op === "deleted";
};

const buildMaterialsFromRevisions = async (input: {
  revisions: (typeof modelRevisions.$inferSelect)[];
}): Promise<Map<string, MaterialEntity>> => {
  if (input.revisions.length === 0) {
    return new Map();
  }

  const rows = await loadOrderedRevisionChanges(input.revisions);
  const materialsById = new Map<string, MaterialEntity>();
  for (const row of rows) {
    if (row.entityType !== "material") {
      continue;
    }
    if (isDeleteOperation(row.op)) {
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

    materialsById.set(
      row.entityId,
      MaterialEntity.rehydrate({
        id: row.entityId,
        revisionId:
          typeof payload.revisionId === "string"
            ? payload.revisionId
            : (row.revisionId ?? ""),
        name,
        pressureE: Pressure.FromPascals(pressureEValue),
        pressureG: Pressure.FromPascals(pressureGValue),
      }),
    );
  }

  return materialsById;
};

const buildModelSettingsFromRevisions = async (input: {
  revisions: (typeof modelRevisions.$inferSelect)[];
}): Promise<ModelSettingsEntity> => {
  if (input.revisions.length === 0) {
    throw new Error("Cannot build model settings without revision history");
  }

  const rows = await loadOrderedRevisionChanges(input.revisions);
  let latestModelSettings: ModelSettingsSnapshot | null = null;
  for (const row of rows) {
    if (row.entityType !== "model_settings") {
      continue;
    }
    if (isDeleteOperation(row.op)) {
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

  return ModelSettingsEntity.rehydrate(latestModelSettings);
};

const buildSectionProfilesFromRevisions = async (input: {
  revisions: (typeof modelRevisions.$inferSelect)[];
}): Promise<Map<string, SectionProfileEntity>> => {
  if (input.revisions.length === 0) {
    return new Map();
  }

  const rows = await loadOrderedRevisionChanges(input.revisions);
  const sectionProfilesById = new Map<string, SectionProfileEntity>();
  for (const row of rows) {
    if (
      row.entityType !== "sectionprofile" &&
      row.entityType !== "section_profile"
    ) {
      continue;
    }
    if (isDeleteOperation(row.op)) {
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

    sectionProfilesById.set(
      row.entityId,
      SectionProfileEntity.rehydrate({
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
      }),
    );
  }

  return sectionProfilesById;
};

const buildElement1dsFromRevisions = async (input: {
  revisions: (typeof modelRevisions.$inferSelect)[];
}): Promise<Map<string, Element1dEntity>> => {
  if (input.revisions.length === 0) {
    return new Map();
  }

  const rows = await loadOrderedRevisionChanges(input.revisions);
  const element1dsById = new Map<string, Element1dEntity>();
  for (const row of rows) {
    if (row.entityType !== "element1d") {
      continue;
    }
    if (isDeleteOperation(row.op)) {
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

    element1dsById.set(
      row.entityId,
      Element1dEntity.rehydrate({
        id: row.entityId,
        revisionId:
          typeof payload.revisionId === "string"
            ? payload.revisionId
            : (row.revisionId ?? ""),
        startNodeId: payload.startNodeId,
        endNodeId: payload.endNodeId,
        materialId: payload.materialId,
        sectionProfileId: payload.sectionProfileId,
      }),
    );
  }

  return element1dsById;
};

const buildLoadCasesFromRevisions = async (input: {
  revisions: (typeof modelRevisions.$inferSelect)[];
}): Promise<Map<string, LoadCaseEntity>> => {
  if (input.revisions.length === 0) {
    return new Map();
  }

  const rows = await loadOrderedRevisionChanges(input.revisions);
  const loadCasesById = new Map<string, LoadCaseEntity>();
  for (const row of rows) {
    if (row.entityType !== "loadcase" && row.entityType !== "load_case") {
      continue;
    }
    if (isDeleteOperation(row.op)) {
      loadCasesById.delete(row.entityId);
      continue;
    }

    const payload = toObject(row.payload);
    if (typeof payload.name !== "string" || payload.name.trim().length === 0) {
      continue;
    }

    loadCasesById.set(
      row.entityId,
      LoadCaseEntity.rehydrate({
        id: row.entityId,
        revisionId:
          typeof payload.revisionId === "string"
            ? payload.revisionId
            : (row.revisionId ?? ""),
        name: payload.name,
      }),
    );
  }

  return loadCasesById;
};

const buildLoadCombinationsFromRevisions = async (input: {
  revisions: (typeof modelRevisions.$inferSelect)[];
}): Promise<Map<string, LoadCombinationEntity>> => {
  if (input.revisions.length === 0) {
    return new Map();
  }

  const rows = await loadOrderedRevisionChanges(input.revisions);
  const loadCombinationsById = new Map<string, LoadCombinationEntity>();
  for (const row of rows) {
    if (
      row.entityType !== "loadcombination" &&
      row.entityType !== "load_combination"
    ) {
      continue;
    }
    if (isDeleteOperation(row.op)) {
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

    loadCombinationsById.set(
      row.entityId,
      LoadCombinationEntity.rehydrate({
        id: row.entityId,
        revisionId:
          typeof payload.revisionId === "string"
            ? payload.revisionId
            : (row.revisionId ?? ""),
        loadCaseFactors,
      }),
    );
  }

  return loadCombinationsById;
};

const buildPointLoadsFromRevisions = async (input: {
  revisions: (typeof modelRevisions.$inferSelect)[];
}): Promise<Map<string, PointLoadEntity>> => {
  if (input.revisions.length === 0) {
    return new Map();
  }

  const rows = await loadOrderedRevisionChanges(input.revisions);
  const pointLoadsById = new Map<string, PointLoadEntity>();
  for (const row of rows) {
    if (row.entityType !== "pointload" && row.entityType !== "point_load") {
      continue;
    }
    if (isDeleteOperation(row.op)) {
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

    pointLoadsById.set(
      row.entityId,
      PointLoadEntity.rehydrate({
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
      }),
    );
  }

  return pointLoadsById;
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
