import { defineEndpoint } from "../contracts/endpoint";
import {
  createModelRevisionReqSchema,
  modelRevisionResSchema,
} from "./create-model-revision-request-schema";
import { AppContext } from "src/common/types";
import { httpError } from "src/common/http-utils";
import { DbTransaction, getDb } from "src/db/client";
import { modelBranchHeads, modelRevisions } from "src/db/schema";
import { RevisionChangeEntity } from "src/revision-changes/revision-change-entity";
import { ModelRevisionAggregate } from "./model-revision-aggregate";
import {
  AreaMomentOfInertiaUnits,
  AreaUnits,
  Force,
  ForceUnits,
  Pressure,
  PressureUnits,
  Torque,
  TorqueUnits,
  VolumeUnits,
  WarpingMomentOfInertiaUnits,
} from "unitsnet-js";
import type { Element1dSnapshot } from "src/element1ds/element1d-entity";
import type { LoadCaseSnapshot } from "src/load-cases/load-case-entity";
import type { LoadCombinationSnapshot } from "src/load-combinations/load-combination-entity";
import type { MaterialEntity } from "src/materials/material-entity";
import type { NodeSnapshot } from "src/nodes/node-entity";
import type { PointLoadSnapshot } from "src/point-loads/point-load-entity";
import type { SectionProfileSnapshot } from "src/section-profiles/section-profile-entity";
import { z } from "zod";

