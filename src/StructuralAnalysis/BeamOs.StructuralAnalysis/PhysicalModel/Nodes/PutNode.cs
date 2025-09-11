using BeamOs.Common.Api;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.Nodes;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Nodes;

namespace BeamOs.StructuralAnalysis.Api.Endpoints.PhysicalModel.Nodes;

[BeamOsRoute(RouteConstants.ModelRoutePrefixWithTrailingSlash + "nodes/{id:int}")]
[BeamOsEndpointType(Http.Put)]
[BeamOsRequiredAuthorizationLevel(UserAuthorizationLevel.Contributor)]
internal class PutNode(PutNodeCommandHandler putNodeCommandHandler)
    : BeamOsModelResourceWithIntIdBaseEndpoint<NodeData, NodeResponse>
{
    public override async Task<Result<NodeResponse>> ExecuteRequestAsync(
        ModelResourceWithIntIdRequest<NodeData> req,
        CancellationToken ct = default
    ) => await putNodeCommandHandler.ExecuteAsync(req, ct);
}

[BeamOsRoute(RouteConstants.ModelRoutePrefixWithTrailingSlash + "nodes")]
[BeamOsEndpointType(Http.Put)]
[BeamOsRequiredAuthorizationLevel(UserAuthorizationLevel.Contributor)]
internal class BatchPutNode(BatchPutNodeCommandHandler putNodeCommandHandler)
    : BeamOsModelResourceBaseEndpoint<PutNodeRequest[], BatchResponse>
{
    public override async Task<Result<BatchResponse>> ExecuteRequestAsync(
        ModelResourceRequest<PutNodeRequest[]> req,
        CancellationToken ct = default
    ) => await putNodeCommandHandler.ExecuteAsync(req, ct);
}
