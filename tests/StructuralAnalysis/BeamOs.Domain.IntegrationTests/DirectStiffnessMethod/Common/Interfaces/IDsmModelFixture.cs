using BeamOs.Domain.IntegrationTests.DirectStiffnessMethod.Common.Fixtures;
using BeamOS.Tests.Common.SolvedProblems.Fixtures;

namespace BeamOs.Domain.IntegrationTests.DirectStiffnessMethod.Common.Interfaces;

public interface IDsmModelFixture
{
    public ModelFixture Fixture { get; }
    public DsmElement1dFixture[] DsmElement1dFixtures { get; }
    public DsmNodeFixture[] DsmNodeFixtures { get; }
}
