using BeamOs.Common.Api;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Api.Endpoints.OpenSees;
using BeamOs.StructuralAnalysis.Application.AnalyticalResults.ResultSets;
using BeamOs.StructuralAnalysis.Contracts.AnalyticalResults;
using BeamOs.StructuralAnalysis.Infrastructure.AnalyticalResults.ResultSets;

namespace BeamOs.StructuralAnalysis.Api.Endpoints.AnalyticalResults.ResultSets;

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

[BeamOsRoute(RouteConstants.ModelRoutePrefixWithTrailingSlash + "result-sets")]
[BeamOsEndpointType(Http.Delete)]
[BeamOsRequiredAuthorizationLevel(UserAuthorizationLevel.Contributor)]
public class DeleteAllResultSets(DeleteResultSetsCommandHandler deleteResultSetsCommandHandler)
    : BeamOsModelIdRequestBaseEndpoint<int>
{
    public override async Task<Result<int>> ExecuteRequestAsync(
        ModelIdRequest req,
        CancellationToken ct = default
    ) => await deleteResultSetsCommandHandler.ExecuteAsync(req.ModelId, ct);
}
