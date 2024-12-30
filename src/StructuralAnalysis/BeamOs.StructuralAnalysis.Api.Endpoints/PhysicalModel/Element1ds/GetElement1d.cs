using BeamOs.Common.Api;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.Element1ds;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Element1d;
using BeamOs.StructuralAnalysis.Infrastructure.PhysicalModel.Element1ds;

namespace BeamOs.StructuralAnalysis.Api.Endpoints.PhysicalModel.Element1ds;

[BeamOsRoute(RouteConstants.ModelRoutePrefixWithTrailingSlash + "element1ds/{id}")]
[BeamOsEndpointType(Http.Get)]
[BeamOsRequiredAuthorizationLevel(UserAuthorizationLevel.Reviewer)]
public class GetElement1d(GetElement1dQueryHandler getElement1dCommandHandler)
    : BeamOsBaseEndpoint<GetModelResourceQuery, Element1dResponse>
{
    public override async Task<Result<Element1dResponse>> ExecuteRequestAsync(
        GetModelResourceQuery req,
        CancellationToken ct = default
    ) => await getElement1dCommandHandler.ExecuteAsync(req, ct);
}
