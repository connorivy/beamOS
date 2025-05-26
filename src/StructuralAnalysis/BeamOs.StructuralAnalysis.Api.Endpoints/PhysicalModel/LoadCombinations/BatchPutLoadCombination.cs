using BeamOs.Common.Api;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.LoadCombinations;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Nodes;
using LoadCombination = BeamOs.StructuralAnalysis.Contracts.PhysicalModel.LoadCombinations.LoadCombination;

namespace BeamOs.StructuralAnalysis.Api.Endpoints.PhysicalModel.LoadCombinations;

[BeamOsRoute(RouteConstants.ModelRoutePrefixWithTrailingSlash + "load-combinations")]
[BeamOsEndpointType(Http.Put)]
[BeamOsRequiredAuthorizationLevel(UserAuthorizationLevel.Contributor)]
public class BatchPutLoadCombination(
    BatchPutLoadCombinationCommandHandler putLoadCombinationCommandHandler
)
    : BeamOsModelResourceBaseEndpoint<
        BatchPutLoadCombinationCommand,
        LoadCombination[],
        BatchResponse
    >
{
    public override async Task<Result<BatchResponse>> ExecuteRequestAsync(
        BatchPutLoadCombinationCommand req,
        CancellationToken ct = default
    ) => await putLoadCombinationCommandHandler.ExecuteAsync(req, ct);
}
