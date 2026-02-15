import crypto from "crypto";
import { and, eq, inArray, sql } from "drizzle-orm";
import {
  Area,
  AreaMomentOfInertia,
  AreaMomentOfInertiaUnits,
  AreaUnits,
  Pressure,
  PressureUnits,
  Ratio,
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
  DEFAULT_MODEL_REVISION_BRANCH_NAME,
  ModelRevisionAggregate,
} from "./model-revision-aggregate";
import { modelRevisionMapper } from "./model-revision-mapper";
import { RevisionChangeEntity } from "../revision-changes/revision-change-entity";
import { revisionChangeMapper } from "../revision-changes/revision-change-mapper";
import type { DomainEvent, ModelRevisionRepository } from "../common/types";
import type { MaterialSnapshot } from "../materials/material-entity";
import type { SectionProfileSnapshot } from "../section-profiles/section-profile-entity";
import type { Element1dSnapshot } from "../element1ds/element1d-entity";
import type { NodeRestraint, NodeSnapshot } from "../nodes/node-entity";
import { NodeRestraints, parseRestraint } from "../nodes/node-entity";

export const drizzleModelVersionRepository: ModelRevisionRepository = {
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
    const [revisions, branchHeadRows] = await Promise.all([
      loadRevisionHistory({
        revisionId: revision.id,
      }),
      getDb()
        .select()
        .from(modelBranchHeads)
        .where(eq(modelBranchHeads.headRevisionId, revision.id))
        .limit(1),
    ]);
    const [nodesById, materialsById, sectionProfilesById, element1dsById] =
      await Promise.all([
        buildNodesFromRevisions({
          revisions,
        }),
        buildMaterialsFromRevisions({ revisions }),
        buildSectionProfilesFromRevisions({ revisions }),
        buildElement1dsFromRevisions({ revisions }),
      ]);

    return ModelRevisionAggregate.rehydrate({
      id: revision.id,
      modelId: revision.modelId,
      branchName:
        branchHeadRows[0]?.branchName ?? DEFAULT_MODEL_REVISION_BRANCH_NAME,
      name: revision.modelName,
      parentRevisionId: revision.parentRevisionId,
      secondParentRevisionId: revision.secondParentRevisionId,
      authorId: revision.authorId,
      message: revision.message,
      createdAt: revision.createdAt,
      nodes: Array.from(nodesById.values()),
      materials: Array.from(materialsById.values()),
      sectionProfiles: Array.from(sectionProfilesById.values()),
      element1ds: Array.from(element1dsById.values()),
    });
  },

  async save(input) {
    const { revision, newRevision = false, tx: existingTx } = input;
    const snapshot = revision.toSnapshot();
    const events = revision.pullDomainEvents();

    if (!newRevision) {
      throw new Error("Only new revisions can be saved");
    }

    const persist = async (tx: DbTransaction) => {
      const revisionRow = await tx
        .insert(modelRevisions)
        .values(modelRevisionMapper.toPersistence(revision))
        .returning();

      const changeRows = buildRevisionChangeRowsFromEvents({
        modelId: snapshot.modelId,
        revisionId: snapshot.id,
        events,
      });

      if (changeRows.length > 0) {
        await tx
          .insert(revisionChanges)
          .values(changeRows);
      }

      if (snapshot.branchName) {
        const branchHead = modelBranchHeadMapper.fromInput({
          modelId: snapshot.modelId,
          branchName: snapshot.branchName,
          headRevisionId: snapshot.id,
        });
        const branchPersistence =
          modelBranchHeadMapper.toPersistence(branchHead);

        await tx
          .insert(modelBranchHeads)
          .values({
            modelId: branchPersistence.modelId,
            branchName: branchPersistence.branchName,
            headRevisionId: branchPersistence.headRevisionId,
          })
          .onConflictDoUpdate({
            target: [modelBranchHeads.modelId, modelBranchHeads.branchName],
            set: {
              headRevisionId: branchPersistence.headRevisionId,
              updatedAt: new Date(),
            },
          });
      }

      const persistedChangeRows = await tx
        .select()
        .from(revisionChanges)
        .where(eq(revisionChanges.revisionId, snapshot.id));

      return modelRevisionMapper.toDomain(
        revisionRow[0],
        persistedChangeRows,
      );
    };

    return existingTx ? persist(existingTx) : getDb().transaction(persist);
  },

  async getBranchHead(modelId, branchName) {
    const rows = await getDb()
      .select()
      .from(modelBranchHeads)
      .where(
        and(
          eq(modelBranchHeads.modelId, modelId),
          eq(modelBranchHeads.branchName, branchName),
        ),
      )
      .limit(1);

    if (!rows[0]) {
      return undefined;
    }

    return modelBranchHeadMapper.toDomain(rows[0]);
  },

  async listBranchHeads(modelId) {
    const rows = await getDb()
      .select()
      .from(modelBranchHeads)
      .where(eq(modelBranchHeads.modelId, modelId));

    return rows.map((row) => modelBranchHeadMapper.toDomain(row));
  },

  async createBranch(input) {
    const aggregate = modelBranchHeadMapper.fromInput(input);
    const persistence = modelBranchHeadMapper.toPersistence(aggregate);

    await getDb()
      .insert(modelBranchHeads)
      .values({
        modelId: persistence.modelId,
        branchName: persistence.branchName,
        headRevisionId: persistence.headRevisionId,
      })
      .onConflictDoUpdate({
        target: [modelBranchHeads.modelId, modelBranchHeads.branchName],
        set: {
          headRevisionId: persistence.headRevisionId,
          updatedAt: new Date(),
        },
      });
  },

  async commitRevision(input) {
    const aggregate = modelRevisionMapper.fromCommitInput({
      id: input.id,
      modelId: input.modelId,
      branchName: input.branchName,
      name: input.name,
      parentRevisionId: input.parentRevisionId ?? null,
      secondParentRevisionId: input.secondParentRevisionId ?? null,
      authorId: input.authorId,
      message: input.message,
      nodes: input.nodes,
    });

    const snapshot = aggregate.toSnapshot();

    return getDb().transaction(async (tx) => {
      const revisionRow = await tx
        .insert(modelRevisions)
        .values(modelRevisionMapper.toPersistence(aggregate))
        .returning();

      if (input.includeModelChange) {
        const modelChangeRow = buildModelRevisionChangeRow({
          modelId: snapshot.modelId,
          modelName: snapshot.name,
          revisionId: snapshot.id,
          op: snapshot.parentRevisionId ? "update" : "insert",
        });
        await tx
          .insert(revisionChanges)
          .values(modelChangeRow as any);
      }

      if (snapshot.nodes.length > 0) {
        const changeRows = buildRevisionChangeRowsFromNodes({
          modelId: snapshot.modelId,
          revisionId: snapshot.id,
          nodes: snapshot.nodes,
        });

        if (changeRows.length > 0) {
          await tx
            .insert(revisionChanges)
            .values(changeRows as any);
        }
      }

      if (input.branchName) {
        const branchHead = modelBranchHeadMapper.fromInput({
          modelId: snapshot.modelId,
          branchName: input.branchName,
          headRevisionId: snapshot.id,
        });
        const branchPersistence =
          modelBranchHeadMapper.toPersistence(branchHead);

        await tx
          .insert(modelBranchHeads)
          .values({
            modelId: branchPersistence.modelId,
            branchName: branchPersistence.branchName,
            headRevisionId: branchPersistence.headRevisionId,
          })
          .onConflictDoUpdate({
            target: [modelBranchHeads.modelId, modelBranchHeads.branchName],
            set: {
              headRevisionId: branchPersistence.headRevisionId,
              updatedAt: new Date(),
            },
          });
      }

      const persistedChangeRows = await tx
        .select()
        .from(revisionChanges)
        .where(eq(revisionChanges.revisionId, snapshot.id));

      return modelRevisionMapper.toDomain(revisionRow[0], persistedChangeRows);
    });
  },
};

