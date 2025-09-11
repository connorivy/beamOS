using BeamOs.Common.Api;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.SectionProfiles;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.SectionProfiles;

namespace BeamOs.StructuralAnalysis.Api.Endpoints.PhysicalModel.SectionProfiles;

[BeamOsRoute(RouteConstants.ModelRoutePrefixWithTrailingSlash + "section-profiles")]
[BeamOsEndpointType(Http.Post)]
[BeamOsRequiredAuthorizationLevel(UserAuthorizationLevel.Contributor)]
internal class CreateSectionProfile(
    CreateSectionProfileCommandHandler createSectionProfileCommandHandler
)
    : BeamOsModelResourceBaseEndpoint<
        CreateSectionProfileCommand,
        CreateSectionProfileRequest,
        SectionProfileResponse
    >
{
    public override async Task<Result<SectionProfileResponse>> ExecuteRequestAsync(
        CreateSectionProfileCommand req,
        CancellationToken ct = default
    ) => await createSectionProfileCommandHandler.ExecuteAsync(req, ct);
}
