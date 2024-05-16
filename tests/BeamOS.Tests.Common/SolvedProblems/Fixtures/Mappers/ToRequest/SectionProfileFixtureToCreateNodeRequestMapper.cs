using BeamOs.Api.Common.Mappers;
using BeamOs.Contracts.PhysicalModel.SectionProfile;

namespace BeamOS.Tests.Common.SolvedProblems.Fixtures.Mappers.ToRequest;

internal static partial class SectionProfileFixtureToCreateNodeRequestMapper
{
    public static CreateSectionProfileRequest ToRequest(
        this SectionProfileFixture fixture,
        string modelId
    )
    {
        return new(
            modelId,
            fixture.Area.ToDto(),
            fixture.StrongAxisMomentOfInertia.ToDto(),
            fixture.WeakAxisMomentOfInertia.ToDto(),
            fixture.PolarMomentOfInertia.ToDto()
        );
    }
}
