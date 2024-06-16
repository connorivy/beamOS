using BeamOs.Contracts.AnalyticalResults;
using BeamOs.Contracts.AnalyticalResults.AnalyticalNode;
using BeamOs.Contracts.AnalyticalResults.Forces;
using BeamOs.Domain.Common.ValueObjects;
using Riok.Mapperly.Abstractions;

namespace BeamOS.Tests.Common.SolvedProblems.Fixtures.Mappers.ToResponse;

[Mapper]
internal static partial class NodeResultFixtureToResponse
{
    public static NodeResultResponse ToResponse(
        this NodeResultFixture fixture,
        Dictionary<NodeFixture, string> nodeFixtureToIdMap
    )
    {
        return new(
            nodeFixtureToIdMap[fixture.Node],
            fixture.Forces.ToResponse(),
            fixture.Displacements.ToResponse()
        );
    }

    private static partial ForcesResponse ToResponse(this Forces forces);

    private static partial DisplacementsResponse ToResponse(this Displacements displacements);
}
