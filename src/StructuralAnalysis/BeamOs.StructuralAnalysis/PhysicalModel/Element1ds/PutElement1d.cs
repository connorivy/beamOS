using BeamOs.Common.Api;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.Element1ds;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Element1ds;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Nodes;

namespace BeamOs.StructuralAnalysis.Api.Endpoints.PhysicalModel.Element1ds;

[BeamOsRoute(RouteConstants.ModelRoutePrefixWithTrailingSlash + "element1ds/{id:int}")]
[BeamOsEndpointType(Http.Put)]
[BeamOsRequiredAuthorizationLevel(UserAuthorizationLevel.Contributor)]
internal class PutElement1d(PutElement1dCommandHandler putElement1dCommandHandler)
    : BeamOsModelResourceWithIntIdBaseEndpoint<Element1dData, Element1dResponse>
{
    public override async Task<Result<Element1dResponse>> ExecuteRequestAsync(
        ModelResourceWithIntIdRequest<Element1dData> req,
        CancellationToken ct = default
    ) => await putElement1dCommandHandler.ExecuteAsync(req, ct);
}

[BeamOsRoute(RouteConstants.ModelRoutePrefixWithTrailingSlash + "element1ds")]
[BeamOsEndpointType(Http.Put)]
[BeamOsRequiredAuthorizationLevel(UserAuthorizationLevel.Contributor)]
internal class BatchPutElement1d(BatchPutElement1dCommandHandler putElement1dCommandHandler)
    : BeamOsModelResourceBaseEndpoint<PutElement1dRequest[], BatchResponse>
{
    public override async Task<Result<BatchResponse>> ExecuteRequestAsync(
        ModelResourceRequest<PutElement1dRequest[]> req,
        CancellationToken ct = default
    ) => await putElement1dCommandHandler.ExecuteAsync(req, ct);
}
