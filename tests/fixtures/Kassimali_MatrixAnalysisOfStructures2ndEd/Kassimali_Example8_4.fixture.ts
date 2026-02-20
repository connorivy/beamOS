import type { CreateModelRevisionRequest } from "@beamos/openapi-client";

export const kassimaliExample8_4BaseRevision: CreateModelRevisionRequest = {
  modelSettings: {
    units: {
      pressure: "KilopoundForcePerSquareInch",
      area: "SquareInch",
      areaMomentOfInertia: "InchToTheFourth",
      warpingMomentOfInertia: "InchToTheSixth",
      volume: "CubicInch",
    },
    yAxisUp: true,
  },
  nodes: {
    create: [
      {
        tempId: "n1",
        location: { type: "spatial", point: { x: 0, y: 0, z: 0 } },
        restraint: {
          canTranslateAlongX: true,
          canTranslateAlongY: true,
          canTranslateAlongZ: true,
          canRotateAboutX: true,
          canRotateAboutY: true,
          canRotateAboutZ: true,
        },
      },
      {
        tempId: "n2",
        location: { type: "spatial", point: { x: -20, y: 0, z: 0 } },
        restraint: {
          canTranslateAlongX: false,
          canTranslateAlongY: false,
          canTranslateAlongZ: false,
          canRotateAboutX: false,
          canRotateAboutY: false,
          canRotateAboutZ: false,
        },
      },
      {
        tempId: "n3",
        location: { type: "spatial", point: { x: 0, y: -20, z: 0 } },
        restraint: {
          canTranslateAlongX: false,
          canTranslateAlongY: false,
          canTranslateAlongZ: false,
          canRotateAboutX: false,
          canRotateAboutY: false,
          canRotateAboutZ: false,
        },
      },
      {
        tempId: "n4",
        location: { type: "spatial", point: { x: 0, y: 0, z: -20 } },
        restraint: {
          canTranslateAlongX: false,
          canTranslateAlongY: false,
          canTranslateAlongZ: false,
          canRotateAboutX: false,
          canRotateAboutY: false,
          canRotateAboutZ: false,
        },
      },
    ],
  },
  materials: {
    create: [
      {
        name: "1",
        modulusOfElasticity: 29000,
        modulusOfRigidity: 11500,
        units: { pressure: "KilopoundForcePerSquareInch" },
      },
    ],
  },
  sectionProfiles: {
    create: [
      {
        name: "Default",
        area: 32.9,
        strongAxisMomentOfInertia: 716,
        weakAxisMomentOfInertia: 236,
        torsionalConstant: 15.1,
        warpingConstant: 0,
        strongAxisPlasticSectionModulus: 0,
        weakAxisPlasticSectionModulus: 0,
        strongAxisElasticSectionModulus: 0,
        weakAxisElasticSectionModulus: 0,
        strongAxisShearArea: 1,
        weakAxisShearArea: 1,
      },
    ],
  },
  loadCases: {
    create: [{ tempId: "lc1", name: "Load Case 1" }],
  },
};

export type KassimaliExample8_4FollowUpInput = {
  nodeIds: {
    node1: string;
    node2: string;
    node3: string;
    node4: string;
  };
  loadCaseId: string;
};

export const createKassimaliExample8_4FollowUpRevision = (
  input: KassimaliExample8_4FollowUpInput,
): CreateModelRevisionRequest => ({
  element1ds: {
    create: [
      {
        tempId: "e1",
        startNodeId: input.nodeIds.node2,
        endNodeId: input.nodeIds.node1,
        materialName: "1",
        sectionProfileName: "Default",
      },
      {
        tempId: "e2",
        startNodeId: input.nodeIds.node3,
        endNodeId: input.nodeIds.node1,
        materialName: "1",
        sectionProfileName: "Default",
      },
      {
        tempId: "e3",
        startNodeId: input.nodeIds.node4,
        endNodeId: input.nodeIds.node1,
        materialName: "1",
        sectionProfileName: "Default",
      },
    ],
  },
  pointLoads: {
    create: [
      {
        tempId: "pl1",
        nodeId: input.nodeIds.node1,
        loadCaseId: input.loadCaseId,
        force: {
          forceAlongX: 0,
          forceAlongY: -30,
          forceAlongZ: 0,
          momentAboutX: 0,
          momentAboutY: 0,
          momentAboutZ: 0,
        },
        direction: { x: 0, y: -1, z: 0 },
        units: { force: "KilopoundForce", torque: "KilopoundForceInch" },
      },
      {
        tempId: "pl2",
        nodeId: input.nodeIds.node2,
        loadCaseId: input.loadCaseId,
        force: {
          forceAlongX: 0,
          forceAlongY: -30,
          forceAlongZ: 0,
          momentAboutX: 0,
          momentAboutY: 0,
          momentAboutZ: 0,
        },
        direction: { x: 0, y: -1, z: 0 },
        units: { force: "KilopoundForce", torque: "KilopoundForceInch" },
      },
      {
        tempId: "ml1",
        nodeId: input.nodeIds.node1,
        loadCaseId: input.loadCaseId,
        force: {
          forceAlongX: 0,
          forceAlongY: 0,
          forceAlongZ: 0,
          momentAboutX: -1800,
          momentAboutY: 0,
          momentAboutZ: 0,
        },
        direction: { x: 1, y: 0, z: 0 },
        units: { force: "KilopoundForce", torque: "KilopoundForceInch" },
      },
      {
        tempId: "ml2",
        nodeId: input.nodeIds.node1,
        loadCaseId: input.loadCaseId,
        force: {
          forceAlongX: 0,
          forceAlongY: 0,
          forceAlongZ: 0,
          momentAboutX: 0,
          momentAboutY: 0,
          momentAboutZ: 1800,
        },
        direction: { x: 0, y: 0, z: 1 },
        units: { force: "KilopoundForce", torque: "KilopoundForceInch" },
      },
      {
        tempId: "ml3",
        nodeId: input.nodeIds.node1,
        loadCaseId: input.loadCaseId,
        force: {
          forceAlongX: 0,
          forceAlongY: 0,
          forceAlongZ: 0,
          momentAboutX: 0,
          momentAboutY: 0,
          momentAboutZ: 100,
        },
        direction: { x: 0, y: 0, z: 1 },
        units: { force: "KilopoundForce", torque: "KilopoundForceFoot" },
      },
      {
        tempId: "ml4",
        nodeId: input.nodeIds.node2,
        loadCaseId: input.loadCaseId,
        force: {
          forceAlongX: 0,
          forceAlongY: 0,
          forceAlongZ: 0,
          momentAboutX: 0,
          momentAboutY: 0,
          momentAboutZ: -100,
        },
        direction: { x: 0, y: 0, z: -1 },
        units: { force: "KilopoundForce", torque: "KilopoundForceFoot" },
      },
    ],
  },
  loadCombinations: {
    create: [
      { tempId: "comb1", loadCaseFactors: { [input.loadCaseId]: 1 } },
      { tempId: "comb2", loadCaseFactors: { [input.loadCaseId]: 1 } },
    ],
  },
});
