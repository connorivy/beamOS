using BeamOS.Tests.Common.SolvedProblems.Fixtures;

namespace BeamOS.Tests.Common.SolvedProblems.Kassimali_MatrixAnalysisOfStructures2ndEd.Example8_4;

internal class Kassimali_Example8_4_PointLoads
{
    public static PointLoadFixture PointLoad1 { get; } =
        new(
            Kassimali_Example8_4_Nodes.Node1,
            new(30, UnitsNet.Units.ForceUnit.KilopoundForce),
            new(0, -1, 0)
        );

    public static PointLoadFixture PointLoad2 { get; } =
        new(
            Kassimali_Example8_4_Nodes.Node2,
            new(30, UnitsNet.Units.ForceUnit.KilopoundForce),
            new(0, -1, 0)
        );
}
