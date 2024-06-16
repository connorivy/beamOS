using BeamOs.Domain.DirectStiffnessMethod;
using BeamOs.Domain.IntegrationTests.DirectStiffnessMethod.Common.Fixtures;
using BeamOS.Tests.Common.SolvedProblems.Fixtures;
using Riok.Mapperly.Abstractions;

namespace BeamOs.Domain.IntegrationTests.DirectStiffnessMethod.Common.Mappers;

[Mapper]
public static partial class DsmElement1dFixtureMapper
{
    public static DsmElement1d ToDomainObjectWithLocalIds(this DsmElement1dFixture fixture) =>
        fixture.Fixture.ToDomainObjectWithLocalIds();

    public static partial DsmElement1d ToDomainObjectWithLocalIds(this Element1dFixture fixture);
}
