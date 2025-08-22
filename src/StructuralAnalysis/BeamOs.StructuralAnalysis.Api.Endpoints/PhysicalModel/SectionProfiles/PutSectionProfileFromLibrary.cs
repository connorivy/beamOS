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
public class PutSectionProfileFromLibrary(
    PutSectionProfileFromLibraryCommandHandler putSectionProfileCommandHandler
)
    : BeamOsModelResourceWithIntIdBaseEndpoint<
        PutSectionProfileFromLibraryCommand,
        SectionProfileFromLibraryData,
        SectionProfileFromLibrary
    >
{
    public override async Task<Result<SectionProfileFromLibrary>> ExecuteRequestAsync(
        PutSectionProfileFromLibraryCommand req,
        CancellationToken ct = default
    ) => await putSectionProfileCommandHandler.ExecuteAsync(req, ct);
}

[BeamOsRoute(RouteConstants.ModelRoutePrefixWithTrailingSlash + "section-profiles/from-library")]
[BeamOsEndpointType(Http.Put)]
[BeamOsRequiredAuthorizationLevel(UserAuthorizationLevel.Contributor)]
public class BatchPutSectionProfileFromLibrary(
    BatchPutSectionProfileFromLibraryCommandHandler putSectionProfileFromLibraryCommandHandler
)
    : BeamOsModelResourceBaseEndpoint<
        BatchPutSectionProfileFromLibraryCommand,
        SectionProfileFromLibrary[],
        BatchResponse
    >
{
    public override async Task<Result<BatchResponse>> ExecuteRequestAsync(
        BatchPutSectionProfileFromLibraryCommand req,
        CancellationToken ct = default
    ) => await putSectionProfileFromLibraryCommandHandler.ExecuteAsync(req, ct);
}
