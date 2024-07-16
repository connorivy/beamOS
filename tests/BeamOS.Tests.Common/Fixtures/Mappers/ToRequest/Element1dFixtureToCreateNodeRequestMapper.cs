using BeamOs.Api.Common.Mappers;
using BeamOs.Contracts.PhysicalModel.Element1d;
using BeamOS.Tests.Common.Fixtures;

namespace BeamOS.Tests.Common.Fixtures.Mappers.ToRequest;

internal static partial class Element1dFixtureToCreateNodeRequestMapper
{
    public static CreateElement1dRequest ToRequest(
        this Element1dFixture fixture,
        string modelId,
        Dictionary<NodeFixture, string> nodeFixtureToIdMap,
        Dictionary<MaterialFixture, string> materialFixtureToIdMap,
        Dictionary<SectionProfileFixture, string> sectionProfileFixtureToIdMap
    )
    {
        return new(
            modelId,
            nodeFixtureToIdMap[fixture.StartNode],
            nodeFixtureToIdMap[fixture.EndNode],
            materialFixtureToIdMap[fixture.Material],
            sectionProfileFixtureToIdMap[fixture.SectionProfile],
            fixture.SectionProfileRotation.ToDto()
        );
    }
}
