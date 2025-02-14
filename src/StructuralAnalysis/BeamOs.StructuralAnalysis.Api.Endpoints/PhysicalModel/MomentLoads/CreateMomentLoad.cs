using BeamOs.Common.Api;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.MomentLoads;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.MomentLoad;

namespace BeamOs.StructuralAnalysis.Api.Endpoints.PhysicalModel.MomentLoads;

[BeamOsRoute(RouteConstants.ModelRoutePrefixWithTrailingSlash + "moment-loads")]
[BeamOsEndpointType(Http.Post)]
[BeamOsRequiredAuthorizationLevel(UserAuthorizationLevel.Contributor)]
public class CreateMomentLoad(CreateMomentLoadCommandHandler createMomentLoadCommandHandler)
    : BeamOsModelResourceBaseEndpoint<
        CreateMomentLoadCommand,
        CreateMomentLoadRequest,
        MomentLoadResponse
    >
{
    public override async Task<Result<MomentLoadResponse>> ExecuteRequestAsync(
        CreateMomentLoadCommand req,
        CancellationToken ct = default
    ) => await createMomentLoadCommandHandler.ExecuteAsync(req, ct);
}
