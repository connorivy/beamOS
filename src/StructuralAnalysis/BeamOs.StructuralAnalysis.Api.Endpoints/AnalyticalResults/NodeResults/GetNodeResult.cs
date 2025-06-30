using BeamOs.Common.Api;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Application.AnalyticalResults.NodeResults;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.Element1ds;
using BeamOs.StructuralAnalysis.Contracts.AnalyticalResults.NodeResult;

namespace BeamOs.StructuralAnalysis.Api.Endpoints.AnalyticalResults.NodeResults;

[BeamOsRoute(
    RouteConstants.ModelRoutePrefixWithTrailingSlash + "result-sets/{resultSetId}/node-results/{id}"
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
