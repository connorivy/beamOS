using BeamOS.Tests.Common.SolvedProblems.Fixtures;

namespace BeamOs.Domain.IntegrationTests.DirectStiffnessMethod.Common.Fixtures;

public class DsmNodeFixture(NodeFixture fixture)
{
    public NodeFixture Fixture { get; } = fixture;
}
