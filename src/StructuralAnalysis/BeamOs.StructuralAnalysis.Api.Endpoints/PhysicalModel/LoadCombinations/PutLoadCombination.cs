using BeamOs.Common.Api;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.LoadCombinations;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.LoadCombinations;
using LoadCombination = BeamOs.StructuralAnalysis.Contracts.PhysicalModel.LoadCombinations.LoadCombination;

namespace BeamOs.StructuralAnalysis.Api.Endpoints.PhysicalModel.LoadCombinations;

[BeamOsRoute(RouteConstants.ModelRoutePrefixWithTrailingSlash + "load-combinations/{id}")]
[BeamOsEndpointType(Http.Put)]
[BeamOsRequiredAuthorizationLevel(UserAuthorizationLevel.Contributor)]
public class PutLoadCombination(PutLoadCombinationCommandHandler putLoadCombinationCommandHandler)
    : BeamOsModelResourceWithIntIdBaseEndpoint<
        PutLoadCombinationCommand,
        LoadCombinationData,
        LoadCombination
    >
{
    public override async Task<Result<LoadCombination>> ExecuteRequestAsync(
        PutLoadCombinationCommand req,
        CancellationToken ct = default
    ) => await putLoadCombinationCommandHandler.ExecuteAsync(req, ct);
}
