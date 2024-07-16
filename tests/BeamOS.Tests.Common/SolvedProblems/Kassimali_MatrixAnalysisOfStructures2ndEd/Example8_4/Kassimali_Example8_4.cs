using BeamOs.Domain.Common.ValueObjects;
using BeamOs.Domain.PhysicalModel.ModelAggregate.ValueObjects;
using BeamOS.Tests.Common.Fixtures;
using BeamOS.Tests.Common.Interfaces;
using BeamOS.Tests.Common.SolvedProblems.Fixtures;
using BeamOS.Tests.Common.Traits;
using Riok.Mapperly.Abstractions;

namespace BeamOS.Tests.Common.SolvedProblems.Kassimali_MatrixAnalysisOfStructures2ndEd.Example8_4;

[Mapper]
[Kassimali_MatrixAnalysisOfStructures2ndEd]
public partial class Kassimali_Example8_4 : ModelFixture2, IHasExpectedNodeResults2
{
    public override SourceInfo SourceInfo { get; } =
        new(
            "Matrix Analysis Of Structures 2nd Edition by Kassimali",
            FixtureSourceType.Textbook,
            "Example 8.4",
            null,
            "https://dokumen.pub/matrix-analysis-of-structures-3nbsped-9780357448304.html#English"
        );
    public static Guid IdStatic { get; } = new("ddb1e60a-df17-48b0-810a-60e425acf640");
    public override Guid Id => IdStatic;
    public static Kassimali_Example8_4 Instance { get; }
    public override ModelSettings Settings { get; } = new(UnitSettings.K_IN);
    public override NodeFixture2[] Nodes => Kassimali_Example8_4_Nodes.All;
    public override MaterialFixture2[] Materials => [Kassimali_Example8_4_Materials.Steel29000Ksi];
    public override SectionProfileFixture2[] SectionProfiles =>
        [Kassimali_Example8_4_SectionProfiles.Profile33in2];
    public override Element1dFixture2[] Element1ds { get; } =

        [
            Kassimali_Example8_4_Element1ds.Element1,
            Kassimali_Example8_4_Element1ds.Element2,
            Kassimali_Example8_4_Element1ds.Element3
        ];
    public override PointLoadFixture2[] PointLoads =>
        [Kassimali_Example8_4_PointLoads.PointLoad1, Kassimali_Example8_4_PointLoads.PointLoad2];
    public override MomentLoadFixture2[] MomentLoads =>

        [
            Kassimali_Example8_4_MomentLoads.MomentLoad1,
            Kassimali_Example8_4_MomentLoads.MomentLoad2,
            Kassimali_Example8_4_MomentLoads.MomentLoad3,
            Kassimali_Example8_4_MomentLoads.MomentLoad4
        ];

    public NodeResultFixture2[] ExpectedNodeResults { get; } =

        [
            Kassimali_Example8_4_Nodes.Node1ExpectedResult,
            Kassimali_Example8_4_Nodes.Node2ExpectedResult,
            Kassimali_Example8_4_Nodes.Node3ExpectedResult,
            Kassimali_Example8_4_Nodes.Node4ExpectedResult
        ];

    static Kassimali_Example8_4()
    {
        Instance = new();
    }
}
