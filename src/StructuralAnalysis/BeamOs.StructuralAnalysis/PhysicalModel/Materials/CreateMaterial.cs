using BeamOs.Common.Api;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.Materials;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Materials;

namespace BeamOs.StructuralAnalysis.Api.Endpoints.PhysicalModel.Materials;

[BeamOsRoute(RouteConstants.ModelRoutePrefixWithTrailingSlash + "materials")]
[BeamOsEndpointType(Http.Post)]
[BeamOsRequiredAuthorizationLevel(UserAuthorizationLevel.Contributor)]
internal class CreateMaterial(CreateMaterialCommandHandler createMaterialCommandHandler)
    : BeamOsModelResourceBaseEndpoint<CreateMaterialRequest, MaterialResponse>
{
    public override async Task<Result<MaterialResponse>> ExecuteRequestAsync(
        ModelResourceRequest<CreateMaterialRequest> req,
        CancellationToken ct = default
    ) => await createMaterialCommandHandler.ExecuteAsync(req, ct);
}
