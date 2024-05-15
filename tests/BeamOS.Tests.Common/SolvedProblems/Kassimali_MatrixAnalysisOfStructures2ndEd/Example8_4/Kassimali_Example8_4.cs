using BeamOs.Contracts.AnalyticalResults.AnalyticalNode;
using BeamOs.Domain.Common.ValueObjects;
using BeamOS.Tests.Common.SolvedProblems.Fixtures;
using BeamOS.Tests.Common.SolvedProblems.Kassimali_MatrixAnalysisOfStructures2ndEd.Example8_4;
using Riok.Mapperly.Abstractions;

namespace BeamOS.Tests.Common.SolvedProblems.DirectStiffnessMethod.Kassimali_MatrixAnalysisOfStructures2ndEd.Example8_4;

[Mapper]
public partial class Kassimali_Example8_4 : ModelFixture, IHasExpectedNodeResults
{
    public override Guid Id => new("ddb1e60a-df17-48b0-810a-60e425acf640");
    public override UnitSettings UnitSettings => UnitSettings.K_IN;
    public override NodeFixture[] NodeFixtures => Kassimali_Example8_4_Nodes.All;
    public override MaterialFixture[] MaterialFixtures =>
        [Kassimali_Example8_4_Materials.Steel29000Ksi];
    public override SectionProfileFixture[] SectionProfileFixtures =>
        [Kassimali_Example8_4_SectionProfiles.Profile33in2];
    public override Element1dFixture[] Element1dFixtures =>

        [
            Kassimali_Example8_4_Element1ds.Element1,
            Kassimali_Example8_4_Element1ds.Element2,
            Kassimali_Example8_4_Element1ds.Element3
        ];
    public override PointLoadFixture[] PointLoadFixtures =>
        [Kassimali_Example8_4_PointLoads.PointLoad1, Kassimali_Example8_4_PointLoads.PointLoad2];
    public override MomentLoadFixture[] MomentLoadFixtures =>

        [
            Kassimali_Example8_4_MomentLoads.MomentLoad1,
            Kassimali_Example8_4_MomentLoads.MomentLoad2,
            Kassimali_Example8_4_MomentLoads.MomentLoad3,
            Kassimali_Example8_4_MomentLoads.MomentLoad4
        ];

    public NodeResultResponse ToResponse(NodeResultFixture fixture) =>
        this.ToResponseSourceGenerated(fixture);

    public partial NodeResultResponse ToResponseSourceGenerated(NodeResultFixture fixture);

    public NodeResultFixture[] ExpectedNodeResults { get; } =

        [
            Kassimali_Example8_4_Nodes.Node1ExpectedResult,
            Kassimali_Example8_4_Nodes.Node2ExpectedResult,
            Kassimali_Example8_4_Nodes.Node3ExpectedResult,
            Kassimali_Example8_4_Nodes.Node4ExpectedResult
        ];
}
