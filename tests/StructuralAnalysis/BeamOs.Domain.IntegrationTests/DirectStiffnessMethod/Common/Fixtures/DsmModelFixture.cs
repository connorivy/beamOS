using BeamOs.Domain.DirectStiffnessMethod;
using BeamOs.Domain.IntegrationTests.DirectStiffnessMethod.Common.Interfaces;
using BeamOS.Tests.Common.SolvedProblems.Fixtures;
using BeamOS.Tests.Common.SolvedProblems.Fixtures.Mappers.ToDomain;

namespace BeamOs.Domain.IntegrationTests.DirectStiffnessMethod.Common.Fixtures;

public abstract class DsmModelFixture : IDsmModelFixture
{
    public DsmModelFixture(ModelFixture2 fixture)
    {
        this.Fixture = fixture;
    }

    public ModelFixture2 Fixture { get; }
    public abstract DsmElement1dFixture[] DsmElement1dFixtures { get; }
    public abstract DsmNodeFixture[] DsmNodeFixtures { get; }

    public DsmAnalysisModel ToDsm() => new(this.Fixture.ToDomain());
}
