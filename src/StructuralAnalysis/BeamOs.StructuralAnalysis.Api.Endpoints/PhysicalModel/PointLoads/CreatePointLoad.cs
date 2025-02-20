using BeamOs.Common.Api;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.PointLoads;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.PointLoad;

namespace BeamOs.StructuralAnalysis.Api.Endpoints.PhysicalModel.PointLoads;

[BeamOsRoute(RouteConstants.ModelRoutePrefixWithTrailingSlash + "point-loads")]
[BeamOsEndpointType(Http.Post)]
[BeamOsRequiredAuthorizationLevel(UserAuthorizationLevel.Contributor)]
public class CreatePointLoad(CreatePointLoadCommandHandler createPointLoadCommandHandler)
    : BeamOsModelResourceBaseEndpoint<
        CreatePointLoadCommand,
        CreatePointLoadRequest,
        PointLoadResponse
    >
{
    public override async Task<Result<PointLoadResponse>> ExecuteRequestAsync(
        CreatePointLoadCommand req,
        CancellationToken ct = default
    ) => await createPointLoadCommandHandler.ExecuteAsync(req, ct);
}
