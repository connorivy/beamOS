using BeamOS.Tests.Common.Fixtures;
using UnitsNet.Units;

namespace BeamOS.Tests.Common.SolvedProblems.Udoeyo_StructuralAnalysis.Example7_11;

public static class Udoeyo_StructuralAnalysis_Example7_11_Element1ds
{
    public static MaterialFixture2 Material { get; } =
        new()
        {
            ModelId = Udoeyo_StructuralAnalysis_Example7_11.IdStatic,
            ModulusOfElasticity = new(29000, PressureUnit.KilopoundForcePerSquareInch),
            ModulusOfRigidity = new(1, PressureUnit.KilopoundForcePerSquareInch)
        };
    public static MaterialFixture2[] Materials { get; } = [Material];

    public static SectionProfileFixture2 Section { get; } =
        new()
        {
            ModelId = Udoeyo_StructuralAnalysis_Example7_11.IdStatic,
            Area = new(1, AreaUnit.SquareFoot),
            StrongAxisMomentOfInertia = new(280, AreaMomentOfInertiaUnit.InchToTheFourth),
            WeakAxisMomentOfInertia = new(1, AreaMomentOfInertiaUnit.InchToTheFourth),
            PolarMomentOfInertia = new(1, AreaMomentOfInertiaUnit.InchToTheFourth),
        };
    public static SectionProfileFixture2[] Sections { get; } = [Section];
    public static Element1dFixture2 ElementAB { get; } =
        new()
        {
            Model = new(() => Udoeyo_StructuralAnalysis_Example7_11.Instance),
            StartNode = Udoeyo_StructuralAnalysis_Example7_11_Nodes.NodeA,
            EndNode = Udoeyo_StructuralAnalysis_Example7_11_Nodes.NodeB,
            Material = Material,
            SectionProfile = Section,
            ElementName = nameof(ElementAB)
        };

    public static Element1dFixture2[] AllElement1ds { get; } = [ElementAB];
}
