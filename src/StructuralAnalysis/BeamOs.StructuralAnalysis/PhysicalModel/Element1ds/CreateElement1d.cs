using BeamOs.Common.Api;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.Element1ds;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Element1ds;

namespace BeamOs.StructuralAnalysis.Api.Endpoints.PhysicalModel.Element1ds;

[BeamOsRoute(RouteConstants.ModelRoutePrefixWithTrailingSlash + "element1ds")]
[BeamOsEndpointType(Http.Post)]
[BeamOsRequiredAuthorizationLevel(UserAuthorizationLevel.Contributor)]
internal class CreateElement1d(CreateElement1dCommandHandler createElement1dCommandHandler)
    : BeamOsModelResourceBaseEndpoint<CreateElement1dRequest, Element1dResponse>
{
    public override async Task<Result<Element1dResponse>> ExecuteRequestAsync(
        ModelResourceRequest<CreateElement1dRequest> req,
        CancellationToken ct = default
    ) => await createElement1dCommandHandler.ExecuteAsync(req, ct);
}
