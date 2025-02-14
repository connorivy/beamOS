using BeamOs.Common.Api;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.Element1ds;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Element1d;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Node;

namespace BeamOs.StructuralAnalysis.Api.Endpoints.PhysicalModel.Element1ds;

[BeamOsRoute(RouteConstants.ModelRoutePrefixWithTrailingSlash + "element1ds/{id}")]
[BeamOsEndpointType(Http.Put)]
[BeamOsRequiredAuthorizationLevel(UserAuthorizationLevel.Contributor)]
public class PutElement1d(PutElement1dCommandHandler putElement1dCommandHandler)
    : BeamOsModelResourceWithIntIdBaseEndpoint<
        PutElement1dCommand,
        Element1dData,
        Element1dResponse
    >
{
    public override async Task<Result<Element1dResponse>> ExecuteRequestAsync(
        PutElement1dCommand req,
        CancellationToken ct = default
    ) => await putElement1dCommandHandler.ExecuteAsync(req, ct);
}

[BeamOsRoute(RouteConstants.ModelRoutePrefixWithTrailingSlash + "element1ds")]
[BeamOsEndpointType(Http.Put)]
[BeamOsRequiredAuthorizationLevel(UserAuthorizationLevel.Contributor)]
public class BatchPutElement1d(BatchPutElement1dCommandHandler putElement1dCommandHandler)
    : BeamOsModelResourceBaseEndpoint<
        BatchPutElement1dCommand,
        PutElement1dRequest[],
        BatchResponse
    >
{
    public override async Task<Result<BatchResponse>> ExecuteRequestAsync(
        BatchPutElement1dCommand req,
        CancellationToken ct = default
    ) => await putElement1dCommandHandler.ExecuteAsync(req, ct);
}
