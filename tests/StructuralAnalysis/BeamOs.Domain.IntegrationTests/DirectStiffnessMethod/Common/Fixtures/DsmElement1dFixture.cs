using BeamOs.Domain.DirectStiffnessMethod;
using BeamOS.Tests.Common.Fixtures;
using BeamOS.Tests.Common.SolvedProblems.Fixtures.Mappers.ToDomain;

namespace BeamOs.Domain.IntegrationTests.DirectStiffnessMethod.Common.Fixtures;

public partial class DsmElement1dFixture(Element1dFixture2 fixture) : FixtureBase2
{
    public Element1dFixture2 Fixture { get; } = fixture;
    public override Guid Id => this.Fixture.Id;

    public double[,]? ExpectedRotationMatrix { get; init; }
    public double[,]? ExpectedTransformationMatrix { get; init; }
    public double[,]? ExpectedLocalStiffnessMatrix { get; init; }
    public double[,]? ExpectedGlobalStiffnessMatrix { get; init; }
    public double[]? ExpectedLocalFixedEndForces { get; init; }
    public double[]? ExpectedGlobalFixedEndForces { get; init; }
    public double[]? ExpectedLocalEndDisplacements { get; init; }
    public double[]? ExpectedGlobalEndDisplacements { get; init; }
    public double[]? ExpectedLocalEndForces { get; init; }
    public double[]? ExpectedGlobalEndForces { get; init; }

    public DsmElement1d ToDomain() => new(this.Fixture.ToDomain());
}
