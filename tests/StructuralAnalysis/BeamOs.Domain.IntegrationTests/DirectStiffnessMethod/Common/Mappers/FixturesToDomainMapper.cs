using BeamOs.Domain.DirectStiffnessMethod;
using BeamOs.Domain.IntegrationTests.DirectStiffnessMethod.Common.Interfaces;
using BeamOS.Tests.Common.Fixtures;

namespace BeamOS.Tests.Common.SolvedProblems.Fixtures.Mappers.ToDomain;

public static class ModelFixtureToDomainMapper
{
    public static DsmAnalysisModel ToDomain(this IDsmModelFixture fixture) =>
        new(((IModelFixture2)fixture).ToDomain());
}
