using BeamOs.Common.Api;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.Materials;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Material;

namespace BeamOs.StructuralAnalysis.Api.Endpoints.PhysicalModel.Materials;

[BeamOsRoute(RouteConstants.ModelRoutePrefixWithTrailingSlash + "materials")]
[BeamOsEndpointType(Http.Post)]
[BeamOsRequiredAuthorizationLevel(UserAuthorizationLevel.Contributor)]
public class CreateMaterial(CreateMaterialCommandHandler createMaterialCommandHandler)
    : BeamOsModelResourceBaseEndpoint<
        CreateMaterialCommand,
        CreateMaterialRequest,
        MaterialResponse
    >
{
    public override async Task<Result<MaterialResponse>> ExecuteRequestAsync(
        CreateMaterialCommand req,
        CancellationToken ct = default
    ) => await createMaterialCommandHandler.ExecuteAsync(req, ct);
}
