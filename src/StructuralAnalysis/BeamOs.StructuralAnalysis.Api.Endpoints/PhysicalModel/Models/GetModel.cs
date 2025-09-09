using BeamOs.Common.Api;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.Models;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Models;

namespace BeamOs.StructuralAnalysis.Api.Endpoints.PhysicalModel.Models;

[BeamOsRoute(RouteConstants.ModelRoutePrefixWithTrailingSlash)]
[BeamOsEndpointType(Http.Get)]
[BeamOsRequiredAuthorizationLevel(UserAuthorizationLevel.Reviewer)]
[BeamOsTag(BeamOsTags.AI)]
public class GetModel(GetModelCommandHandler getModelCommandHandler)
    : BeamOsModelIdRequestBaseEndpoint<ModelResponse>
{
    public override async Task<Result<ModelResponse>> ExecuteRequestAsync(
        ModelResourceRequest req,
        CancellationToken ct = default
    ) => await getModelCommandHandler.ExecuteAsync(req.ModelId, ct);
}
