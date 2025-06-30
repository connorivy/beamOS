using BeamOs.Common.Api;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.Materials;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Materials;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Nodes;

namespace BeamOs.StructuralAnalysis.Api.Endpoints.PhysicalModel.Materials;

[BeamOsRoute(RouteConstants.ModelRoutePrefixWithTrailingSlash + "materials/{id}")]
[BeamOsEndpointType(Http.Put)]
[BeamOsRequiredAuthorizationLevel(UserAuthorizationLevel.Contributor)]
public class PutMaterial(PutMaterialCommandHandler putMaterialCommandHandler)
    : BeamOsModelResourceWithIntIdBaseEndpoint<PutMaterialCommand, MaterialData, MaterialResponse>
{
    public override async Task<Result<MaterialResponse>> ExecuteRequestAsync(
        PutMaterialCommand req,
        CancellationToken ct = default
    ) => await putMaterialCommandHandler.ExecuteAsync(req, ct);
}

[BeamOsRoute(RouteConstants.ModelRoutePrefixWithTrailingSlash + "materials")]
[BeamOsEndpointType(Http.Put)]
[BeamOsRequiredAuthorizationLevel(UserAuthorizationLevel.Contributor)]
public class BatchPutMaterial(BatchPutMaterialCommandHandler putMaterialCommandHandler)
    : BeamOsModelResourceBaseEndpoint<BatchPutMaterialCommand, PutMaterialRequest[], BatchResponse>
{
    public override async Task<Result<BatchResponse>> ExecuteRequestAsync(
        BatchPutMaterialCommand req,
        CancellationToken ct = default
    ) => await putMaterialCommandHandler.ExecuteAsync(req, ct);
}
