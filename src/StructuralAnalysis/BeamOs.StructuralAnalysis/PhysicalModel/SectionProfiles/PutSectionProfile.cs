using BeamOs.Common.Api;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.SectionProfiles;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Nodes;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.SectionProfiles;

namespace BeamOs.StructuralAnalysis.Api.Endpoints.PhysicalModel.SectionProfiles;

[BeamOsRoute(RouteConstants.ModelRoutePrefixWithTrailingSlash + "section-profiles/{id:int}")]
[BeamOsEndpointType(Http.Put)]
[BeamOsRequiredAuthorizationLevel(UserAuthorizationLevel.Contributor)]
internal class PutSectionProfile(PutSectionProfileCommandHandler putSectionProfileCommandHandler)
    : BeamOsModelResourceWithIntIdBaseEndpoint<SectionProfileData, SectionProfileResponse>
{
    public override async Task<Result<SectionProfileResponse>> ExecuteRequestAsync(
        ModelResourceWithIntIdRequest<SectionProfileData> req,
        CancellationToken ct = default
    ) => await putSectionProfileCommandHandler.ExecuteAsync(req, ct);
}

[BeamOsRoute(RouteConstants.ModelRoutePrefixWithTrailingSlash + "section-profiles")]
[BeamOsEndpointType(Http.Put)]
[BeamOsRequiredAuthorizationLevel(UserAuthorizationLevel.Contributor)]
internal class BatchPutSectionProfile(
    BatchPutSectionProfileCommandHandler putSectionProfileCommandHandler
) : BeamOsModelResourceBaseEndpoint<PutSectionProfileRequest[], BatchResponse>
{
    public override async Task<Result<BatchResponse>> ExecuteRequestAsync(
        ModelResourceRequest<PutSectionProfileRequest[]> req,
        CancellationToken ct = default
    ) => await putSectionProfileCommandHandler.ExecuteAsync(req, ct);
}