export const createModelRevision = defineEndpoint({
  method: "POST",
  path: "/api/projects/:projectId/branches/:branchName/revisions",
  req: createModelRevisionReqSchema,
  res: modelRevisionResSchema,
  async handler(req, ctx: AppContext) {
    const branch = await ctx.services.modelRevisionRepository.getBranchHead(
      req.params.projectId,
      req.params.branchName,
    );
    if (!branch) {
      throw httpError(
        `Could not find branch ${req.params.branchName} on model with ID ${req.params.projectId}`,
        404,
      );
    }

    const parentRevision =
      await ctx.services.modelRevisionRepository.getRevisionById(
        branch.headRevisionId,
      );
    if (!parentRevision) {
      throw httpError(
        `Could not find parent revision ${branch.headRevisionId} for branch ${req.params.branchName}`,
        404,
      );
    }

    const currentNodesById = new Map(
      parentRevision.nodes.map((node) => [node.id, node.toSnapshot()] as const),
    );
    const currentMaterialsById = parentRevision.materials;
    const currentSectionProfilesById = new Map(
      parentRevision.sectionProfiles.map(
        (sectionProfile) =>
          [sectionProfile.id, sectionProfile.toSnapshot()] as const,
      ),
    );
    const currentElement1dsById = new Map(
      parentRevision.element1ds.map(
        (element1d) => [element1d.id, element1d.toSnapshot()] as const,
      ),
    );
    const currentLoadCasesById = new Map(
      parentRevision.loadCases.map(
        (loadCase) => [loadCase.id, loadCase.toSnapshot()] as const,
      ),
    );
    const currentLoadCombinationsById = new Map(
      parentRevision.loadCombinations.map(
        (loadCombination) =>
          [loadCombination.id, loadCombination.toSnapshot()] as const,
      ),
    );
    const currentPointLoadsById = new Map(
      parentRevision.pointLoads.map(
        (pointLoad) => [pointLoad.id, pointLoad.toSnapshot()] as const,
      ),
    );

    const revisionId = await getDb().transaction(async (tx) => {
      const revisionId = await createNewRevisionHandler(req, ctx, tx);
      const createdAt = new Date();

      const changes = buildRevisionChanges({
        req,
        revisionId,
        createdAt,
        currentNodesById,
        currentMaterialsById,
        currentSectionProfilesById,
        currentElement1dsById,
        currentLoadCasesById,
        currentLoadCombinationsById,
        currentPointLoadsById,
      });

      await ctx.services.revisionChangeRepository.batchCreate(tx, changes);
      return revisionId;
    });

    const modelRevision =
      await ctx.services.modelRevisionRepository.getRevisionById(revisionId);
    if (!modelRevision) {
      throw httpError("Failed to load created model revision", 500);
    }

    return {
      id: modelRevision.id,
      projectId: modelRevision.projectId,
      parentRevisionId: modelRevision.parentRevisionId,
      secondParentRevisionId: modelRevision.secondParentRevisionId,
      authorId: modelRevision.authorId,
      message: modelRevision.message,
      createdAt: modelRevision.createdAt.toISOString(),
      nodes: modelRevision.nodes.map((node) => ({
        id: node.id,
        projectId: modelRevision.projectId,
        nodeTypeDescriminator:
          node.toSnapshot().nodeTypeDescriminator ??
          (node.nodeType === "internalNode" ? "internal" : "external"),
      })),
      materials: [...modelRevision.materials.values()].map((material) => ({
        id: material.id,
        revisionId: material.revisionId,
        name: material.name,
        modulusOfElasticity: material.pressureE.Pascals,
        modulusOfRigidity: material.pressureG.Pascals,
        units: {
          pressure: PressureUnits.Pascals as const,
        },
      })),
      modelSettings: modelRevision.modelSettings
        ? {
            id: modelRevision.modelSettings.id,
            revisionId: modelRevision.modelSettings.revisionId,
            units: modelRevision.modelSettings.units,
            yAxisUp: modelRevision.modelSettings.yAxisUp,
          }
        : null,
      sectionProfiles: modelRevision.sectionProfiles.map((sectionProfile) => ({
        id: sectionProfile.id,
        revisionId: sectionProfile.revisionId,
        name: sectionProfile.name,
        discriminator: sectionProfile.discriminator,
        area: {
          value: sectionProfile.area.SquareMeters,
          unit: AreaUnits.SquareMeters as const,
        },
        strongAxisMomentOfInertia: {
          value: sectionProfile.strongAxisMomentOfInertia.MetersToTheFourth,
          unit: AreaMomentOfInertiaUnits.MetersToTheFourth as const,
        },
        weakAxisMomentOfInertia: {
          value: sectionProfile.weakAxisMomentOfInertia.MetersToTheFourth,
          unit: AreaMomentOfInertiaUnits.MetersToTheFourth as const,
        },
        torsionalConstant: {
          value: sectionProfile.torsionalConstant.MetersToTheFourth,
          unit: AreaMomentOfInertiaUnits.MetersToTheFourth as const,
        },
        warpingConstant: {
          value: sectionProfile.warpingConstant.MetersToTheSixth,
          unit: WarpingMomentOfInertiaUnits.MetersToTheSixth as const,
        },
        strongAxisPlasticSectionModulus: {
          value: sectionProfile.strongAxisPlasticSectionModulus.CubicMeters,
          unit: VolumeUnits.CubicMeters as const,
        },
        weakAxisPlasticSectionModulus: {
          value: sectionProfile.weakAxisPlasticSectionModulus.CubicMeters,
          unit: VolumeUnits.CubicMeters as const,
        },
        strongAxisElasticSectionModulus: {
          value: sectionProfile.strongAxisElasticSectionModulus.CubicMeters,
          unit: VolumeUnits.CubicMeters as const,
        },
        weakAxisElasticSectionModulus: {
          value: sectionProfile.weakAxisElasticSectionModulus.CubicMeters,
          unit: VolumeUnits.CubicMeters as const,
        },
        ...(sectionProfile.strongAxisShearArea
          ? {
              strongAxisShearArea: {
                value: sectionProfile.strongAxisShearArea.SquareMeters,
                unit: AreaUnits.SquareMeters as const,
              },
            }
          : {}),
        ...(sectionProfile.weakAxisShearArea
          ? {
              weakAxisShearArea: {
                value: sectionProfile.weakAxisShearArea.SquareMeters,
                unit: AreaUnits.SquareMeters as const,
              },
            }
          : {}),
      })),
      element1ds: modelRevision.element1ds.map((element1d) => ({
        id: element1d.id,
        revisionId: element1d.revisionId,
        startNodeId: element1d.startNodeId,
        endNodeId: element1d.endNodeId,
        materialId: element1d.materialId,
        sectionProfileId: element1d.sectionProfileId,
      })),
      loadCases: modelRevision.loadCases.map((loadCase) => ({
        id: loadCase.id,
        revisionId: loadCase.revisionId,
        name: loadCase.name,
      })),
      loadCombinations: modelRevision.loadCombinations.map(
        (loadCombination) => ({
          id: loadCombination.id,
          revisionId: loadCombination.revisionId,
          loadCaseFactors: { ...loadCombination.loadCaseFactors },
        }),
      ),
      pointLoads: modelRevision.pointLoads.map((pointLoad) => ({
        id: pointLoad.id,
        revisionId: pointLoad.revisionId,
        nodeId: pointLoad.nodeId,
        loadCaseId: pointLoad.loadCaseId,
        force: {
          forceAlongX: pointLoad.force.forceAlongX.Newtons,
          forceAlongY: pointLoad.force.forceAlongY.Newtons,
          forceAlongZ: pointLoad.force.forceAlongZ.Newtons,
          momentAboutX: pointLoad.force.momentAboutX.NewtonMeters,
          momentAboutY: pointLoad.force.momentAboutY.NewtonMeters,
          momentAboutZ: pointLoad.force.momentAboutZ.NewtonMeters,
        },
        direction: pointLoad.direction,
        units: {
          force: ForceUnits.Newtons as const,
          torque: TorqueUnits.NewtonMeters as const,
        },
      })),
    };
  },
});

