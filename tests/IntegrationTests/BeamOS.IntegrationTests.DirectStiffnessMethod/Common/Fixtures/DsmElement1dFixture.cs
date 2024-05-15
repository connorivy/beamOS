using BeamOS.Tests.Common.SolvedProblems.Fixtures;

namespace BeamOS.IntegrationTests.DirectStiffnessMethod.Common.Fixtures;

public partial class DsmElement1dFixture(Element1dFixture fixture)
{
    public Element1dFixture Fixture { get; } = fixture;

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
}