const buildRevisionChangeRowsFromNodeOps = (input: {
  modelId: string;
  revisionId: string | null;
  nodes: {
    nodeId: string;
    name?: string;
    nodeTypeDescriminator?: "external" | "internal";
    op: "insert" | "update" | "delete";
  }[];
}): (typeof revisionChanges.$inferInsert)[] => {
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
        modelId: input.modelId,
        name: node.name ?? "",
        nodeTypeDescriminator: node.nodeTypeDescriminator ?? "internal",
      },
      createdAt: now,
    });

    return revisionChangeMapper.toPersistence(entity);
  });
};

const buildRevisionChangeRowsFromNodes = (input: {
  modelId: string;
  revisionId: string | null;
  nodes: NodeSnapshot[];
}): (typeof revisionChanges.$inferInsert)[] =>
  buildRevisionChangeRowsFromNodeOps({
    modelId: input.modelId,
    revisionId: input.revisionId,
    nodes: input.nodes.map((node) => ({
      nodeId: node.id,
      nodeTypeDescriminator: node.nodeTypeDescriminator,
      op: "update",
    })),
  });

const buildRevisionChangeRowsFromEvents = (input: {
  modelId: string;
  revisionId: string | null;
  events: DomainEvent[];
}): (typeof revisionChanges.$inferInsert)[] => {
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
    modelId: input.modelId,
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
      (event): event is Extract<DomainEvent, { type: "section_profile_created" }> =>
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

  return [...nodeChanges, ...nodeDeleteChanges, ...materialChanges, ...sectionProfileChanges, ...element1dChanges];
};

