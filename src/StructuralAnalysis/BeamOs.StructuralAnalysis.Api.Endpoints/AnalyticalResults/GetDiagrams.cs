using BeamOs.Common.Api;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Application.AnalyticalResults;
using BeamOs.StructuralAnalysis.Contracts.AnalyticalResults.Diagrams;

namespace BeamOs.StructuralAnalysis.Api.Endpoints.AnalyticalResults;

[BeamOsRoute(RouteConstants.ModelRoutePrefixWithTrailingSlash + "result-sets/{id}/diagrams")]
[BeamOsEndpointType(Http.Get)]
[BeamOsRequiredAuthorizationLevel(UserAuthorizationLevel.Reviewer)]
public class GetDiagrams(GetDiagramsCommandHandler getDiagramsCommandHandler)
    : BeamOsBaseEndpoint<GetDiagramsRequest, AnalyticalResultsResponse>
{
    public override async Task<Result<AnalyticalResultsResponse>> ExecuteRequestAsync(
        GetDiagramsRequest req,
        CancellationToken ct = default
    ) =>
        await getDiagramsCommandHandler.ExecuteAsync(
            new()
            {
                Id = req.Id,
                ModelId = req.ModelId,
                UnitsOverride = req.UnitsOverride,
            },
            ct
        );
}

public readonly record struct GetDiagramsRequest : IHasModelId
{
    [FromRoute]
    public Guid ModelId { get; init; }

    [FromRoute]
    public int Id { get; init; }

    [FromQuery]
    public string? UnitsOverride { get; init; }
}
