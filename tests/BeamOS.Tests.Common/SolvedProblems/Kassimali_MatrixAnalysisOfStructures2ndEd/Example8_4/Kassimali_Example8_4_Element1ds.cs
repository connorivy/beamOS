using BeamOS.Tests.Common.SolvedProblems.Fixtures;

namespace BeamOS.Tests.Common.SolvedProblems.Kassimali_MatrixAnalysisOfStructures2ndEd.Example8_4;

public class Kassimali_Example8_4_Element1ds
{
    public static Element1dFixture Element1 { get; } =
        new(
            Kassimali_Example8_4.Instance,
            Kassimali_Example8_4_Nodes.Node2,
            Kassimali_Example8_4_Nodes.Node1,
            Kassimali_Example8_4_Materials.Steel29000Ksi,
            Kassimali_Example8_4_SectionProfiles.Profile33in2,
            Kassimali_Example8_4.StaticUnitSettings,
            nameof(Element1)
        );

    public static Element1dFixture Element2 { get; } =
        new(
            Kassimali_Example8_4.Instance,
            Kassimali_Example8_4_Nodes.Node3,
            Kassimali_Example8_4_Nodes.Node1,
            Kassimali_Example8_4_Materials.Steel29000Ksi,
            Kassimali_Example8_4_SectionProfiles.Profile33in2,
            Kassimali_Example8_4.StaticUnitSettings,
            nameof(Element2)
        )
        {
            SectionProfileRotation = new(90, UnitsNet.Units.AngleUnit.Degree)
        };

    public static Element1dFixture Element3 { get; } =
        new(
            Kassimali_Example8_4.Instance,
            Kassimali_Example8_4_Nodes.Node4,
            Kassimali_Example8_4_Nodes.Node1,
            Kassimali_Example8_4_Materials.Steel29000Ksi,
            Kassimali_Example8_4_SectionProfiles.Profile33in2,
            Kassimali_Example8_4.StaticUnitSettings,
            nameof(Element3)
        )
        {
            SectionProfileRotation = new(30, UnitsNet.Units.AngleUnit.Degree)
        };
}
