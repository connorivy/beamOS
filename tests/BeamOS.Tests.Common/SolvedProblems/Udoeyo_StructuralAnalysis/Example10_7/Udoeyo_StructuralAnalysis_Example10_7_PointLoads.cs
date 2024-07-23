using BeamOS.Tests.Common.Fixtures;

namespace BeamOS.Tests.Common.SolvedProblems.Udoeyo_StructuralAnalysis.Example10_7;

public static class Udoeyo_StructuralAnalysis_Example10_7_PointLoads
{
    public static PointLoadFixture2 PointLoad1 { get; } =
        new()
        {
            ModelId = Udoeyo_StructuralAnalysis_Example10_7.IdStatic,
            Direction = new(-1, 0, 0),
            Force = new(1, UnitsNet.Units.ForceUnit.KilopoundForce),
            Node = new(() => Udoeyo_StructuralAnalysis_Example10_7_Nodes.NodeD)
        };
}
