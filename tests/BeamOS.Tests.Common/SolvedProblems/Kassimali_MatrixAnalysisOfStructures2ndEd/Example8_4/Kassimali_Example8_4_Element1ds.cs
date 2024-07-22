using BeamOS.Tests.Common.Fixtures;

namespace BeamOS.Tests.Common.SolvedProblems.Kassimali_MatrixAnalysisOfStructures2ndEd.Example8_4;

public class Kassimali_Example8_4_Element1ds
{
    public static Element1dFixture2 Element1 { get; } =
        new()
        {
            Model = new(() => Kassimali_Example8_4.Instance),
            StartNode = Kassimali_Example8_4_Nodes.Node2,
            EndNode = Kassimali_Example8_4_Nodes.Node1,
            Material = Kassimali_Example8_4_Materials.Steel29000Ksi,
            SectionProfile = Kassimali_Example8_4_SectionProfiles.Profile33in2,
            ElementName = nameof(Element1)
        };

    public static Element1dFixture2 Element2 { get; } =
        new()
        {
            Model = new(() => Kassimali_Example8_4.Instance),
            StartNode = Kassimali_Example8_4_Nodes.Node3,
            EndNode = Kassimali_Example8_4_Nodes.Node1,
            Material = Kassimali_Example8_4_Materials.Steel29000Ksi,
            SectionProfile = Kassimali_Example8_4_SectionProfiles.Profile33in2,
            ElementName = nameof(Element2),
            SectionProfileRotation = new(90, UnitsNet.Units.AngleUnit.Degree)
        };

    public static Element1dFixture2 Element3 { get; } =
        new()
        {
            Model = new(() => Kassimali_Example8_4.Instance),
            StartNode = Kassimali_Example8_4_Nodes.Node4,
            EndNode = Kassimali_Example8_4_Nodes.Node1,
            Material = Kassimali_Example8_4_Materials.Steel29000Ksi,
            SectionProfile = Kassimali_Example8_4_SectionProfiles.Profile33in2,
            ElementName = nameof(Element3),
            SectionProfileRotation = new(30, UnitsNet.Units.AngleUnit.Degree)
        };

    public static Element1dFixture2[] All { get; } = [Element1, Element2, Element3];
}
