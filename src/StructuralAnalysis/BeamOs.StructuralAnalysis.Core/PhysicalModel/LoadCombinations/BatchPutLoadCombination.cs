using BeamOs.Common.Api;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.LoadCombinations;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Nodes;

namespace BeamOs.StructuralAnalysis.Api.Endpoints.PhysicalModel.LoadCombinations;

[BeamOsRoute(RouteConstants.ModelRoutePrefixWithTrailingSlash + "load-combinations")]
[BeamOsEndpointType(Http.Put)]
[BeamOsRequiredAuthorizationLevel(UserAuthorizationLevel.Contributor)]
internal class BatchPutLoadCombination(
    BatchPutLoadCombinationCommandHandler putLoadCombinationCommandHandler
) : BeamOsModelResourceBaseEndpoint<LoadCombinationContract[], BatchResponse>
{
    public override async Task<Result<BatchResponse>> ExecuteRequestAsync(
        ModelResourceRequest<LoadCombinationContract[]> req,
        CancellationToken ct = default
    ) => await putLoadCombinationCommandHandler.ExecuteAsync(req, ct);
}
