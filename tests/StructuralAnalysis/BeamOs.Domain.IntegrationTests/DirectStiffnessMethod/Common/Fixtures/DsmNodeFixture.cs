using BeamOS.Tests.Common.Fixtures;

namespace BeamOs.Domain.IntegrationTests.DirectStiffnessMethod.Common.Fixtures;

public class DsmNodeFixture(NodeFixture2 fixture)
{
    public NodeFixture2 Fixture { get; } = fixture;
}
