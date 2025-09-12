using BeamOs.Common.Api;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.PointLoads;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.PointLoads;

namespace BeamOs.StructuralAnalysis.Api.Endpoints.PhysicalModel.PointLoads;

[BeamOsRoute(RouteConstants.ModelRoutePrefixWithTrailingSlash + "point-loads")]
[BeamOsEndpointType(Http.Post)]
[BeamOsRequiredAuthorizationLevel(UserAuthorizationLevel.Contributor)]
internal class CreatePointLoad(CreatePointLoadCommandHandler createPointLoadCommandHandler)
    : BeamOsModelResourceBaseEndpoint<CreatePointLoadRequest, PointLoadResponse>
{
    public override async Task<Result<PointLoadResponse>> ExecuteRequestAsync(
        ModelResourceRequest<CreatePointLoadRequest> req,
        CancellationToken ct = default
    ) => await createPointLoadCommandHandler.ExecuteAsync(req, ct);
}
