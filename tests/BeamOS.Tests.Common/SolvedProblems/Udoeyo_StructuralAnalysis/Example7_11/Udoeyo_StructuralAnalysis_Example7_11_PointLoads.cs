using BeamOS.Tests.Common.Fixtures;

namespace BeamOS.Tests.Common.SolvedProblems.Udoeyo_StructuralAnalysis.Example7_11;

public static class Udoeyo_StructuralAnalysis_Example7_11_PointLoads
{
    public static PointLoadFixture2 PointLoad1 { get; } =
        new()
        {
            ModelId = Udoeyo_StructuralAnalysis_Example7_11.IdStatic,
            Direction = new(0, -1, 0),
            Force = new(3, UnitsNet.Units.ForceUnit.KilopoundForce),
            Node = new(() => Udoeyo_StructuralAnalysis_Example7_11_Nodes.NodeA)
        };

    public static PointLoadFixture2[] All { get; } = [PointLoad1];
}
