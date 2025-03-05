using BeamOs.Common.Api;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Api.Endpoints.OpenSees;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Model;
using BeamOs.StructuralAnalysis.Infrastructure.PhysicalModel.Models;

namespace BeamOs.StructuralAnalysis.Api.Endpoints.PhysicalModel.Models;

[BeamOsRoute(RouteConstants.ModelRoutePrefixWithTrailingSlash)]
[BeamOsEndpointType(Http.Get)]
[BeamOsRequiredAuthorizationLevel(UserAuthorizationLevel.Reviewer)]
public class GetModel(GetModelQueryHandler getModelCommandHandler)
    : BeamOsModelIdRequestBaseEndpoint<ModelResponse>
{
    public override async Task<Result<ModelResponse>> ExecuteRequestAsync(
        ModelIdRequest req,
        CancellationToken ct = default
    ) => await getModelCommandHandler.ExecuteAsync(req.ModelId, ct);
}
