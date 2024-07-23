using BeamOs.Domain.Common.ValueObjects;
using BeamOS.Tests.Common.Fixtures;
using UnitsNet.Units;

namespace BeamOS.Tests.Common.SolvedProblems.Udoeyo_StructuralAnalysis.Example7_11;

public static class Udoeyo_StructuralAnalysis_Example7_11_Nodes
{
    public static NodeFixture2 NodeA { get; } =
        new()
        {
            ModelId = Udoeyo_StructuralAnalysis_Example7_11.IdStatic,
            LocationPoint = new(0, 0, 0, LengthUnit.Foot),
            Restraint = Restraint.FreeInXyPlane,
            PointLoads =  [Udoeyo_StructuralAnalysis_Example7_11_PointLoads.PointLoad1]
        };

    public static NodeFixture2 NodeB { get; } =
        new()
        {
            ModelId = Udoeyo_StructuralAnalysis_Example7_11.IdStatic,
            LocationPoint = new(12, 0, 0, LengthUnit.Foot),
            Restraint = Restraint.Fixed
        };

    public static NodeFixture2[] All { get; } = [NodeA, NodeB];
}
