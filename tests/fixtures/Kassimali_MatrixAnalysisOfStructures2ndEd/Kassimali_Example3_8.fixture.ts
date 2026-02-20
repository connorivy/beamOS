import type { CreateModelRevisionRequest, CreateProjectRequest } from "@beamos/openapi-client";

export const kassimaliExample3_8Model: CreateModelRevisionRequest = {
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
                location: { type: "spatial", point: { x: 12, y: 16, z: 0 } },
                restraint: {
                    canTranslateAlongX: true,
                    canTranslateAlongY: true,
                    canTranslateAlongZ: false,
                    canRotateAboutX: false,
                    canRotateAboutY: false,
                    canRotateAboutZ: true,
                },
            },
            {
                tempId: "n2",
                location: { type: "spatial", point: { x: 0, y: 0, z: 0 } },
                restraint: {
                    canTranslateAlongX: false,
                    canTranslateAlongY: false,
                    canTranslateAlongZ: false,
                    canRotateAboutX: false,
                    canRotateAboutY: false,
                    canRotateAboutZ: true,
                },
            },
            {
                tempId: "n3",
                location: { type: "spatial", point: { x: 12, y: 0, z: 0 } },
                restraint: {
                    canTranslateAlongX: false,
                    canTranslateAlongY: false,
                    canTranslateAlongZ: false,
                    canRotateAboutX: false,
                    canRotateAboutY: false,
                    canRotateAboutZ: true,
                },
            },
            {
                tempId: "n4",
                location: { type: "spatial", point: { x: 24, y: 0, z: 0 } },
                restraint: {
                    canTranslateAlongX: false,
                    canTranslateAlongY: false,
                    canTranslateAlongZ: false,
                    canRotateAboutX: false,
                    canRotateAboutY: false,
                    canRotateAboutZ: true,
                },
            },
        ],
    },
    materials: {
        create: [
            {
                name: "992",
                modulusOfElasticity: 29000,
                modulusOfRigidity: 1,
                units: { pressure: "KilopoundForcePerSquareInch" },
            },
        ],
    },
    sectionProfiles: {
        create: [
            {
                name: "8",
                area: 8,
                strongAxisMomentOfInertia: 1,
                weakAxisMomentOfInertia: 1,
                torsionalConstant: 1,
                warpingConstant: 0,
                strongAxisPlasticSectionModulus: 1,
                weakAxisPlasticSectionModulus: 1,
                strongAxisElasticSectionModulus: 0,
                weakAxisElasticSectionModulus: 0,
                strongAxisShearArea: 1,
                weakAxisShearArea: 1,
                units: {
                    area: "SquareInch",
                    areaMomentOfInertia: "InchToTheFourth",
                    warpingMomentOfInertia: "InchToTheSixth",
                    volume: "CubicInch",
                },
            },
            {
                name: "6",
                area: 6,
                strongAxisMomentOfInertia: 1,
                weakAxisMomentOfInertia: 1,
                torsionalConstant: 1,
                warpingConstant: 0,
                strongAxisPlasticSectionModulus: 1,
                weakAxisPlasticSectionModulus: 1,
                strongAxisElasticSectionModulus: 0,
                weakAxisElasticSectionModulus: 0,
                strongAxisShearArea: 1,
                weakAxisShearArea: 1,
                units: {
                    area: "SquareInch",
                    areaMomentOfInertia: "InchToTheFourth",
                    warpingMomentOfInertia: "InchToTheSixth",
                    volume: "CubicInch",
                },
            },
        ],
    },
    loadCases: {
        create: [{ tempId: "lc1", name: "Load Case 1" }],
    },
    element1ds: {
        create: [
            {
                tempId: "e1",
                startNodeId: "n2",
                endNodeId: "n1",
                materialName: "992",
                sectionProfileName: "8",
            },
            {
                tempId: "e2",
                startNodeId: "n3",
                endNodeId: "n1",
                materialName: "992",
                sectionProfileName: "6",
            },
            {
                tempId: "e3",
                startNodeId: "n4",
                endNodeId: "n1",
                materialName: "992",
                sectionProfileName: "8",
            },
        ],
    },
    pointLoads: {
        create: [
            {
                tempId: "pl1",
                nodeId: "n1",
                loadCaseId: "lc1",
                force: {
                    forceAlongX: 150,
                    forceAlongY: 0,
                    forceAlongZ: 0,
                    momentAboutX: 0,
                    momentAboutY: 0,
                    momentAboutZ: 0,
                },
                direction: { x: 1, y: 0, z: 0 },
                units: { force: "KilopoundForce", torque: "KilopoundForceInch" },
            },
            {
                tempId: "pl2",
                nodeId: "n1",
                loadCaseId: "lc1",
                force: {
                    forceAlongX: 0,
                    forceAlongY: -300,
                    forceAlongZ: 0,
                    momentAboutX: 0,
                    momentAboutY: 0,
                    momentAboutZ: 0,
                },
                direction: { x: 0, y: -1, z: 0 },
                units: { force: "KilopoundForce", torque: "KilopoundForceInch" },
            },
        ],
    },
    loadCombinations: {
        create: [
            { tempId: "comb1", loadCaseFactors: { lc1: 1 } },
            { tempId: "comb2", loadCaseFactors: { lc1: 1 } },
        ],
    },
};

export const kassimaliExample3_8Project: {
    project: CreateProjectRequest & { id: string };
    model: CreateModelRevisionRequest;
} = {
    project: {
        id: "019c7bd2-0b18-7828-9d96-3658b1064987",
        name: "Kassimali Example 3.8",
        description: "Model for Kassimali Matrix Analysis of Structures 2nd Ed, Example 3.8",
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
    },
    model: kassimaliExample3_8Model,
};
