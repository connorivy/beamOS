using BeamOs.Common.Api;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.Models;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Models;

namespace BeamOs.StructuralAnalysis.Api.Endpoints.PhysicalModel.Models;

[BeamOsRoute(RouteConstants.ModelRoutePrefixWithoutTrailingSlash + "/diff")]
[BeamOsEndpointType(Http.Post)]
[BeamOsRequiredAuthorizationLevel(UserAuthorizationLevel.Reviewer)]
[BeamOsTag(BeamOsTags.AI)]
internal class GitModelDiff(GetModelDiffCommandHandler getModelDiffCommandHandler)
    : BeamOsModelResourceBaseEndpoint<DiffModelRequest, ModelDiffResponse>
{
    public override async Task<Result<ModelDiffResponse>> ExecuteRequestAsync(
        ModelResourceRequest<DiffModelRequest> req,
        CancellationToken ct = default
    ) => await getModelDiffCommandHandler.ExecuteAsync(req, ct);
}
