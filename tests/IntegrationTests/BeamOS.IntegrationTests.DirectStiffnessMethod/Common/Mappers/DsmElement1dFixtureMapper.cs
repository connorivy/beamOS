using BeamOs.Domain.DirectStiffnessMethod;
using BeamOS.IntegrationTests.DirectStiffnessMethod.Common.Fixtures;
using BeamOS.Tests.Common.SolvedProblems.Fixtures;
using Riok.Mapperly.Abstractions;

namespace BeamOS.IntegrationTests.DirectStiffnessMethod.Common;

[Mapper]
public static partial class DsmElement1dFixtureMapper
{
    public static DsmElement1d ToDomainObjectWithLocalIds(this DsmElement1dFixture fixture) =>
        ToDomainObjectWithLocalIds(fixture.Fixture);

    public static partial DsmElement1d ToDomainObjectWithLocalIds(this Element1dFixture fixture);
}
