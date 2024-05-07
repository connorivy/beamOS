using BeamOs.Domain.Common.ValueObjects;
using BeamOS.Tests.Common.SolvedProblems.Fixtures;

namespace BeamOS.Tests.Common.SolvedProblems.DirectStiffnessMethod.Kassimali_MatrixAnalysisOfStructures2ndEd.Example8_4;

public class Example8_4f : ModelFixture, IHasExpectedNodeResults
{
    public override Guid Id => new("ddb1e60a-df17-48b0-810a-60e425acf640");
    public override UnitSettings UnitSettings => UnitSettings.K_IN;
    public override NodeFixture[] NodeFixtures => Nodes.All;
    public override MaterialFixture[] MaterialFixtures => [Materials.Steel29000Ksi];
    public override SectionProfileFixture[] SectionProfileFixtures =>
        [SectionProfiles.Profile33in2];
    public override Element1dFixture[] Element1dFixtures =>
        [Element1ds.Element1, Element1ds.Element2, Element1ds.Element3];
    public override PointLoadFixture[] PointLoadFixtures =>
        [PointLoads.PointLoad1, PointLoads.PointLoad2];
    public override MomentLoadFixture[] MomentLoadFixtures =>

        [
            MomentLoads.MomentLoad1,
            MomentLoads.MomentLoad2,
            MomentLoads.MomentLoad3,
            MomentLoads.MomentLoad4
        ];

    public NodeResultFixture[] ExpectedNodeResults { get; } =

        [
            Nodes.Node1ExpectedResult,
            Nodes.Node2ExpectedResult,
            Nodes.Node3ExpectedResult,
            Nodes.Node4ExpectedResult
        ];
}
