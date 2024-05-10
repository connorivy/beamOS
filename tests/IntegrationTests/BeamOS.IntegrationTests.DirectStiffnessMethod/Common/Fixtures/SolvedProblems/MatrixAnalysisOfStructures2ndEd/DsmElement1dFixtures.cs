using BeamOS.Tests.Common.SolvedProblems.DirectStiffnessMethod.Kassimali_MatrixAnalysisOfStructures2ndEd.Example8_4;

namespace BeamOS.IntegrationTests.DirectStiffnessMethod.Common.Fixtures.SolvedProblems.MatrixAnalysisOfStructures2ndEd;

internal static class DsmElement1dFixtures
{
    public static DsmElement1dFixture Element1 { get; } =
        new(Element1ds.Element1)
        {
            ExpectedRotationMatrix = new double[,]
            {
                { 1, 0, 0 },
                { 0, 1, 0 },
                { 0, 0, 1 }
            },
            ExpectedTransformationMatrix = new double[,]
            {
                { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 }
            },
            ExpectedLocalStiffnessMatrix = new double[,]
            {
                { 3975.4, 0, 0, 0, 0, 0, -3975.4, 0, 0, 0, 0, 0 },
                { 0, 18.024, 0, 0, 0, 2162.9, 0, -18.024, 0, 0, 0, 2162.9 },
                { 0, 0, 5.941, 0, -712.92, 0, 0, 0, -5.941, 0, -712.92, 0, },
                { 0, 0, 0, 723.54, 0, 0, 0, 0, 0, -723.54, 0, 0 },
                { 0, 0, -712.92, 0, 114066.7, 0, 0, 0, 712.92, 0, 57033.3, 0 },
                { 0, 2162.9, 0, 0, 0, 346066.7, 0, -2162.9, 0, 0, 0, 173033.3 },
                { -3975.4, 0, 0, 0, 0, 0, 3975.4, 0, 0, 0, 0, 0 },
                { 0, -18.024, 0, 0, 0, -2162.9, 0, 18.024, 0, 0, 0, -2162.9 },
                { 0, 0, -5.941, 0, 712.92, 0, 0, 0, 5.941, 0, 712.92, 0 },
                { 0, 0, 0, -723.54, 0, 0, 0, 0, 0, 723.54, 0, 0 },
                { 0, 0, -712.92, 0, 57033.3, 0, 0, 0, 712.92, 0, 114066.7, 0 },
                { 0, 2162.9, 0, 0, 0, 173033.3, 0, -2162.9, 0, 0, 0, 346066.7 }
            },
            // Note : same as local stiffness matrix
            ExpectedGlobalStiffnessMatrix = new double[,]
            {
                { 3975.4, 0, 0, 0, 0, 0, -3975.4, 0, 0, 0, 0, 0 },
                { 0, 18.024, 0, 0, 0, 2162.9, 0, -18.024, 0, 0, 0, 2162.9 },
                { 0, 0, 5.941, 0, -712.92, 0, 0, 0, -5.941, 0, -712.92, 0, },
                { 0, 0, 0, 723.54, 0, 0, 0, 0, 0, -723.54, 0, 0 },
                { 0, 0, -712.92, 0, 114066.7, 0, 0, 0, 712.92, 0, 57033.3, 0 },
                { 0, 2162.9, 0, 0, 0, 346066.7, 0, -2162.9, 0, 0, 0, 173033.3 },
                { -3975.4, 0, 0, 0, 0, 0, 3975.4, 0, 0, 0, 0, 0 },
                { 0, -18.024, 0, 0, 0, -2162.9, 0, 18.024, 0, 0, 0, -2162.9 },
                { 0, 0, -5.941, 0, 712.92, 0, 0, 0, 5.941, 0, 712.92, 0 },
                { 0, 0, 0, -723.54, 0, 0, 0, 0, 0, 723.54, 0, 0 },
                { 0, 0, -712.92, 0, 57033.3, 0, 0, 0, 712.92, 0, 114066.7, 0 },
                { 0, 2162.9, 0, 0, 0, 173033.3, 0, -2162.9, 0, 0, 0, 346066.7 }
            },
            ExpectedLocalFixedEndForces =  [0, 30, 0, 0, 0, 12000, 0, 30, 0, 0, 0, -1200],
            ExpectedLocalEndDisplacements =
            [
                0,
                0,
                0,
                0,
                0,
                0,
                -1.3522e-3,
                -2.7965e-3,
                -1.812e-3,
                -3.0021e-3,
                1.0569e-3,
                6.4986e-3
            ],
            ExpectedLocalEndForces =
            [
                5.3757,
                44.106,
                -0.74272,
                2.1722,
                58.987,
                2330.5,
                -5.3757,
                15.894,
                0.74272,
                -2.1722,
                119.27,
                1055
            ]
        };

