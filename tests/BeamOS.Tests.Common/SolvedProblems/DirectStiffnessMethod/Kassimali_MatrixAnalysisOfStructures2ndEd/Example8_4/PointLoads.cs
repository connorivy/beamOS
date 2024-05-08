using BeamOS.Tests.Common.SolvedProblems.Fixtures;
using MathNet.Spatial.Euclidean;

namespace BeamOS.Tests.Common.SolvedProblems.DirectStiffnessMethod.Kassimali_MatrixAnalysisOfStructures2ndEd.Example8_4;

internal class PointLoads
{
    public static PointLoadFixture PointLoad1 { get; } =
        new(
            Nodes.Node1,
            new(-30, UnitsNet.Units.ForceUnit.KilopoundForce),
            new(0, 1, 0)
        );

    public static PointLoadFixture PointLoad2 { get; } =
        new(
            Nodes.Node2,
            new(-30, UnitsNet.Units.ForceUnit.KilopoundForce),
            new(0, 1, 0)
        );
}
