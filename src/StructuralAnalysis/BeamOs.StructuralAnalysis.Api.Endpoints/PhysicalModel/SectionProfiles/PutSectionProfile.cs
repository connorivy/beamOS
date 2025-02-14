using BeamOs.Common.Api;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.SectionProfiles;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Node;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.SectionProfile;

namespace BeamOs.StructuralAnalysis.Api.Endpoints.PhysicalModel.SectionProfiles;

[BeamOsRoute(RouteConstants.ModelRoutePrefixWithTrailingSlash + "section-profiles/{id}")]
[BeamOsEndpointType(Http.Put)]
[BeamOsRequiredAuthorizationLevel(UserAuthorizationLevel.Contributor)]
public class PutSectionProfile(PutSectionProfileCommandHandler putSectionProfileCommandHandler)
    : BeamOsModelResourceWithIntIdBaseEndpoint<
        PutSectionProfileCommand,
        SectionProfileRequestData,
        SectionProfileResponse
    >
{
    public override async Task<Result<SectionProfileResponse>> ExecuteRequestAsync(
        PutSectionProfileCommand req,
        CancellationToken ct = default
    ) => await putSectionProfileCommandHandler.ExecuteAsync(req, ct);
}

[BeamOsRoute(RouteConstants.ModelRoutePrefixWithTrailingSlash + "section-profiles")]
[BeamOsEndpointType(Http.Put)]
[BeamOsRequiredAuthorizationLevel(UserAuthorizationLevel.Contributor)]
public class BatchPutSectionProfile(
    BatchPutSectionProfileCommandHandler putSectionProfileCommandHandler
)
    : BeamOsModelResourceBaseEndpoint<
        BatchPutSectionProfileCommand,
        PutSectionProfileRequest[],
        BatchResponse
    >
{
    public override async Task<Result<BatchResponse>> ExecuteRequestAsync(
        BatchPutSectionProfileCommand req,
        CancellationToken ct = default
    ) => await putSectionProfileCommandHandler.ExecuteAsync(req, ct);
}
