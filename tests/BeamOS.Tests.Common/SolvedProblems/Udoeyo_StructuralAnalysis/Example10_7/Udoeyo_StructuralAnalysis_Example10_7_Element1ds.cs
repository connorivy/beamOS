using BeamOS.Tests.Common.Fixtures;

namespace BeamOS.Tests.Common.SolvedProblems.Udoeyo_StructuralAnalysis.Example10_7;

public static class Udoeyo_StructuralAnalysis_Example10_7_Element1ds
{
    public static MaterialFixture2 Constant { get; } =
        new()
        {
            ModelId = Udoeyo_StructuralAnalysis_Example10_7.IdStatic,
            ModulusOfElasticity = new(1, UnitsNet.Units.PressureUnit.KilopoundForcePerSquareFoot),
            ModulusOfRigidity = new(1, UnitsNet.Units.PressureUnit.KilopoundForcePerSquareFoot)
        };
    public static MaterialFixture2[] Materials { get; } = [Constant];

    public static SectionProfileFixture2 ConstantSection { get; } =
        new()
        {
            ModelId = Udoeyo_StructuralAnalysis_Example10_7.IdStatic,
            Area = new(1, UnitsNet.Units.AreaUnit.SquareFoot),
            StrongAxisMomentOfInertia = new(
                1,
                UnitsNet.Units.AreaMomentOfInertiaUnit.FootToTheFourth
            ),
            WeakAxisMomentOfInertia = new(
                1,
                UnitsNet.Units.AreaMomentOfInertiaUnit.FootToTheFourth
            ),
            PolarMomentOfInertia = new(1, UnitsNet.Units.AreaMomentOfInertiaUnit.FootToTheFourth),
        };
    public static SectionProfileFixture2[] Sections { get; } = [ConstantSection];
    public static Element1dFixture2 ElementAB { get; } =
        new()
        {
            Model = new(() => Udoeyo_StructuralAnalysis_Example10_7.Instance),
            StartNode = Udoeyo_StructuralAnalysis_Example10_7_Nodes.NodeA,
            EndNode = Udoeyo_StructuralAnalysis_Example10_7_Nodes.NodeB,
            Material = Constant,
            SectionProfile = ConstantSection,
            ElementName = nameof(ElementAB)
        };

    public static Element1dFixture2 ElementBC { get; } =
        new()
        {
            Model = new(() => Udoeyo_StructuralAnalysis_Example10_7.Instance),
            StartNode = Udoeyo_StructuralAnalysis_Example10_7_Nodes.NodeB,
            EndNode = Udoeyo_StructuralAnalysis_Example10_7_Nodes.NodeC,
            Material = Constant,
            SectionProfile = ConstantSection,
            ElementName = nameof(ElementBC)
        };

    public static Element1dFixture2 ElementCD { get; } =
        new()
        {
            Model = new(() => Udoeyo_StructuralAnalysis_Example10_7.Instance),
            StartNode = Udoeyo_StructuralAnalysis_Example10_7_Nodes.NodeC,
            EndNode = Udoeyo_StructuralAnalysis_Example10_7_Nodes.NodeD,
            Material = Constant,
            SectionProfile = ConstantSection,
            ElementName = nameof(ElementBC)
        };

    public static Element1dFixture2[] AllElement1ds { get; } = [ElementAB, ElementBC, ElementCD];
}
