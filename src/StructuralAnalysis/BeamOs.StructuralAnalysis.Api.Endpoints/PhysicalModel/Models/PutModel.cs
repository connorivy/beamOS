using BeamOs.Common.Api;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.Models;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Model;

namespace BeamOs.StructuralAnalysis.Api.Endpoints.PhysicalModel.Models;

[BeamOsRoute(RouteConstants.ModelRoutePrefixWithTrailingSlash)]
[BeamOsEndpointType(Http.Put)]
[BeamOsRequiredAuthorizationLevel(UserAuthorizationLevel.Contributor)]
public class PutModel(PutModelCommandHandler putModelCommandHandler)
    : BeamOsModelResourceBaseEndpoint<ModelResourceRequest<ModelData>, ModelData, ModelResponse>
{
    public override async Task<Result<ModelResponse>> ExecuteRequestAsync(
        ModelResourceRequest<ModelData> req,
        CancellationToken ct = default
    ) => await putModelCommandHandler.ExecuteAsync(req, ct);
}
