using BeamOs.Common.Api;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.Element1ds;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Element1d;

namespace BeamOs.StructuralAnalysis.Api.Endpoints.PhysicalModel.Element1ds;

[BeamOsRoute(RouteConstants.ModelRoutePrefixWithTrailingSlash + "element1ds")]
[BeamOsEndpointType(Http.Post)]
[BeamOsRequiredAuthorizationLevel(UserAuthorizationLevel.Contributor)]
public class CreateElement1d(CreateElement1dCommandHandler createElement1dCommandHandler)
    : BeamOsModelResourceBaseEndpoint<
        CreateElement1dCommand,
        CreateElement1dRequest,
        Element1dResponse
    >
{
    public override async Task<Result<Element1dResponse>> ExecuteRequestAsync(
        CreateElement1dCommand req,
        CancellationToken ct = default
    ) => await createElement1dCommandHandler.ExecuteAsync(req, ct);
}