const buildRevisionChanges = (input: {
  req: z.infer<typeof createModelRevisionReqSchema>;
  revisionId: string;
  createdAt: Date;
  currentNodesById: Map<string, NodeSnapshot>;
  currentMaterialsById: ReadonlyMap<string, MaterialEntity>;
  currentSectionProfilesById: Map<string, SectionProfileSnapshot>;
  currentElement1dsById: Map<string, Element1dSnapshot>;
  currentLoadCasesById: Map<string, LoadCaseSnapshot>;
  currentLoadCombinationsById: Map<string, LoadCombinationSnapshot>;
  currentPointLoadsById: Map<string, PointLoadSnapshot>;
}): RevisionChangeEntity[] => {
  const changes: RevisionChangeEntity[] = [];
  const normalizeName = (name: string) => name.trim().toLowerCase();
  const materialIdByName = new Map<string, string>();
  const currentMaterialIdByNormalizedName = new Map<string, string>();
  for (const material of input.currentMaterialsById.values()) {
    if (materialIdByName.has(material.name)) {
      throw httpError(`Duplicate material name "${material.name}"`, 400);
    }
    materialIdByName.set(material.name, material.id);
    const normalizedName = normalizeName(material.name);
    if (currentMaterialIdByNormalizedName.has(normalizedName)) {
      throw httpError(`Duplicate material name "${material.name}"`, 400);
    }
    currentMaterialIdByNormalizedName.set(normalizedName, material.id);
  }
  const sectionProfileIdByName = new Map<string, string>();
  for (const sectionProfile of input.currentSectionProfilesById.values()) {
    if (sectionProfileIdByName.has(sectionProfile.name)) {
      throw httpError(
        `Duplicate section profile name "${sectionProfile.name}"`,
        400,
      );
    }
    sectionProfileIdByName.set(sectionProfile.name, sectionProfile.id);
  }
  const currentSectionProfileIdByName = new Map(sectionProfileIdByName);
  const toEntity = (change: {
    entityType:
      | "node"
      | "material"
      | "sectionprofile"
      | "element1d"
      | "loadcase"
      | "loadcombination"
      | "pointload";
    entityId: string;
    op: "insert" | "update" | "delete";
    payload: Record<string, unknown>;
  }) =>
    RevisionChangeEntity.create({
      id: Bun.randomUUIDv7(),
      revisionId: input.revisionId,
      entityType: change.entityType,
      entityId: change.entityId,
      schemaVersion: 1,
      op: change.op,
      payload: change.payload,
      createdAt: input.createdAt,
    });

  for (const createNode of input.req.body.nodes?.create ?? []) {
    const id = Bun.randomUUIDv7();
    const payload =
      createNode.location.type === "internal"
        ? {
            id,
            modelRevisionId: input.revisionId,
            nodeType: "internalNode",
            nodeTypeDescriminator: "internal" as const,
            element1dId: createNode.location.element1dId,
            distanceAlongElement1d: createNode.location.ratioAlongElement1d,
            restraint: createNode.restraint,
          }
        : {
            id,
            modelRevisionId: input.revisionId,
            nodeType: "spatialNode",
            nodeTypeDescriminator: "external" as const,
            point: createNode.location.point,
            restraint: createNode.restraint,
          };

    changes.push(
      toEntity({
        entityType: "node",
        entityId: id,
        op: "insert",
        payload,
      }),
    );
  }

  for (const putNode of input.req.body.nodes?.update ?? []) {
    const existing = input.currentNodesById.get(putNode.id);
    if (!existing) {
      throw httpError(`Node ${putNode.id} not found`, 400);
    }
    const payload =
      putNode.location.type === "internal"
        ? {
            id: putNode.id,
            modelRevisionId: input.revisionId,
            nodeType: "internalNode",
            nodeTypeDescriminator: "internal" as const,
            element1dId: putNode.location.element1dId,
            distanceAlongElement1d: putNode.location.ratioAlongElement1d,
            restraint: putNode.restraint,
          }
        : {
            id: putNode.id,
            modelRevisionId: input.revisionId,
            nodeType: "spatialNode",
            nodeTypeDescriminator: "external" as const,
            point: putNode.location.point,
            restraint: putNode.restraint,
          };

    changes.push(
      toEntity({
        entityType: "node",
        entityId: putNode.id,
        op: "update",
        payload,
      }),
    );
  }

  for (const deleteNodeId of input.req.body.nodes?.delete ?? []) {
    changes.push(
      toEntity({
        entityType: "node",
        entityId: deleteNodeId,
        op: "delete",
        payload: { id: deleteNodeId },
      }),
    );
  }

  const materialUpdateNames = new Set(
    (input.req.body.materials?.update ?? []).map((material) =>
      normalizeName(material.name),
    ),
  );
  for (const materialName of materialUpdateNames) {
    const materialId = currentMaterialIdByNormalizedName.get(materialName);
    if (!materialId) {
      throw httpError(`Material "${materialName}" not found`, 400);
    }
    const existing = input.currentMaterialsById.get(materialId);
    if (!existing) {
      throw httpError(`Material "${materialName}" not found`, 400);
    }
    materialIdByName.delete(existing.name);
  }

  for (const deleteMaterialId of input.req.body.materials?.delete ?? []) {
    const existing = input.currentMaterialsById.get(deleteMaterialId);
    if (existing) {
      materialIdByName.delete(existing.name);
    }
  }

  for (const createMaterial of input.req.body.materials?.create ?? []) {
    const id = Bun.randomUUIDv7();
    if (materialIdByName.has(createMaterial.name)) {
      throw httpError(`Duplicate material name "${createMaterial.name}"`, 400);
    }
    materialIdByName.set(createMaterial.name, id);
    changes.push(
      toEntity({
        entityType: "material",
        entityId: id,
        op: "insert",
        payload: {
          id,
          revisionId: input.revisionId,
          name: createMaterial.name,
          pressureE: {
            value: new Pressure(
              createMaterial.modulusOfElasticity,
              createMaterial.units.pressure,
            ).Pascals,
            unit: "Pascals",
          },
          pressureG: {
            value: new Pressure(
              createMaterial.modulusOfRigidity,
              createMaterial.units.pressure,
            ).Pascals,
            unit: "Pascals",
          },
        },
      }),
    );
  }

  for (const putMaterial of input.req.body.materials?.update ?? []) {
    const putMaterialId = currentMaterialIdByNormalizedName.get(
      normalizeName(putMaterial.name),
    );
    if (!putMaterialId) {
      throw httpError(`Material "${putMaterial.name}" not found`, 400);
    }
    const existing = input.currentMaterialsById.get(putMaterialId);
    if (!existing) {
      throw httpError(`Material "${putMaterial.name}" not found`, 400);
    }
    const targetMaterialName = putMaterial.newName ?? putMaterial.name;
    if (materialIdByName.has(targetMaterialName)) {
      throw httpError(`Duplicate material name "${targetMaterialName}"`, 400);
    }
    materialIdByName.set(targetMaterialName, putMaterialId);

    changes.push(
      toEntity({
        entityType: "material",
        entityId: putMaterialId,
        op: "update",
        payload: {
          id: putMaterialId,
          revisionId: input.revisionId,
          name: targetMaterialName,
          pressureE: {
            value: new Pressure(
              putMaterial.modulusOfElasticity,
              putMaterial.units.pressure,
            ).Pascals,
            unit: "Pascals",
          },
          pressureG: {
            value: new Pressure(
              putMaterial.modulusOfRigidity,
              putMaterial.units.pressure,
            ).Pascals,
            unit: "Pascals",
          },
        },
      }),
    );
  }

  for (const deleteMaterialId of input.req.body.materials?.delete ?? []) {
    changes.push(
      toEntity({
        entityType: "material",
        entityId: deleteMaterialId,
        op: "delete",
        payload: { id: deleteMaterialId },
      }),
    );
  }

  const sectionProfileUpdateNames = new Set(
    (input.req.body.sectionProfiles?.update ?? []).map(
      (sectionProfile) => sectionProfile.name,
    ),
  );
  for (const sectionProfileName of sectionProfileUpdateNames) {
    const sectionProfileId = currentSectionProfileIdByName.get(sectionProfileName);
    if (!sectionProfileId) {
      throw httpError(`Section profile "${sectionProfileName}" not found`, 400);
    }
    const existing = input.currentSectionProfilesById.get(sectionProfileId);
    if (!existing) {
      throw httpError(`Section profile "${sectionProfileName}" not found`, 400);
    }
    sectionProfileIdByName.delete(existing.name);
  }

  for (const deleteSectionProfileName of input.req.body.sectionProfiles?.delete ??
    []) {
    const deleteSectionProfileId = currentSectionProfileIdByName.get(
      deleteSectionProfileName,
    );
    if (!deleteSectionProfileId) {
      continue;
    }
    const existing = input.currentSectionProfilesById.get(deleteSectionProfileId);
    if (existing) {
      sectionProfileIdByName.delete(existing.name);
    }
  }

  for (const createSectionProfile of input.req.body.sectionProfiles?.create ??
    []) {
    const hasStrongAxisShearArea =
      createSectionProfile.strongAxisShearArea !== undefined;
    const hasWeakAxisShearArea =
      createSectionProfile.weakAxisShearArea !== undefined;
    if (hasStrongAxisShearArea !== hasWeakAxisShearArea) {
      throw httpError(
        "strongAxisShearArea and weakAxisShearArea must both be provided or both be omitted",
        400,
      );
    }
    const hasShearAreas = hasStrongAxisShearArea && hasWeakAxisShearArea;

    const id = Bun.randomUUIDv7();
    if (sectionProfileIdByName.has(createSectionProfile.name)) {
      throw httpError(
        `Duplicate section profile name "${createSectionProfile.name}"`,
        400,
      );
    }
    sectionProfileIdByName.set(createSectionProfile.name, id);
    changes.push(
      toEntity({
        entityType: "sectionprofile",
        entityId: id,
        op: "insert",
        payload: {
          id,
          revisionId: input.revisionId,
          name: createSectionProfile.name,
          discriminator: hasShearAreas ? "WITH_SHEAR_AREAS" : "STANDARD",
          area: {
            value: createSectionProfile.area,
            unit: "SquareMeters",
          },
          strongAxisMomentOfInertia: {
            value: createSectionProfile.strongAxisMomentOfInertia,
            unit: "MetersToTheFourth",
          },
          weakAxisMomentOfInertia: {
            value: createSectionProfile.weakAxisMomentOfInertia,
            unit: "MetersToTheFourth",
          },
          torsionalConstant: {
            value: createSectionProfile.torsionalConstant,
            unit: "MetersToTheFourth",
          },
          warpingConstant: {
            value: createSectionProfile.warpingConstant,
            unit: "MetersToTheSixth",
          },
          strongAxisPlasticSectionModulus: {
            value: createSectionProfile.strongAxisPlasticSectionModulus,
            unit: "CubicMeters",
          },
          weakAxisPlasticSectionModulus: {
            value: createSectionProfile.weakAxisPlasticSectionModulus,
            unit: "CubicMeters",
          },
          strongAxisElasticSectionModulus: {
            value: createSectionProfile.strongAxisElasticSectionModulus,
            unit: "CubicMeters",
          },
          weakAxisElasticSectionModulus: {
            value: createSectionProfile.weakAxisElasticSectionModulus,
            unit: "CubicMeters",
          },
          ...(hasShearAreas
            ? {
                strongAxisShearArea: {
                  value: createSectionProfile.strongAxisShearArea!,
                  unit: "SquareMeters",
                },
                weakAxisShearArea: {
                  value: createSectionProfile.weakAxisShearArea!,
                  unit: "SquareMeters",
                },
              }
            : {}),
        },
      }),
    );
  }

  for (const putSectionProfile of input.req.body.sectionProfiles?.update ??
    []) {
    const targetSectionProfileName =
      putSectionProfile.newName ?? putSectionProfile.name;
    const sectionProfileId = currentSectionProfileIdByName.get(
      putSectionProfile.name,
    );
    if (!sectionProfileId) {
      throw httpError(`Section profile "${putSectionProfile.name}" not found`, 400);
    }
    const existing = input.currentSectionProfilesById.get(sectionProfileId);
    if (!existing) {
      throw httpError(`Section profile "${putSectionProfile.name}" not found`, 400);
    }
    if (sectionProfileIdByName.has(targetSectionProfileName)) {
      throw httpError(
        `Duplicate section profile name "${targetSectionProfileName}"`,
        400,
      );
    }
    const hasStrongAxisShearArea =
      putSectionProfile.strongAxisShearArea !== undefined;
    const hasWeakAxisShearArea =
      putSectionProfile.weakAxisShearArea !== undefined;
    if (hasStrongAxisShearArea !== hasWeakAxisShearArea) {
      throw httpError(
        "strongAxisShearArea and weakAxisShearArea must both be provided or both be omitted",
        400,
      );
    }
    const hasShearAreas = hasStrongAxisShearArea && hasWeakAxisShearArea;
    sectionProfileIdByName.set(targetSectionProfileName, sectionProfileId);

    changes.push(
      toEntity({
        entityType: "sectionprofile",
        entityId: sectionProfileId,
        op: "update",
        payload: {
          id: sectionProfileId,
          revisionId: input.revisionId,
          name: targetSectionProfileName,
          discriminator: hasShearAreas ? "WITH_SHEAR_AREAS" : "STANDARD",
          area: {
            value: putSectionProfile.area,
            unit: "SquareMeters",
          },
          strongAxisMomentOfInertia: {
            value: putSectionProfile.strongAxisMomentOfInertia,
            unit: "MetersToTheFourth",
          },
          weakAxisMomentOfInertia: {
            value: putSectionProfile.weakAxisMomentOfInertia,
            unit: "MetersToTheFourth",
          },
          torsionalConstant: {
            value: putSectionProfile.torsionalConstant,
            unit: "MetersToTheFourth",
          },
          warpingConstant: {
            value: putSectionProfile.warpingConstant,
            unit: "MetersToTheSixth",
          },
          strongAxisPlasticSectionModulus: {
            value: putSectionProfile.strongAxisPlasticSectionModulus,
            unit: "CubicMeters",
          },
          weakAxisPlasticSectionModulus: {
            value: putSectionProfile.weakAxisPlasticSectionModulus,
            unit: "CubicMeters",
          },
          strongAxisElasticSectionModulus: {
            value: putSectionProfile.strongAxisElasticSectionModulus,
            unit: "CubicMeters",
          },
          weakAxisElasticSectionModulus: {
            value: putSectionProfile.weakAxisElasticSectionModulus,
            unit: "CubicMeters",
          },
          ...(hasShearAreas
            ? {
                strongAxisShearArea: {
                  value: putSectionProfile.strongAxisShearArea!,
                  unit: "SquareMeters",
                },
                weakAxisShearArea: {
                  value: putSectionProfile.weakAxisShearArea!,
                  unit: "SquareMeters",
                },
              }
            : {}),
        },
      }),
    );
  }

  for (const deleteSectionProfileName of input.req.body.sectionProfiles?.delete ??
    []) {
    const deleteSectionProfileId = currentSectionProfileIdByName.get(
      deleteSectionProfileName,
    );
    if (!deleteSectionProfileId) {
      continue;
    }
    changes.push(
      toEntity({
        entityType: "sectionprofile",
        entityId: deleteSectionProfileId,
        op: "delete",
        payload: { id: deleteSectionProfileId },
      }),
    );
  }

  for (const createElement1d of input.req.body.element1ds?.create ?? []) {
    const id = Bun.randomUUIDv7();
    const materialId = materialIdByName.get(createElement1d.materialName);
    if (!materialId) {
      throw httpError(`Material "${createElement1d.materialName}" not found`, 400);
    }
    const sectionProfileId = sectionProfileIdByName.get(
      createElement1d.sectionProfileName,
    );
    if (!sectionProfileId) {
      throw httpError(
        `Section profile "${createElement1d.sectionProfileName}" not found`,
        400,
      );
    }
    changes.push(
      toEntity({
        entityType: "element1d",
        entityId: id,
        op: "insert",
        payload: {
          id,
          revisionId: input.revisionId,
          startNodeId: createElement1d.startNodeId,
          endNodeId: createElement1d.endNodeId,
          materialId,
          sectionProfileId,
        },
      }),
    );
  }

  for (const putElement1d of input.req.body.element1ds?.update ?? []) {
    const existing = input.currentElement1dsById.get(putElement1d.id);
    if (!existing) {
      throw httpError(`Element1d ${putElement1d.id} not found`, 400);
    }

    changes.push(
      toEntity({
        entityType: "element1d",
        entityId: putElement1d.id,
        op: "update",
        payload: {
          id: putElement1d.id,
          revisionId: input.revisionId,
          startNodeId: putElement1d.startNodeId,
          endNodeId: putElement1d.endNodeId,
          materialId: putElement1d.materialId,
          sectionProfileId: putElement1d.sectionProfileId,
        },
      }),
    );
  }

  for (const deleteElement1dId of input.req.body.element1ds?.delete ?? []) {
    changes.push(
      toEntity({
        entityType: "element1d",
        entityId: deleteElement1dId,
        op: "delete",
        payload: { id: deleteElement1dId },
      }),
    );
  }

  for (const createLoadCase of input.req.body.loadCases?.create ?? []) {
    const id = Bun.randomUUIDv7();
    changes.push(
      toEntity({
        entityType: "loadcase",
        entityId: id,
        op: "insert",
        payload: {
          id,
          revisionId: input.revisionId,
          name: createLoadCase.name,
        },
      }),
    );
  }

  for (const putLoadCase of input.req.body.loadCases?.update ?? []) {
    const existing = input.currentLoadCasesById.get(putLoadCase.id);
    if (!existing) {
      throw httpError(`Load case ${putLoadCase.id} not found`, 400);
    }

    changes.push(
      toEntity({
        entityType: "loadcase",
        entityId: putLoadCase.id,
        op: "update",
        payload: {
          id: putLoadCase.id,
          revisionId: input.revisionId,
          name: putLoadCase.name,
        },
      }),
    );
  }

  for (const deleteLoadCaseId of input.req.body.loadCases?.delete ?? []) {
    changes.push(
      toEntity({
        entityType: "loadcase",
        entityId: deleteLoadCaseId,
        op: "delete",
        payload: { id: deleteLoadCaseId },
      }),
    );
  }

  for (const createLoadCombination of input.req.body.loadCombinations?.create ??
    []) {
    const id = Bun.randomUUIDv7();
    changes.push(
      toEntity({
        entityType: "loadcombination",
        entityId: id,
        op: "insert",
        payload: {
          id,
          revisionId: input.revisionId,
          loadCaseFactors: { ...createLoadCombination.loadCaseFactors },
        },
      }),
    );
  }

  for (const putLoadCombination of input.req.body.loadCombinations?.update ??
    []) {
    const existing = input.currentLoadCombinationsById.get(
      putLoadCombination.id,
    );
    if (!existing) {
      throw httpError(
        `Load combination ${putLoadCombination.id} not found`,
        400,
      );
    }

    changes.push(
      toEntity({
        entityType: "loadcombination",
        entityId: putLoadCombination.id,
        op: "update",
        payload: {
          id: putLoadCombination.id,
          revisionId: input.revisionId,
          loadCaseFactors: { ...putLoadCombination.loadCaseFactors },
        },
      }),
    );
  }

  for (const deleteLoadCombinationId of input.req.body.loadCombinations
    ?.delete ?? []) {
    changes.push(
      toEntity({
        entityType: "loadcombination",
        entityId: deleteLoadCombinationId,
        op: "delete",
        payload: { id: deleteLoadCombinationId },
      }),
    );
  }

  for (const createPointLoad of input.req.body.pointLoads?.create ?? []) {
    const id = Bun.randomUUIDv7();
    changes.push(
      toEntity({
        entityType: "pointload",
        entityId: id,
        op: "insert",
        payload: {
          id,
          revisionId: input.revisionId,
          nodeId: createPointLoad.nodeId,
          loadCaseId: createPointLoad.loadCaseId,
          force: {
            forceAlongX: {
              value: new Force(
                createPointLoad.force.forceAlongX,
                createPointLoad.units.force,
              ).Newtons,
              unit: ForceUnits.Newtons,
            },
            forceAlongY: {
              value: new Force(
                createPointLoad.force.forceAlongY,
                createPointLoad.units.force,
              ).Newtons,
              unit: ForceUnits.Newtons,
            },
            forceAlongZ: {
              value: new Force(
                createPointLoad.force.forceAlongZ,
                createPointLoad.units.force,
              ).Newtons,
              unit: ForceUnits.Newtons,
            },
            momentAboutX: {
              value: new Torque(
                createPointLoad.force.momentAboutX,
                createPointLoad.units.torque,
              ).NewtonMeters,
              unit: TorqueUnits.NewtonMeters,
            },
            momentAboutY: {
              value: new Torque(
                createPointLoad.force.momentAboutY,
                createPointLoad.units.torque,
              ).NewtonMeters,
              unit: TorqueUnits.NewtonMeters,
            },
            momentAboutZ: {
              value: new Torque(
                createPointLoad.force.momentAboutZ,
                createPointLoad.units.torque,
              ).NewtonMeters,
              unit: TorqueUnits.NewtonMeters,
            },
          },
          direction: createPointLoad.direction,
        },
      }),
    );
  }

  for (const putPointLoad of input.req.body.pointLoads?.update ?? []) {
    const existing = input.currentPointLoadsById.get(putPointLoad.id);
    if (!existing) {
      throw httpError(`Point load ${putPointLoad.id} not found`, 400);
    }

    changes.push(
      toEntity({
        entityType: "pointload",
        entityId: putPointLoad.id,
        op: "update",
        payload: {
          id: putPointLoad.id,
          revisionId: input.revisionId,
          nodeId: putPointLoad.nodeId,
          loadCaseId: putPointLoad.loadCaseId,
          force: {
            forceAlongX: {
              value: new Force(
                putPointLoad.force.forceAlongX,
                putPointLoad.units.force,
              ).Newtons,
              unit: ForceUnits.Newtons,
            },
            forceAlongY: {
              value: new Force(
                putPointLoad.force.forceAlongY,
                putPointLoad.units.force,
              ).Newtons,
              unit: ForceUnits.Newtons,
            },
            forceAlongZ: {
              value: new Force(
                putPointLoad.force.forceAlongZ,
                putPointLoad.units.force,
              ).Newtons,
              unit: ForceUnits.Newtons,
            },
            momentAboutX: {
              value: new Torque(
                putPointLoad.force.momentAboutX,
                putPointLoad.units.torque,
              ).NewtonMeters,
              unit: TorqueUnits.NewtonMeters,
            },
            momentAboutY: {
              value: new Torque(
                putPointLoad.force.momentAboutY,
                putPointLoad.units.torque,
              ).NewtonMeters,
              unit: TorqueUnits.NewtonMeters,
            },
            momentAboutZ: {
              value: new Torque(
                putPointLoad.force.momentAboutZ,
                putPointLoad.units.torque,
              ).NewtonMeters,
              unit: TorqueUnits.NewtonMeters,
            },
          },
          direction: putPointLoad.direction,
        },
      }),
    );
  }

  for (const deletePointLoadId of input.req.body.pointLoads?.delete ?? []) {
    changes.push(
      toEntity({
        entityType: "pointload",
        entityId: deletePointLoadId,
        op: "delete",
        payload: { id: deletePointLoadId },
      }),
    );
  }

  return changes;
};

