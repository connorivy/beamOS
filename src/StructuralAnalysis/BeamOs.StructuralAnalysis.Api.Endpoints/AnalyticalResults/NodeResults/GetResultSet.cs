using BeamOs.Common.Api;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Api.Endpoints.OpenSees;
using BeamOs.StructuralAnalysis.Contracts.AnalyticalResults;
using BeamOs.StructuralAnalysis.Infrastructure.AnalyticalResults.ResultSets;

namespace BeamOs.StructuralAnalysis.Api.Endpoints.AnalyticalResults.NodeResults;

[BeamOsRoute(RouteConstants.ModelRoutePrefixWithTrailingSlash + "result-sets/{id}")]
[BeamOsEndpointType(Http.Get)]
[BeamOsRequiredAuthorizationLevel(UserAuthorizationLevel.Reviewer)]
public class GetResultSet(GetResultSetQueryHandler getResultSetCommandHandler)
    : BeamOsModelResourceQueryBaseEndpoint<ResultSetResponse>
{
    public override async Task<Result<ResultSetResponse>> ExecuteRequestAsync(
        ModelEntityRequest req,
        CancellationToken ct = default
    ) => await getResultSetCommandHandler.ExecuteAsync(req, ct);
}
