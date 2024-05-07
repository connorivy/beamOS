using BeamOS.Tests.Common.SolvedProblems.Fixtures;

namespace BeamOS.Tests.Common.SolvedProblems.DirectStiffnessMethod.Kassimali_MatrixAnalysisOfStructures2ndEd.Example8_4;

internal class Element1ds
{
    public static Element1dFixture Element1 { get; } =
        new(Nodes.Node2, Nodes.Node1, Materials.Steel29000Ksi, SectionProfiles.Profile33in2);

    public static Element1dFixture Element2 { get; } =
        new(Nodes.Node3, Nodes.Node1, Materials.Steel29000Ksi, SectionProfiles.Profile33in2);

    public static Element1dFixture Element3 { get; } =
        new(Nodes.Node4, Nodes.Node1, Materials.Steel29000Ksi, SectionProfiles.Profile33in2);
}
