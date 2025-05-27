using BeamOs.Common.Api;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.MomentLoads;
using BeamOs.StructuralAnalysis.Contracts.Common;

namespace BeamOs.StructuralAnalysis.Api.Endpoints.PhysicalModel.MomentLoads;

[BeamOsRoute(RouteConstants.ModelRoutePrefixWithTrailingSlash + "moment-loads/{id}")]
[BeamOsEndpointType(Http.Delete)]
[BeamOsRequiredAuthorizationLevel(UserAuthorizationLevel.Contributor)]
public class DeleteMomentLoad(DeleteMomentLoadCommandHandler deleteMomentLoadCommandHandler)
    : BeamOsModelResourceQueryBaseEndpoint<ModelEntityResponse>
{
    public override async Task<Result<ModelEntityResponse>> ExecuteRequestAsync(
        OpenSees.ModelEntityRequest req,
        CancellationToken ct = default
    ) => await deleteMomentLoadCommandHandler.ExecuteAsync(req, ct);
}