const buildModelRevisionChangeRow = (input: {
  modelId: string;
  modelName: string;
  revisionId: string | null;
  op: "insert" | "update";
}): typeof revisionChanges.$inferInsert => {
  const entity = RevisionChangeEntity.create({
    id: crypto.randomUUID(),
    revisionId: input.revisionId,
    entityType: "model",
    entityId: input.modelId,
    schemaVersion: 1,
    op: input.op,
    payload: {
      id: input.modelId,
      name: input.modelName,
    },
    createdAt: new Date(),
  });

  return revisionChangeMapper.toPersistence(entity);
};

const applyNodeChanges = (input: {
  current: Map<string, NodeSnapshot>;
  changes: {
    nodeId: string;
    nodeTypeDescriminator?: "external" | "internal";
    op: string;
  }[];
}) => {
  for (const change of input.changes) {
    if (change.op === "delete") {
      input.current.delete(change.nodeId);
      continue;
    }

    input.current.set(change.nodeId, {
      id: change.nodeId,
      modelRevisionId: "",
      nodeType:
        change.nodeTypeDescriminator === "external"
          ? "spatialNode"
          : "internalNode",
      nodeTypeDescriminator: change.nodeTypeDescriminator ?? "internal",
      point:
        change.nodeTypeDescriminator === "external"
          ? { x: 0, y: 0, z: 0 }
          : undefined,
      element1dId:
        change.nodeTypeDescriminator === "external" ? undefined : change.nodeId,
      distanceAlongElement1d:
        change.nodeTypeDescriminator === "external"
          ? undefined
          : Ratio.FromDecimalFractions(0),
      restraint: { ...NodeRestraints.FREE },
    });
  }
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
    const distanceAlongElement1d = toFiniteNumber(payload.distanceAlongElement1d);
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
    if (pressureEValue === undefined || pressureGValue === undefined) {
      continue;
    }

    materialsById.set(row.entityId, {
      id: row.entityId,
      revisionId:
        typeof payload.revisionId === "string"
          ? payload.revisionId
          : (row.revisionId ?? ""),
      pressureE: Pressure.FromPascals(pressureEValue),
      pressureG: Pressure.FromPascals(pressureGValue),
    });
  }

  return materialsById;
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
    const strongAxisMomentOfInertia = toObject(payload.strongAxisMomentOfInertia);
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
    const weakPlasticValue = toFiniteNumber(weakAxisPlasticSectionModulus.value);
    const strongElasticValue = toFiniteNumber(
      strongAxisElasticSectionModulus.value,
    );
    const weakElasticValue = toFiniteNumber(weakAxisElasticSectionModulus.value);

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
      strongAxisPlasticSectionModulus: Volume.FromCubicMeters(strongPlasticValue),
      weakAxisPlasticSectionModulus: Volume.FromCubicMeters(weakPlasticValue),
      strongAxisElasticSectionModulus: Volume.FromCubicMeters(strongElasticValue),
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