    public static DsmElement1dFixture Element2 { get; } =
        new(Element1ds.Element2)
        {
            ExpectedRotationMatrix = new double[,]
            {
                { 0, 1, 0 },
                { 0, 0, 1 },
                { 1, 0, 0 }
            },
            ExpectedTransformationMatrix = new double[,]
            {
                { 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
                { 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0 }
            },
            ExpectedGlobalStiffnessMatrix = new double[,]
            {
                { 5.941, 0, 0, 0, 0, -712.92, -5.941, 0, 0, 0, 0, -712.92 },
                { 0, 3975.4, 0, 0, 0, 0, 0, -3975.4, 0, 0, 0, 0 },
                { 0, 0, 18.024, 2162.9, 0, 0, 0, 0, -18.024, 2162.9, 0, 0 },
                { 0, 0, 2162.9, 346066.7, 0, 0, 0, 0, -2162.9, 173033.3, 0, 0 },
                { 0, 0, 0, 0, 723.54, 0, 0, 0, 0, 0, -723.54, 0 },
                { -712.92, 0, 0, 0, 0, 114066.7, 712.92, 0, 0, 0, 0, 57033.3 },
                { -5.941, 0, 0, 0, 0, 712.92, 5.941, 0, 0, 0, 0, 712.92 },
                { 0, -3975.4, 0, 0, 0, 0, 0, 3975.4, 0, 0, 0, 0 },
                { 0, 0, -18.024, -2162.9, 0, 0, 0, 0, 18.024, -2162.9, 0, 0 },
                { 0, 0, 2162.9, 173033.3, 0, 0, 0, 0, -2162.9, 346066.7, 0, 0 },
                { 0, 0, 0, 0, -723.54, 0, 0, 0, 0, 0, 723.54, 0 },
                { -712.92, 0, 0, 0, 0, 57033.3, 712.92, 0, 0, 0, 0, 114066.7 },
            },
            ExpectedLocalFixedEndForces =  [0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0],
            ExpectedLocalEndDisplacements =
            [
                0,
                0,
                0,
                0,
                0,
                0,
                -2.7965e-3,
                -1.812e-3,
                -1.3522e-3,
                1.0569e-3,
                6.4986e-3,
                -3.0021e-3
            ],
            ExpectedGlobalEndDisplacements =
            [
                0,
                0,
                0,
                0,
                0,
                0,
                -1.3522e-3,
                -2.7965e-3,
                -1.812e-3,
                -3.0021e-3,
                1.0569e-3,
                6.4986e-3
            ],
            ExpectedLocalEndForces =
            [
                11.117,
                -6.4607,
                -4.6249,
                -0.76472,
                369.67,
                -515.55,
                -11.117,
                6.4607,
                4.6249,
                0.76472,
                740.31,
                -1035,
            ],
            ExpectedGlobalEndForces =
            [
                -4.6249,
                11.117,
                -6.4607,
                -515.55,
                -0.76472,
                369.67,
                4.6249,
                -11.117,
                6.4607,
                -1,
                035,
                0.76472,
                740.31,
            ]
        };

    public static DsmElement1dFixture Element3 { get; } =
        new(Element1ds.Element3)
        {
            ExpectedRotationMatrix = new double[,]
            {
                { 0, 0, 1 },
                { -.5, .86603, 0 },
                { -.86603, -.5, 0 }
            },
            ExpectedTransformationMatrix = new double[,]
            {
                { 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                { -0.5, 0.86603, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                { -0.86603, -0.5, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, -0.5, 0.86603, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, -0.86603, -0.5, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, -0.5, 0.86603, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, -0.86603, -0.5, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
                { 0, 0, 0, 0, 0, 0, 0, 0, 0, -0.5, 0.86603, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0, 0, -0.86603, -0.5, 0 }
            },
            ExpectedGlobalStiffnessMatrix = new double[,]
            {
                { 8.9618, -5.2322, 0, 627.87, 1075.4, 0, -8.9618, 5.2322, 0, 627.87, 1075.4, 0 },
                {
                    -5.2322,
                    15.003,
                    0,
                    -1800.4,
                    -627.87,
                    0,
                    5.2322,
                    -15.003,
                    0,
                    -1800.4,
                    -627.87,
                    0
                },
                { 0, 0, 3975.4, 0, 0, 0, 0, 0, -3975.4, 0, 0, 0 },
                {
                    627.87,
                    -1800.4,
                    0,
                    288066.7,
                    100458.9,
                    0,
                    -627.87,
                    1800.4,
                    0,
                    144033.3,
                    50229.5,
                    0
                },
                {
                    1075.4,
                    -627.87,
                    0,
                    100458.9,
                    172066.7,
                    0,
                    -1075.4,
                    627.87,
                    0,
                    50229.5,
                    86033.3,
                    0
                },
                { 0, 0, 0, 0, 0, 723.54, 0, 0, 0, 0, 0, -723.54 },
                {
                    -8.9618,
                    5.2322,
                    0,
                    -627.87,
                    -1075.4,
                    0,
                    8.9618,
                    -5.2322,
                    0,
                    -627.87,
                    -1075.4,
                    0
                },
                { 5.2322, -15.003, 0, 1800.4, 627.87, 0, -5.2322, 15.003, 0, 1800.4, 627.87, 0 },
                { 0, 0, -3975.4, 0, 0, 0, 0, 0, 3975.4, 0, 0, 0 },
                {
                    627.87,
                    -1800.4,
                    0,
                    144033.3,
                    50229.5,
                    0,
                    -627.87,
                    1800.4,
                    0,
                    288066.7,
                    100458.9,
                    0
                },
                {
                    1075.4,
                    -627.87,
                    0,
                    50229.5,
                    86033.3,
                    0,
                    -1075.4,
                    627.87,
                    0,
                    100458.9,
                    172066.7,
                    0
                },
                { 0, 0, 0, 0, 0, -723.54, 0, 0, 0, 0, 0, 723.54 }
            },
            ExpectedLocalFixedEndForces =  [0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0],
            ExpectedLocalEndDisplacements =
            [
                0,
                0,
                0,
                0,
                0,
                0,
                -1.812e-3,
                -1.7457e-3,
                2.5693e-3,
                6.4986e-3,
                2.4164e-3,
                2.0714e-3
            ],
            ExpectedGlobalEndDisplacements =
            [
                0,
                0,
                0,
                0,
                0,
                0,
                -1.3522e-3,
                -2.7965e-3,
                -1.812e-3,
                -3.0021e-3,
                1.0569e-3,
                6.4986e-3
            ],
            ExpectedLocalEndForces =
            [
                7.2034,
                4.5118,
                -1.7379,
                -4.702,
                139.65,
                362.21,
                -7.2034,
                -4.5118,
                1.7379,
                4.702,
                277.46,
                720.63,
            ],
            ExpectedGlobalEndForces =
            [
                -0.75082,
                4.7763,
                7.2034,
                -383.5,
                -60.166,
                -4.702,
                0.75082,
                -4.7763,
                -7.2034,
                -762.82,
                -120.03,
                4.702,
            ]
        };
}
