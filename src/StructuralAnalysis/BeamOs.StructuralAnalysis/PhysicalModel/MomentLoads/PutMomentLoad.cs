using BeamOs.Common.Api;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.MomentLoads;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.MomentLoads;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Nodes;

namespace BeamOs.StructuralAnalysis.Api.Endpoints.PhysicalModel.MomentLoads;

[BeamOsRoute(RouteConstants.ModelRoutePrefixWithTrailingSlash + "moment-loads/{id:int}")]
[BeamOsEndpointType(Http.Put)]
[BeamOsRequiredAuthorizationLevel(UserAuthorizationLevel.Contributor)]
internal class PutMomentLoad(PutMomentLoadCommandHandler putMomentLoadCommandHandler)
    : BeamOsModelResourceWithIntIdBaseEndpoint<
        PutMomentLoadCommand,
        MomentLoadData,
        MomentLoadResponse
    >
{
    public override async Task<Result<MomentLoadResponse>> ExecuteRequestAsync(
        PutMomentLoadCommand req,
        CancellationToken ct = default
    ) => await putMomentLoadCommandHandler.ExecuteAsync(req, ct);
}

[BeamOsRoute(RouteConstants.ModelRoutePrefixWithTrailingSlash + "moment-loads")]
[BeamOsEndpointType(Http.Put)]
[BeamOsRequiredAuthorizationLevel(UserAuthorizationLevel.Contributor)]
internal class BatchPutMomentLoad(BatchPutMomentLoadCommandHandler putMomentLoadCommandHandler)
    : BeamOsModelResourceBaseEndpoint<
        BatchPutMomentLoadRequest,
        PutMomentLoadRequest[],
        BatchResponse
    >
{
    public override async Task<Result<BatchResponse>> ExecuteRequestAsync(
        BatchPutMomentLoadRequest req,
        CancellationToken ct = default
    ) => await putMomentLoadCommandHandler.ExecuteAsync(req, ct);
}
