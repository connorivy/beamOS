using BeamOs.Common.Api;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Application.AnalyticalResults.ResultSets;

namespace BeamOs.StructuralAnalysis.Api.Endpoints.AnalyticalResults;

[BeamOsRoute(RouteConstants.ModelRoutePrefixWithTrailingSlash + "result-sets")]
[BeamOsEndpointType(Http.Delete)]
[BeamOsRequiredAuthorizationLevel(UserAuthorizationLevel.Contributor)]
public class DeleteAllResultSets(DeleteAllResultSetsCommandHandler deleteResultSetsCommandHandler)
    : BeamOsModelIdRequestBaseEndpoint<int>
{
    public override async Task<Result<int>> ExecuteRequestAsync(
        ModelResourceRequest req,
        CancellationToken ct = default
    ) => await deleteResultSetsCommandHandler.ExecuteAsync(req.ModelId, ct);
}
