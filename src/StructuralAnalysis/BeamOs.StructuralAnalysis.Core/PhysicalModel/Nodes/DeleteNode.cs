using BeamOs.Common.Api;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.Nodes;
using BeamOs.StructuralAnalysis.Contracts.Common;

namespace BeamOs.StructuralAnalysis.Api.Endpoints.PhysicalModel.Nodes;

[BeamOsRoute(RouteConstants.ModelRoutePrefixWithTrailingSlash + "nodes/{id:int}")]
[BeamOsEndpointType(Http.Delete)]
[BeamOsRequiredAuthorizationLevel(UserAuthorizationLevel.Contributor)]
[BeamOsTag(BeamOsTags.AI)]
internal class DeleteNode(DeleteNodeCommandHandler deleteNodeCommandHandler)
    : BeamOsModelResourceQueryBaseEndpoint<ModelEntityResponse>
{
    public override async Task<Result<ModelEntityResponse>> ExecuteRequestAsync(
        ModelResourceWithIntIdRequest req,
        CancellationToken ct = default
    ) => await deleteNodeCommandHandler.ExecuteAsync(req, ct);
}
