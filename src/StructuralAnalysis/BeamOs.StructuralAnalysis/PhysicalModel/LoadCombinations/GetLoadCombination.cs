using BeamOs.Common.Api;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.LoadCombinations;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.LoadCombinations;

namespace BeamOs.StructuralAnalysis.Api.Endpoints.PhysicalModel.LoadCombinations;

[BeamOsRoute(RouteConstants.ModelRoutePrefixWithTrailingSlash + "load-combinations/{id:int}")]
[BeamOsEndpointType(Http.Get)]
[BeamOsRequiredAuthorizationLevel(UserAuthorizationLevel.Reviewer)]
internal class GetLoadCombination(GetLoadCombinationCommandHandler getLoadCombinationCommandHandler)
    : BeamOsModelResourceQueryBaseEndpoint<LoadCombinationContract>
{
    public override async Task<Result<LoadCombinationContract>> ExecuteRequestAsync(
        ModelResourceWithIntIdRequest req,
        CancellationToken ct = default
    ) => await getLoadCombinationCommandHandler.ExecuteAsync(req, ct);
}
