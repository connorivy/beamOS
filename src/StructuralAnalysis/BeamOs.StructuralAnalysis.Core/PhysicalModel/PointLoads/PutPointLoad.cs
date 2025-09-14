using BeamOs.Common.Api;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.PointLoads;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Nodes;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.PointLoads;

namespace BeamOs.StructuralAnalysis.Api.Endpoints.PhysicalModel.PointLoads;

[BeamOsRoute(RouteConstants.ModelRoutePrefixWithTrailingSlash + "point-loads/{id:int}")]
[BeamOsEndpointType(Http.Put)]
[BeamOsRequiredAuthorizationLevel(UserAuthorizationLevel.Contributor)]
internal class PutPointLoad(PutPointLoadCommandHandler putPointLoadCommandHandler)
    : BeamOsModelResourceWithIntIdBaseEndpoint<PointLoadData, PointLoadResponse>
{
    public override async Task<Result<PointLoadResponse>> ExecuteRequestAsync(
        ModelResourceWithIntIdRequest<PointLoadData> req,
        CancellationToken ct = default
    ) => await putPointLoadCommandHandler.ExecuteAsync(req, ct);
}

[BeamOsRoute(RouteConstants.ModelRoutePrefixWithTrailingSlash + "point-loads")]
[BeamOsEndpointType(Http.Put)]
[BeamOsRequiredAuthorizationLevel(UserAuthorizationLevel.Contributor)]
internal class BatchPutPointLoad(BatchPutPointLoadCommandHandler putPointLoadCommandHandler)
    : BeamOsModelResourceBaseEndpoint<PutPointLoadRequest[], BatchResponse>
{
    public override async Task<Result<BatchResponse>> ExecuteRequestAsync(
        ModelResourceRequest<PutPointLoadRequest[]> req,
        CancellationToken ct = default
    ) => await putPointLoadCommandHandler.ExecuteAsync(req, ct);
}
