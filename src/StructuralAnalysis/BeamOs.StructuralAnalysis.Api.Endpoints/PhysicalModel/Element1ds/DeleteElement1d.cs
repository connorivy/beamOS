using BeamOs.Common.Api;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Api.Endpoints.OpenSees;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.Element1ds;
using BeamOs.StructuralAnalysis.Contracts.Common;

namespace BeamOs.StructuralAnalysis.Api.Endpoints.PhysicalModel.Element1ds;

[BeamOsRoute(RouteConstants.ModelRoutePrefixWithTrailingSlash + "element1ds/{id}")]
[BeamOsEndpointType(Http.Delete)]
[BeamOsRequiredAuthorizationLevel(UserAuthorizationLevel.Contributor)]
public class DeleteElement1d(DeleteElement1dCommandHandler deleteElement1dCommandHandler)
    : BeamOsModelResourceQueryBaseEndpoint<ModelEntityResponse>
{
    public override async Task<Result<ModelEntityResponse>> ExecuteRequestAsync(
        ModelEntityRequest req,
        CancellationToken ct = default
    ) => await deleteElement1dCommandHandler.ExecuteAsync(req, ct);
}
