using BeamOs.Common.Api;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.SectionProfiles;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.SectionProfiles;

namespace BeamOs.StructuralAnalysis.Api.Endpoints.PhysicalModel.SectionProfiles;

[BeamOsRoute(RouteConstants.ModelRoutePrefixWithTrailingSlash + "section-profiles/from-library")]
[BeamOsEndpointType(Http.Post)]
[BeamOsRequiredAuthorizationLevel(UserAuthorizationLevel.Contributor)]
internal class AddSectionProfileFromLibrary(
    AddSectionProfileFromLibraryCommandHandler createSectionProfileFromLibraryCommandHandler
)
    : BeamOsModelResourceBaseEndpoint<
        ModelResourceRequest<SectionProfileFromLibraryData>,
        SectionProfileFromLibraryData,
        SectionProfileResponse
    >
{
    public override async Task<Result<SectionProfileResponse>> ExecuteRequestAsync(
        ModelResourceRequest<SectionProfileFromLibraryData> req,
        CancellationToken ct = default
    ) => await createSectionProfileFromLibraryCommandHandler.ExecuteAsync(req, ct);
}
