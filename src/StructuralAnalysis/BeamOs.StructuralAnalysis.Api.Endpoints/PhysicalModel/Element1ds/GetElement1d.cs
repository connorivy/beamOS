using BeamOs.Common.Api;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Api.Endpoints.OpenSees;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.Element1ds;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Element1ds;

namespace BeamOs.StructuralAnalysis.Api.Endpoints.PhysicalModel.Element1ds;

[BeamOsRoute(RouteConstants.ModelRoutePrefixWithTrailingSlash + "element1ds/{id}")]
[BeamOsEndpointType(Http.Get)]
[BeamOsRequiredAuthorizationLevel(UserAuthorizationLevel.Reviewer)]
public class GetElement1d(GetElement1dCommandHandler getElement1dCommandHandler)
    : BeamOsModelResourceQueryBaseEndpoint<Element1dResponse>
{
    public override async Task<Result<Element1dResponse>> ExecuteRequestAsync(
        ModelEntityRequest req,
        CancellationToken ct = default
    ) => await getElement1dCommandHandler.ExecuteAsync(req, ct);
}
