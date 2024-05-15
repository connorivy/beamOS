using BeamOS.IntegrationTests.DirectStiffnessMethod.Common.Fixtures;
using BeamOS.Tests.Common.SolvedProblems.Fixtures;

namespace BeamOS.IntegrationTests.DirectStiffnessMethod.Common.Interfaces;

public interface IDsmModelFixture
{
    public ModelFixture Fixture { get; }
    public DsmElement1dFixture[] DsmElement1dFixtures { get; }
    public DsmNodeFixture[] DsmNodeFixtures { get; }
}
