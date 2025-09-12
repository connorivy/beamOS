using BeamOs.Common.Api;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.MomentLoads;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.MomentLoads;

namespace BeamOs.StructuralAnalysis.Api.Endpoints.PhysicalModel.MomentLoads;

[BeamOsRoute(RouteConstants.ModelRoutePrefixWithTrailingSlash + "moment-loads")]
[BeamOsEndpointType(Http.Post)]
[BeamOsRequiredAuthorizationLevel(UserAuthorizationLevel.Contributor)]
internal class CreateMomentLoad(CreateMomentLoadCommandHandler createMomentLoadCommandHandler)
    : BeamOsModelResourceBaseEndpoint<CreateMomentLoadRequest, MomentLoadResponse>
{
    public override async Task<Result<MomentLoadResponse>> ExecuteRequestAsync(
        ModelResourceRequest<CreateMomentLoadRequest> req,
        CancellationToken ct = default
    ) => await createMomentLoadCommandHandler.ExecuteAsync(req, ct);
}