export async function createNewRevisionHandler(
  req: {
    params: { projectId: string; branchName: string };
  },
  ctx: AppContext,
  tx: DbTransaction,
) {
  const { projectId, branchName } = req.params;
  const branch = await ctx.services.modelRevisionRepository.getBranchHead(
    projectId,
    branchName,
  );
  if (!branch) {
    throw httpError(
      `Could not find branch ${branchName} on model with ID ${projectId}`,
      404,
    );
  }

  const parentRevision =
    await ctx.services.modelRevisionRepository.getRevisionById(
      branch.headRevisionId,
    );
  if (!parentRevision) {
    throw httpError(
      `Could not find parent revision ${branch.headRevisionId} for branch ${branchName}`,
      404,
    );
  }

  const revisionId = Bun.randomUUIDv7();
  const now = new Date();
  await tx.insert(modelRevisions).values({
    id: revisionId,
    projectId,
    parentRevisionId: parentRevision.id,
    secondParentRevisionId: null,
    authorId: parentRevision.authorId,
    message: "Batch create materials",
    createdAt: now,
  });

  await tx
    .insert(modelBranchHeads)
    .values({
      projectId,
      branchName,
      headRevisionId: revisionId,
      updatedAt: now,
    })
    .onConflictDoUpdate({
      target: [modelBranchHeads.projectId, modelBranchHeads.branchName],
      set: {
        headRevisionId: revisionId,
        updatedAt: now,
      },
    });
  return revisionId;
}

