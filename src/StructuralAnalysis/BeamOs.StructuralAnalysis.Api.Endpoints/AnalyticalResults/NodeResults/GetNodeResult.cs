using BeamOs.Common.Api;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Application.AnalyticalResults.NodeResults;
using BeamOs.StructuralAnalysis.Contracts.AnalyticalResults.NodeResult;

namespace BeamOs.StructuralAnalysis.Api.Endpoints.AnalyticalResults.NodeResults;

[BeamOsRoute(
    RouteConstants.ModelRoutePrefixWithTrailingSlash
        + "result-sets/{resultSetId:int}/node-results/{id:int}"
)]
[BeamOsEndpointType(Http.Get)]
[BeamOsRequiredAuthorizationLevel(UserAuthorizationLevel.Reviewer)]
public class GetNodeResult(GetNodeResultQueryHandler getNodeResultCommandHandler)
    : BeamOsAnalyticalResultResourceQueryBaseEndpoint<NodeResultResponse>
{
    public override async Task<Result<NodeResultResponse>> ExecuteRequestAsync(
        GetAnalyticalResultResourceQuery req,
        CancellationToken ct = default
    ) => await getNodeResultCommandHandler.ExecuteAsync(req, ct);
}
