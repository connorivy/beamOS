using BeamOs.Common.Api;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Application.AnalyticalResults;
using BeamOs.StructuralAnalysis.Contracts.AnalyticalResults.Diagrams;

namespace BeamOs.StructuralAnalysis.Api.Endpoints.AnalyticalResults;

[BeamOsRoute(RouteConstants.ModelRoutePrefixWithTrailingSlash + "result-sets/{id:int}/diagrams")]
[BeamOsEndpointType(Http.Get)]
[BeamOsRequiredAuthorizationLevel(UserAuthorizationLevel.Reviewer)]
internal class GetDiagrams(GetDiagramsCommandHandler getDiagramsCommandHandler)
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
