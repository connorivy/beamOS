using BeamOs.Domain.Common.Utils;
using BeamOs.Domain.Common.ValueObjects;
using BeamOs.Domain.PhysicalModel.ModelAggregate.ValueObjects;
using BeamOS.Tests.Common.Fixtures;
using BeamOS.Tests.Common.Interfaces;
using BeamOS.Tests.Common.SolvedProblems.Fixtures;

namespace BeamOS.Tests.Common.SolvedProblems.Kassimali_MatrixAnalysisOfStructures2ndEd.Example3_8;

public class Kassimali_Example3_8 : ModelFixture2
{
    public static Guid IdStatic { get; } = TypedGuids.G0;
    public override Guid Id => IdStatic;
    public override ModelSettings Settings { get; } = new(UnitSettings.K_IN);
    public override SourceInfo SourceInfo { get; } =
        new(
            "Matrix Analysis Of Structures 2nd Edition by Kassimali",
            FixtureSourceType.Textbook,
            "Example 3.8",
            null,
            "https://dokumen.pub/matrix-analysis-of-structures-3nbsped-9780357448304.html#English"
        );

    public override PointLoadFixture2[] PointLoads => Kassimali_Example3_8_PointLoads.All;
    public override NodeFixture2[] Nodes => Kassimali_Example3_8_Nodes.All;
    public override MaterialFixture2[] Materials => Kassimali_Example3_8_Materials.All;
    public override SectionProfileFixture2[] SectionProfiles =>
        Kassimali_Example3_8_SectionProfiles.All;
    public override Element1dFixture2[] Element1ds => Kassimali_Example3_8_Element1ds.All;

    public static Kassimali_Example3_8 Instance { get; } = new();
}
