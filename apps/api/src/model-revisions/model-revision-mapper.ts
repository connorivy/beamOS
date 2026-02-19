import { modelRevisions, revisionChanges } from "../db/schema";
import {
  ModelRevisionAggregate,
  type ModelRevisionCreateSnapshot,
} from "./model-revision-aggregate";
import type { NodeSnapshot } from "../nodes/node-entity";
import {
  AreaMomentOfInertiaUnits,
  AreaUnits,
  PressureUnits,
  VolumeUnits,
  WarpingMomentOfInertiaUnits,
} from "unitsnet-js";

export const modelRevisionMapper = {
  toDomain(
    row: typeof modelRevisions.$inferSelect,
    changeRows: (typeof revisionChanges.$inferSelect)[],
  ): ModelRevisionAggregate {
    return ModelRevisionAggregate.create({
      id: row.id,
      projectId: row.projectId,
      parentRevisionId: row.parentRevisionId,
      secondParentRevisionId: row.secondParentRevisionId,
      authorId: row.authorId,
      message: row.message,
      createdAt: row.createdAt,
      nodes: changeRows
        .filter((change) => change.entityType === "node")
        .filter((change) => change.op !== "delete" && change.op !== "deleted")
        .map((change) => ({
          id: change.entityId,
          modelRevisionId: row.id,
          nodeType: "spatialNode",
          nodeTypeDescriminator: extractNodeTypeDescriminator(change.payload),
        })),
      materials: [],
      modelSettings: createDefaultModelSettingsSnapshot({
        revisionId: row.id,
      }),
      sectionProfiles: [],
      element1ds: [],
      loadCases: [],
      loadCombinations: [],
      pointLoads: [],
    });
  },

  toPersistence(
    aggregate: ModelRevisionAggregate,
  ): typeof modelRevisions.$inferInsert {
    const snapshot = aggregate.toSnapshot();
    return {
      id: snapshot.id,
      projectId: snapshot.projectId,
      parentRevisionId: snapshot.parentRevisionId,
      secondParentRevisionId: snapshot.secondParentRevisionId,
      authorId: snapshot.authorId,
      message: snapshot.message,
      createdAt: snapshot.createdAt,
    };
  },

  fromCommitInput(input: {
    id: string;
    projectId: string;
    parentRevisionId: string | null;
    secondParentRevisionId: string | null;
    authorId: string;
    message: string;
    nodes: NodeSnapshot[];
  }): ModelRevisionAggregate {
    const snapshot: ModelRevisionCreateSnapshot = {
      id: input.id,
      projectId: input.projectId,
      parentRevisionId: input.parentRevisionId,
      secondParentRevisionId: input.secondParentRevisionId,
      authorId: input.authorId,
      message: input.message,
      createdAt: new Date(),
      nodes: input.nodes.map((node) => ({
        id: node.id,
        modelRevisionId: input.id,
        nodeType: node.nodeType ?? "spatialNode",
        nodeTypeDescriminator: node.nodeTypeDescriminator,
      })),
      materials: [],
      modelSettings: createDefaultModelSettingsSnapshot({
        revisionId: input.id,
      }),
      sectionProfiles: [],
      element1ds: [],
      loadCases: [],
      loadCombinations: [],
      pointLoads: [],
    };

    return ModelRevisionAggregate.create(snapshot);
  },
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
