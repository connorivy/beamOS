using BeamOs.Common.Api;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Api.Endpoints.OpenSees;
using BeamOs.StructuralAnalysis.Application.AnalyticalResults.ResultSets;

namespace BeamOs.StructuralAnalysis.Api.Endpoints.AnalyticalResults;

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
