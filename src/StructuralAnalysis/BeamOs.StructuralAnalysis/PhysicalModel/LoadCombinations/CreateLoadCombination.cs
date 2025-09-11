using BeamOs.Common.Api;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.LoadCombinations;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.LoadCombinations;

namespace BeamOs.StructuralAnalysis.Api.Endpoints.PhysicalModel.LoadCombinations;

[BeamOsRoute(RouteConstants.ModelRoutePrefixWithTrailingSlash + "load-combinations")]
[BeamOsEndpointType(Http.Post)]
[BeamOsRequiredAuthorizationLevel(UserAuthorizationLevel.Contributor)]
internal class CreateLoadCombination(
    CreateLoadCombinationCommandHandler createLoadCombinationCommandHandler
) : BeamOsModelResourceBaseEndpoint<LoadCombinationData, LoadCombinationContract>
{
    public override async Task<Result<LoadCombinationContract>> ExecuteRequestAsync(
        ModelResourceRequest<LoadCombinationData> req,
        CancellationToken ct = default
    ) => await createLoadCombinationCommandHandler.ExecuteAsync(req, ct);
}
