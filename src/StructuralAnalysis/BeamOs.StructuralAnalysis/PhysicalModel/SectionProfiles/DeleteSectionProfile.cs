using BeamOs.Common.Api;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.SectionProfiles;
using BeamOs.StructuralAnalysis.Contracts.Common;

namespace BeamOs.StructuralAnalysis.Api.Endpoints.PhysicalModel.SectionProfiles;

[BeamOsRoute(RouteConstants.ModelRoutePrefixWithTrailingSlash + "section-profiles/{id:int}")]
[BeamOsEndpointType(Http.Delete)]
[BeamOsRequiredAuthorizationLevel(UserAuthorizationLevel.Contributor)]
internal class DeleteSectionProfile(
    DeleteSectionProfileCommandHandler deleteSectionProfileCommandHandler
) : BeamOsModelResourceQueryBaseEndpoint<ModelEntityResponse>
{
    public override async Task<Result<ModelEntityResponse>> ExecuteRequestAsync(
        ModelResourceWithIntIdRequest req,
        CancellationToken ct = default
    ) => await deleteSectionProfileCommandHandler.ExecuteAsync(req, ct);
}
