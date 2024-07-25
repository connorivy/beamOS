using BeamOs.ApiClient.Builders;
using BeamOs.Domain.Common.ValueObjects;
using BeamOs.Domain.PhysicalModel.ModelAggregate.ValueObjects;
using BeamOS.Tests.Common.Fixtures;
using BeamOS.Tests.Common.Interfaces;

namespace BeamOS.Tests.Common.SolvedProblems.Udoeyo_StructuralAnalysis.Example7_11;

public class Udoeyo_StructuralAnalysis_Example7_11
    : ModelFixture2,
        IHasExpectedNodeDisplacementResults
{
    public override ModelSettings Settings { get; } = new(UnitSettings.K_FT);
    public override SourceInfo SourceInfo { get; } =
        new(
            "Structural Analysis by Felix Udoeyo",
            FixtureSourceType.Textbook,
            "Example7_11",
            null,
            "https://temple.manifoldapp.org/read/structural-analysis/section/072c774b-392a-4b99-a65d-f2e9d1fddfbd#:~:text=The%20moment%2Darea%20method%20uses,method%2C%20which%20are%20derived%20below."
        );

    public static Guid IdStatic { get; } = Guid.Parse("e3e93bca-ab6e-424f-98b9-3ef4ac337275");
    public override Guid ModelGuid => IdStatic;
    public override PointLoadFixture2[] PointLoads =>
        Udoeyo_StructuralAnalysis_Example7_11_PointLoads.All;
    public override MaterialFixture2[] Materials =>
        Udoeyo_StructuralAnalysis_Example7_11_Element1ds.Materials;
    public override SectionProfileFixture2[] SectionProfiles =>
        Udoeyo_StructuralAnalysis_Example7_11_Element1ds.Sections;
    public override NodeFixture2[] Nodes => Udoeyo_StructuralAnalysis_Example7_11_Nodes.All;
    public override Element1dFixture2[] Element1ds =>
        Udoeyo_StructuralAnalysis_Example7_11_Element1ds.AllElement1ds;

    public static Udoeyo_StructuralAnalysis_Example7_11 Instance { get; } = new();
    public NodeDisplacementResultFixture[] ExpectedNodeDisplacementResults { get; } =

        [
            new() {
                NodeId = Udoeyo_StructuralAnalysis_Example7_11_Nodes.NodeA.Id,
                RotationAboutZ = new UnitsNet.Angle(.0038, UnitsNet.Units.AngleUnit.Radian),
                DisplacementAlongY = new UnitsNet.Length(-.37, UnitsNet.Units.LengthUnit.Inch)
            }
        ];
}
