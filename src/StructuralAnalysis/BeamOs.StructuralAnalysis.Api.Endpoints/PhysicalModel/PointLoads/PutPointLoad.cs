using BeamOs.Common.Api;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.PointLoads;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Node;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.PointLoad;

namespace BeamOs.StructuralAnalysis.Api.Endpoints.PhysicalModel.PointLoads;

[BeamOsRoute(RouteConstants.ModelRoutePrefixWithTrailingSlash + "point-loads/{id}")]
[BeamOsEndpointType(Http.Put)]
[BeamOsRequiredAuthorizationLevel(UserAuthorizationLevel.Contributor)]
public class PutPointLoad(PutPointLoadCommandHandler putPointLoadCommandHandler)
    : BeamOsModelResourceWithIntIdBaseEndpoint<
        PutPointLoadCommand,
        PointLoadData,
        PointLoadResponse
    >
{
    public override async Task<Result<PointLoadResponse>> ExecuteRequestAsync(
        PutPointLoadCommand req,
        CancellationToken ct = default
    ) => await putPointLoadCommandHandler.ExecuteAsync(req, ct);
}

[BeamOsRoute(RouteConstants.ModelRoutePrefixWithTrailingSlash + "point-loads")]
[BeamOsEndpointType(Http.Put)]
[BeamOsRequiredAuthorizationLevel(UserAuthorizationLevel.Contributor)]
public class BatchPutPointLoad(BatchPutPointLoadCommandHandler putPointLoadCommandHandler)
    : BeamOsModelResourceBaseEndpoint<
        BatchPutPointLoadCommand,
        PutPointLoadRequest[],
        BatchResponse
    >
{
    public override async Task<Result<BatchResponse>> ExecuteRequestAsync(
        BatchPutPointLoadCommand req,
        CancellationToken ct = default
    ) => await putPointLoadCommandHandler.ExecuteAsync(req, ct);
}
