using BeamOS.Tests.Common.Fixtures;

namespace BeamOS.Tests.Common.SolvedProblems.Kassimali_MatrixAnalysisOfStructures2ndEd.Example3_8;

public static class Kassimali_Example3_8_PointLoads
{
    public static PointLoadFixture2 PointLoad1 { get; } =
        new()
        {
            Force = new(150, UnitsNet.Units.ForceUnit.KilopoundForce),
            Direction = new(1, 0, 0),
            ModelId = Kassimali_Example3_8.IdStatic,
            Node = new(() => Kassimali_Example3_8_Nodes.Node1)
        };

    public static PointLoadFixture2 PointLoad2 { get; } =
        new()
        {
            Force = new(300, UnitsNet.Units.ForceUnit.KilopoundForce),
            Direction = new(0, -1, 0),
            ModelId = Kassimali_Example3_8.IdStatic,
            Node = new(() => Kassimali_Example3_8_Nodes.Node1)
        };

    public static PointLoadFixture2[] All { get; } = [PointLoad1, PointLoad2];
}
