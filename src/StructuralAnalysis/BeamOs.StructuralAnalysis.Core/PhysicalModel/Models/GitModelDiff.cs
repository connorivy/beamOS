using BeamOs.Common.Api;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.Models;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Models;

namespace BeamOs.StructuralAnalysis.Api.Endpoints.PhysicalModel.Models;

[BeamOsRoute(RouteConstants.ModelRoutePrefixWithoutTrailingSlash + "/diff")]
[BeamOsEndpointType(Http.Post)]
[BeamOsRequiredAuthorizationLevel(UserAuthorizationLevel.Reviewer)]
[BeamOsTag(BeamOsTags.AI)]
internal class GetModelDiff(GetModelDiffCommandHandler getModelDiffCommandHandler)
    : BeamOsModelResourceBaseEndpoint<DiffModelRequest, ModelDiffData>
{
    public override async Task<Result<ModelDiffData>> ExecuteRequestAsync(
        ModelResourceRequest<DiffModelRequest> req,
        CancellationToken ct = default
    ) => await getModelDiffCommandHandler.ExecuteAsync(req, ct);
}
