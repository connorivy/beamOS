using BeamOs.Common.Api;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.Materials;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Materials;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Nodes;

namespace BeamOs.StructuralAnalysis.Api.Endpoints.PhysicalModel.Materials;

[BeamOsRoute(RouteConstants.ModelRoutePrefixWithTrailingSlash + "materials/{id:int}")]
[BeamOsEndpointType(Http.Put)]
[BeamOsRequiredAuthorizationLevel(UserAuthorizationLevel.Contributor)]
internal class PutMaterial(PutMaterialCommandHandler putMaterialCommandHandler)
    : BeamOsModelResourceWithIntIdBaseEndpoint<MaterialData, MaterialResponse>
{
    public override async Task<Result<MaterialResponse>> ExecuteRequestAsync(
        ModelResourceWithIntIdRequest<MaterialData> req,
        CancellationToken ct = default
    ) => await putMaterialCommandHandler.ExecuteAsync(req, ct);
}

[BeamOsRoute(RouteConstants.ModelRoutePrefixWithTrailingSlash + "materials")]
[BeamOsEndpointType(Http.Put)]
[BeamOsRequiredAuthorizationLevel(UserAuthorizationLevel.Contributor)]
internal class BatchPutMaterial(BatchPutMaterialCommandHandler putMaterialCommandHandler)
    : BeamOsModelResourceBaseEndpoint<PutMaterialRequest[], BatchResponse>
{
    public override async Task<Result<BatchResponse>> ExecuteRequestAsync(
        ModelResourceRequest<PutMaterialRequest[]> req,
        CancellationToken ct = default
    ) => await putMaterialCommandHandler.ExecuteAsync(req, ct);
}
