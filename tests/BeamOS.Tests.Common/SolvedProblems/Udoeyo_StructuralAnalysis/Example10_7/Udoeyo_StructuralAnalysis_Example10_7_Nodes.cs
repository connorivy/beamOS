using BeamOs.Domain.Common.ValueObjects;
using BeamOS.Tests.Common.Fixtures;

namespace BeamOS.Tests.Common.SolvedProblems.Udoeyo_StructuralAnalysis.Example10_7;

public static class Udoeyo_StructuralAnalysis_Example10_7_Nodes
{
    private static Restraint free2d = new(true, true, false, false, false, true);
    private static Restraint pinned2d = new(false, false, false, false, false, true);
    public static NodeFixture2 NodeA { get; } =
        new()
        {
            ModelId = Udoeyo_StructuralAnalysis_Example10_7.IdStatic,
            LocationPoint = new(0, 0, 0, UnitsNet.Units.LengthUnit.Foot),
            Restraint = Restraint.Fixed,
        };

    public static NodeFixture2 NodeB { get; } =
        new()
        {
            ModelId = Udoeyo_StructuralAnalysis_Example10_7.IdStatic,
            LocationPoint = new(0, 8, 0, UnitsNet.Units.LengthUnit.Foot),
            Restraint = free2d
        };

    public static NodeFixture2 NodeC { get; } =
        new()
        {
            ModelId = Udoeyo_StructuralAnalysis_Example10_7.IdStatic,
            LocationPoint = new(6, 8, 0, UnitsNet.Units.LengthUnit.Foot),
            Restraint = free2d
        };

    public static NodeFixture2 NodeD { get; } =
        new()
        {
            ModelId = Udoeyo_StructuralAnalysis_Example10_7.IdStatic,
            LocationPoint = new(6, 4, 0, UnitsNet.Units.LengthUnit.Foot),
            Restraint = free2d,
            PointLoads =  [Udoeyo_StructuralAnalysis_Example10_7_PointLoads.PointLoad1]
        };

    public static NodeFixture2[] All { get; } = [NodeA, NodeB, NodeC, NodeD];
}
