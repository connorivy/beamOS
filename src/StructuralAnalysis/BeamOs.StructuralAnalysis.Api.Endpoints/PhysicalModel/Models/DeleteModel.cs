using BeamOs.Common.Api;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.Models;

namespace BeamOs.StructuralAnalysis.Api.Endpoints.PhysicalModel.Models;

[BeamOsRoute(RouteConstants.ModelRoutePrefixWithTrailingSlash)]
[BeamOsEndpointType(Http.Delete)]
[BeamOsRequiredAuthorizationLevel(UserAuthorizationLevel.Owner)]
public class DeleteModel(DeleteModelCommandHandler deleteModelCommandHandler)
    : BeamOsModelIdRequestBaseEndpoint<bool>
{
    public override async Task<Result<bool>> ExecuteRequestAsync(
        ModelResourceRequest req,
        CancellationToken ct = default
    ) => await deleteModelCommandHandler.ExecuteAsync(req.ModelId, ct);
}
