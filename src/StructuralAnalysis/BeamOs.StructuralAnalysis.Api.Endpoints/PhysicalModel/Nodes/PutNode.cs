using BeamOs.Common.Api;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.Nodes;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Nodes;

namespace BeamOs.StructuralAnalysis.Api.Endpoints.PhysicalModel.Nodes;

[BeamOsRoute(RouteConstants.ModelRoutePrefixWithTrailingSlash + "nodes/{id:int}")]
[BeamOsEndpointType(Http.Put)]
[BeamOsRequiredAuthorizationLevel(UserAuthorizationLevel.Contributor)]
public class PutNode(PutNodeCommandHandler putNodeCommandHandler)
    : BeamOsModelResourceBaseEndpoint<PutNodeCommand, NodeData, NodeResponse>
{
    public override async Task<Result<NodeResponse>> ExecuteRequestAsync(
        PutNodeCommand req,
        CancellationToken ct = default
    ) => await putNodeCommandHandler.ExecuteAsync(req, ct);
}

[BeamOsRoute(RouteConstants.ModelRoutePrefixWithTrailingSlash + "nodes")]
[BeamOsEndpointType(Http.Put)]
[BeamOsRequiredAuthorizationLevel(UserAuthorizationLevel.Contributor)]
public class BatchPutNode(BatchPutNodeCommandHandler putNodeCommandHandler)
    : BeamOsModelResourceBaseEndpoint<BatchPutNodeCommand, PutNodeRequest[], BatchResponse>
{
    public override async Task<Result<BatchResponse>> ExecuteRequestAsync(
        BatchPutNodeCommand req,
        CancellationToken ct = default
    ) => await putNodeCommandHandler.ExecuteAsync(req, ct);
}
