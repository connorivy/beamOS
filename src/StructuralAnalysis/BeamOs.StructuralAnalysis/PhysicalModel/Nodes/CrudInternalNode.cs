using BeamOs.Common.Api;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.Nodes;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Nodes;

namespace BeamOs.StructuralAnalysis.Api.Endpoints.PhysicalModel.Nodes;

[BeamOsRoute(RouteConstants.ModelRoutePrefixWithTrailingSlash + "nodes/{id:int}/internal")]
[BeamOsEndpointType(Http.Get)]
[BeamOsRequiredAuthorizationLevel(UserAuthorizationLevel.Reviewer)]
internal class GetInternalNode(GetInternalNodeCommandHandler getNodeCommandHandler)
    : BeamOsModelResourceQueryBaseEndpoint<InternalNodeContract>
{
    public override async Task<Result<InternalNodeContract>> ExecuteRequestAsync(
        ModelResourceWithIntIdRequest req,
        CancellationToken ct = default
    ) => await getNodeCommandHandler.ExecuteAsync(req, ct);
}

[BeamOsRoute(RouteConstants.ModelRoutePrefixWithTrailingSlash + "nodes/internal")]
[BeamOsEndpointType(Http.Post)]
[BeamOsRequiredAuthorizationLevel(UserAuthorizationLevel.Contributor)]
[BeamOsTag(BeamOsTags.AI)]
internal class CreateInternalNode(CreateInternalNodeCommandHandler createInternalNodeCommandHandler)
    : BeamOsModelResourceBaseEndpoint<CreateInternalNodeRequest, InternalNodeContract>
{
    public override async Task<Result<InternalNodeContract>> ExecuteRequestAsync(
        ModelResourceRequest<CreateInternalNodeRequest> req,
        CancellationToken ct = default
    ) => await createInternalNodeCommandHandler.ExecuteAsync(req, ct);
}

[BeamOsRoute(RouteConstants.ModelRoutePrefixWithTrailingSlash + "nodes/{id:int}/internal")]
[BeamOsEndpointType(Http.Put)]
[BeamOsRequiredAuthorizationLevel(UserAuthorizationLevel.Contributor)]
internal class PutInternalNode(PutInternalNodeCommandHandler putInternalNodeCommandHandler)
    : BeamOsModelResourceWithIntIdBaseEndpoint<InternalNodeData, InternalNodeContract>
{
    public override async Task<Result<InternalNodeContract>> ExecuteRequestAsync(
        ModelResourceWithIntIdRequest<InternalNodeData> req,
        CancellationToken ct = default
    ) => await putInternalNodeCommandHandler.ExecuteAsync(req, ct);
}

[BeamOsRoute(RouteConstants.ModelRoutePrefixWithTrailingSlash + "nodes/internal")]
[BeamOsEndpointType(Http.Put)]
[BeamOsRequiredAuthorizationLevel(UserAuthorizationLevel.Contributor)]
internal class BatchPutInternalNode(
    BatchPutInternalNodeCommandHandler putInternalNodeCommandHandler
) : BeamOsModelResourceBaseEndpoint<InternalNodeContract[], BatchResponse>
{
    public override async Task<Result<BatchResponse>> ExecuteRequestAsync(
        ModelResourceRequest<InternalNodeContract[]> req,
        CancellationToken ct = default
    ) => await putInternalNodeCommandHandler.ExecuteAsync(req, ct);
}

// [BeamOsRoute(RouteConstants.ModelRoutePrefixWithTrailingSlash + "nodes/{id:int}")]
// [BeamOsEndpointType(Http.Delete)]
// [BeamOsRequiredAuthorizationLevel(UserAuthorizationLevel.Contributor)]
// [BeamOsTag(BeamOsTags.AI)]
// internal class DeleteInternalNode(DeleteInternalNodeCommandHandler deleteInternalNodeCommandHandler)
//     : BeamOsModelResourceQueryBaseEndpoint<ModelEntityResponse>
// {
//     public override async Task<Result<ModelEntityResponse>> ExecuteRequestAsync(
//         ModelEntityRequest req,
//         CancellationToken ct = default
//     ) => await deleteInternalNodeCommandHandler.ExecuteAsync(req, ct);
// }
