using BeamOS.Tests.Common.SolvedProblems.Fixtures;

namespace BeamOS.IntegrationTests.DirectStiffnessMethod.Common.Fixtures;

public class DsmNodeFixture(NodeFixture fixture)
{
    public NodeFixture Fixture { get; } = fixture;
}
