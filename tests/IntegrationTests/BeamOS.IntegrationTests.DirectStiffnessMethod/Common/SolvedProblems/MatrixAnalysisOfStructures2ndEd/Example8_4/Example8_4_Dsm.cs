using BeamOS.IntegrationTests.DirectStiffnessMethod.Common.Fixtures;
using BeamOS.IntegrationTests.DirectStiffnessMethod.Common.Interfaces;
using BeamOS.Tests.Common.SolvedProblems.DirectStiffnessMethod.Kassimali_MatrixAnalysisOfStructures2ndEd.Example8_4;
using BeamOS.Tests.Common.SolvedProblems.Kassimali_MatrixAnalysisOfStructures2ndEd.Example8_4;

namespace BeamOS.IntegrationTests.DirectStiffnessMethod.Common.SolvedProblems.MatrixAnalysisOfStructures2ndEd.Example8_4;

public class Example8_4_Dsm
    : DsmModelFixture,
        IHasStructuralStiffnessMatrix,
        IHasExpectedReactionVector,
        IHasExpectedDisplacementVector
{
    public Example8_4_Dsm(Kassimali_Example8_4 modelFixture)
        : base(modelFixture) { }

    public override DsmNodeFixture[] DsmNodeFixtures { get; } =

        [
            new(Kassimali_Example8_4_Nodes.Node1),
            new(Kassimali_Example8_4_Nodes.Node2),
            new(Kassimali_Example8_4_Nodes.Node3),
            new(Kassimali_Example8_4_Nodes.Node4),
        ];
    public override DsmElement1dFixture[] DsmElement1dFixtures { get; } =

        [
            Example8_4.DsmElement1dFixtures.Element1,
            Example8_4.DsmElement1dFixtures.Element2,
            Example8_4.DsmElement1dFixtures.Element3,
        ];
    public double[,] ExpectedStructuralStiffnessMatrix { get; } =
        new double[,]
        {
            { 3990.3, -5.2322, 0, -627.87, -1075.4, 712.92 },
            { -5.2322, 4008.4, 0, 1800.4, 627.87, -2162.9 },
            { 0, 0, 3999.4, -2162.9, 712.92, 0 },
            { -627.87, 1800.4, -2162.9, 634857, 100459, 0 },
            { -1075.4, 627.87, 712.92, 100459, 286857, 0 },
            { 712.92, -2162.9, 0, 0, 0, 460857 }
        };

    public double[] ExpectedReactionVector { get; } =

        [
            5.3757,
            44.106 - 30, // subtracting qf because we're not using fixed end forces. this will change.
            -0.74272,
            2.1722,
            58.987,
            2330.52 - 1200, // subtracting 1200 for same reason as above ^
            -4.6249,
            11.117,
            -6.4607,
            -515.55,
            -0.76472,
            369.67,
            -0.75082,
            4.7763,
            7.2034,
            -383.5,
            -60.166,
            -4.702
        ];

    public double[] ExpectedDisplacementVector { get; } =
        [-1.3522e-3, -2.7965e-3, -1.812e-3, -3.0021e-3, 1.0569e-3, 6.4986e-3];
}
