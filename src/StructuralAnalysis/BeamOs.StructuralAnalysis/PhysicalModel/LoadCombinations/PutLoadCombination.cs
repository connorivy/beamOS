using BeamOs.Common.Api;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.LoadCombinations;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.LoadCombinations;

namespace BeamOs.StructuralAnalysis.Api.Endpoints.PhysicalModel.LoadCombinations;

[BeamOsRoute(RouteConstants.ModelRoutePrefixWithTrailingSlash + "load-combinations/{id:int}")]
[BeamOsEndpointType(Http.Put)]
[BeamOsRequiredAuthorizationLevel(UserAuthorizationLevel.Contributor)]
public class PutLoadCombination(PutLoadCombinationCommandHandler putLoadCombinationCommandHandler)
    : BeamOsModelResourceWithIntIdBaseEndpoint<
        PutLoadCombinationCommand,
        LoadCombinationData,
        LoadCombinationContract
    >
{
    public override async Task<Result<LoadCombinationContract>> ExecuteRequestAsync(
        PutLoadCombinationCommand req,
        CancellationToken ct = default
    ) => await putLoadCombinationCommandHandler.ExecuteAsync(req, ct);
}