export async function createNewRevisionAggregateHandler(
  req: {
    params: { projectId: string; branchName: string };
  },
  ctx: AppContext,
  message = "Batch create materials",
) {
  const { projectId, branchName } = req.params;
  const branch = await ctx.services.modelRevisionRepository.getBranchHead(
    projectId,
    branchName,
  );
  if (!branch) {
    throw httpError(
      `Could not find branch ${branchName} on model with ID ${projectId}`,
      404,
    );
  }

  const parentRevision =
    await ctx.services.modelRevisionRepository.getRevisionById(
      branch.headRevisionId,
    );
  if (!parentRevision) {
    throw httpError(
      `Could not find parent revision ${branch.headRevisionId} for branch ${branchName}`,
      404,
    );
  }

  return ModelRevisionAggregate.create({
    projectId,
    parentRevisionId: parentRevision.id,
    secondParentRevisionId: null,
    authorId: parentRevision.authorId,
    message,
    createdAt: new Date(),
    nodes: parentRevision.nodes.map((node) => node.toSnapshot()),
    materials: Array.from(parentRevision.materials.values()).map((material) =>
      material.toRevisionV1(),
    ),
    modelSettings: parentRevision.modelSettings?.toSnapshot() ?? null,
    sectionProfiles: parentRevision.sectionProfiles.map((sectionProfile) =>
      sectionProfile.toSnapshot(),
    ),
    element1ds: parentRevision.element1ds.map((element1d) =>
      element1d.toSnapshot(),
    ),
    loadCases: parentRevision.loadCases.map((loadCase) =>
      loadCase.toSnapshot(),
    ),
    loadCombinations: parentRevision.loadCombinations.map((loadCombination) =>
      loadCombination.toSnapshot(),
    ),
    pointLoads: parentRevision.pointLoads.map((pointLoad) =>
      pointLoad.toSnapshot(),
    ),
  });
}
