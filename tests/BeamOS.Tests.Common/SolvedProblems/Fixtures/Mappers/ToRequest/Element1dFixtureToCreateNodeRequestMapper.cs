using BeamOs.Api.Common.Mappers;
using BeamOs.Contracts.PhysicalModel.Element1d;

namespace BeamOS.Tests.Common.SolvedProblems.Fixtures.Mappers;

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
