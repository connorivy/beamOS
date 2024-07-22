using BeamOS.Tests.Common.Fixtures;

namespace BeamOS.Tests.Common.SolvedProblems.Kassimali_MatrixAnalysisOfStructures2ndEd.Example3_8;

public static class Kassimali_Example3_8_Element1ds
{
    public static Element1dFixture2 Element1 { get; } =
        new()
        {
            Model = new(() => Kassimali_Example3_8.Instance),
            StartNode = Kassimali_Example3_8_Nodes.Node2,
            EndNode = Kassimali_Example3_8_Nodes.Node1,
            Material = Kassimali_Example3_8_Materials.Default,
            SectionProfile = Kassimali_Example3_8_SectionProfiles.Profile8in2,
            ElementName = nameof(Element1)
        };

    public static Element1dFixture2 Element2 { get; } =
        new()
        {
            Model = new(() => Kassimali_Example3_8.Instance),
            StartNode = Kassimali_Example3_8_Nodes.Node3,
            EndNode = Kassimali_Example3_8_Nodes.Node1,
            Material = Kassimali_Example3_8_Materials.Default,
            SectionProfile = Kassimali_Example3_8_SectionProfiles.Profile6in2,
            ElementName = nameof(Element2)
        };

    public static Element1dFixture2 Element3 { get; } =
        new()
        {
            Model = new(() => Kassimali_Example3_8.Instance),
            StartNode = Kassimali_Example3_8_Nodes.Node4,
            EndNode = Kassimali_Example3_8_Nodes.Node1,
            Material = Kassimali_Example3_8_Materials.Default,
            SectionProfile = Kassimali_Example3_8_SectionProfiles.Profile8in2,
            ElementName = nameof(Element3)
        };

    public static Element1dFixture2[] All { get; } = [Element1, Element2, Element3];
}
