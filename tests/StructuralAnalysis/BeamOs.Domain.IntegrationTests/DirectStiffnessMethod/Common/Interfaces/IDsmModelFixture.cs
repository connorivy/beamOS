using BeamOs.Domain.IntegrationTests.DirectStiffnessMethod.Common.Fixtures;
using BeamOS.Tests.Common.Fixtures;

namespace BeamOs.Domain.IntegrationTests.DirectStiffnessMethod.Common.Interfaces;

public interface IDsmModelFixture : IModelFixture2
{
    //public ModelFixture2 Fixture { get; }
    public DsmElement1dFixture[] DsmElement1dFixtures { get; }
    public DsmNodeFixture[] DsmNodeFixtures { get; }
}
