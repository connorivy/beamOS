using BeamOs.Domain.Common.ValueObjects;
using BeamOs.Domain.PhysicalModel.ModelAggregate.ValueObjects;
using BeamOS.Tests.Common.Fixtures;
using BeamOS.Tests.Common.Interfaces;
using BeamOS.Tests.Common.SolvedProblems.Fixtures;
using BeamOS.Tests.Common.SolvedProblems.Udoeyo_StructuralAnalysis.Example7_6;

namespace BeamOS.Tests.Common.SolvedProblems.Udoeyo_StructuralAnalysis.Example10_7;

public class Udoeyo_StructuralAnalysis_Example10_7 : ModelFixture2
{
    public override ModelSettings Settings { get; } = new(UnitSettings.K_FT);
    public override SourceInfo SourceInfo { get; } =
        new(
            "Structural Analysis by Felix Udoeyo",
            FixtureSourceType.Textbook,
            "Example7_6",
            null,
            "https://temple.manifoldapp.org/read/structural-analysis/section/072c774b-392a-4b99-a65d-f2e9d1fddfbd#:~:text=The%20moment%2Darea%20method%20uses,method%2C%20which%20are%20derived%20below."
        );

    public static Guid IdStatic { get; } = Guid.Parse("e21ec1cd-cb98-4036-804a-2d51bf740524");
    public override Guid Id => IdStatic;

    public override MaterialFixture2[] Materials =>
        Udoeyo_StructuralAnalysis_Example10_7_Element1ds.Materials;
    public override SectionProfileFixture2[] SectionProfiles =>
        Udoeyo_StructuralAnalysis_Example10_7_Element1ds.Sections;
    public override NodeFixture2[] Nodes => Udoeyo_StructuralAnalysis_Example10_7_Nodes.All;
    public override Element1dFixture2[] Element1ds =>
        Udoeyo_StructuralAnalysis_Example10_7_Element1ds.AllElement1ds;

    public static Udoeyo_StructuralAnalysis_Example10_7 Instance { get; } = new();
}
