using BeamOs.Common.Api;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.SectionProfiles;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Nodes;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.SectionProfiles;

namespace BeamOs.StructuralAnalysis.Api.Endpoints.PhysicalModel.SectionProfiles;

[BeamOsRoute(
    RouteConstants.ModelRoutePrefixWithTrailingSlash + "section-profiles/{id:int}/from-library"
)]
[BeamOsEndpointType(Http.Put)]
[BeamOsRequiredAuthorizationLevel(UserAuthorizationLevel.Contributor)]
internal class PutSectionProfileFromLibrary(
    PutSectionProfileFromLibraryCommandHandler putSectionProfileCommandHandler
)
    : BeamOsModelResourceWithIntIdBaseEndpoint<
        SectionProfileFromLibraryData,
        SectionProfileFromLibraryContract
    >
{
    public override async Task<Result<SectionProfileFromLibraryContract>> ExecuteRequestAsync(
        ModelResourceWithIntIdRequest<SectionProfileFromLibraryData> req,
        CancellationToken ct = default
    ) => await putSectionProfileCommandHandler.ExecuteAsync(req, ct);
}

[BeamOsRoute(RouteConstants.ModelRoutePrefixWithTrailingSlash + "section-profiles/from-library")]
[BeamOsEndpointType(Http.Put)]
[BeamOsRequiredAuthorizationLevel(UserAuthorizationLevel.Contributor)]
internal class BatchPutSectionProfileFromLibrary(
    BatchPutSectionProfileFromLibraryCommandHandler putSectionProfileFromLibraryCommandHandler
) : BeamOsModelResourceBaseEndpoint<SectionProfileFromLibraryContract[], BatchResponse>
{
    public override async Task<Result<BatchResponse>> ExecuteRequestAsync(
        ModelResourceRequest<SectionProfileFromLibraryContract[]> req,
        CancellationToken ct = default
    ) => await putSectionProfileFromLibraryCommandHandler.ExecuteAsync(req, ct);
}
