using BeamOs.Common.Api;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.Nodes;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Nodes;

namespace BeamOs.StructuralAnalysis.Api.Endpoints.PhysicalModel.Nodes;

[BeamOsRoute(RouteConstants.ModelRoutePrefixWithTrailingSlash + "nodes/{id:int}/internal")]
[BeamOsEndpointType(Http.Get)]
[BeamOsRequiredAuthorizationLevel(UserAuthorizationLevel.Reviewer)]
public class GetInternalNode(GetInternalNodeCommandHandler getNodeCommandHandler)
    : BeamOsModelResourceQueryBaseEndpoint<InternalNode>
{
    public override async Task<Result<InternalNode>> ExecuteRequestAsync(
        ModelResourceWithIntIdRequest req,
        CancellationToken ct = default
    ) => await getNodeCommandHandler.ExecuteAsync(req, ct);
}

[BeamOsRoute(RouteConstants.ModelRoutePrefixWithTrailingSlash + "nodes/internal")]
[BeamOsEndpointType(Http.Post)]
[BeamOsRequiredAuthorizationLevel(UserAuthorizationLevel.Contributor)]
[BeamOsTag(BeamOsTags.AI)]
public class CreateInternalNode(CreateInternalNodeCommandHandler createInternalNodeCommandHandler)
    : BeamOsModelResourceBaseEndpoint<
        ModelResourceRequest<CreateInternalNodeRequest>,
        CreateInternalNodeRequest,
        InternalNode
    >
{
    public override async Task<Result<InternalNode>> ExecuteRequestAsync(
        ModelResourceRequest<CreateInternalNodeRequest> req,
        CancellationToken ct = default
    ) => await createInternalNodeCommandHandler.ExecuteAsync(req, ct);
}

[BeamOsRoute(RouteConstants.ModelRoutePrefixWithTrailingSlash + "nodes/{id:int}/internal")]
[BeamOsEndpointType(Http.Put)]
[BeamOsRequiredAuthorizationLevel(UserAuthorizationLevel.Contributor)]
public class PutInternalNode(PutInternalNodeCommandHandler putInternalNodeCommandHandler)
    : BeamOsModelResourceWithIntIdBaseEndpoint<
        ModelResourceWithIntIdRequest<InternalNodeData>,
        InternalNodeData,
        InternalNode
    >
{
    public override async Task<Result<InternalNode>> ExecuteRequestAsync(
        ModelResourceWithIntIdRequest<InternalNodeData> req,
        CancellationToken ct = default
    ) => await putInternalNodeCommandHandler.ExecuteAsync(req, ct);
}

[BeamOsRoute(RouteConstants.ModelRoutePrefixWithTrailingSlash + "nodes/internal")]
[BeamOsEndpointType(Http.Put)]
[BeamOsRequiredAuthorizationLevel(UserAuthorizationLevel.Contributor)]
public class BatchPutInternalNode(BatchPutInternalNodeCommandHandler putInternalNodeCommandHandler)
    : BeamOsModelResourceBaseEndpoint<
        ModelResourceRequest<InternalNode[]>,
        InternalNode[],
        BatchResponse
    >
{
    public override async Task<Result<BatchResponse>> ExecuteRequestAsync(
        ModelResourceRequest<InternalNode[]> req,
        CancellationToken ct = default
    ) => await putInternalNodeCommandHandler.ExecuteAsync(req, ct);
}

// [BeamOsRoute(RouteConstants.ModelRoutePrefixWithTrailingSlash + "nodes/{id:int}")]
// [BeamOsEndpointType(Http.Delete)]
// [BeamOsRequiredAuthorizationLevel(UserAuthorizationLevel.Contributor)]
// [BeamOsTag(BeamOsTags.AI)]
// public class DeleteInternalNode(DeleteInternalNodeCommandHandler deleteInternalNodeCommandHandler)
//     : BeamOsModelResourceQueryBaseEndpoint<ModelEntityResponse>
// {
//     public override async Task<Result<ModelEntityResponse>> ExecuteRequestAsync(
//         ModelEntityRequest req,
//         CancellationToken ct = default
//     ) => await deleteInternalNodeCommandHandler.ExecuteAsync(req, ct);
// }
