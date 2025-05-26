using BeamOs.Common.Api;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.LoadCases;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.LoadCases;
using LoadCase = BeamOs.StructuralAnalysis.Contracts.PhysicalModel.LoadCases.LoadCase;

namespace BeamOs.StructuralAnalysis.Api.Endpoints.PhysicalModel.LoadCases;

[BeamOsRoute(RouteConstants.ModelRoutePrefixWithTrailingSlash + "load-cases/{id}")]
[BeamOsEndpointType(Http.Put)]
[BeamOsRequiredAuthorizationLevel(UserAuthorizationLevel.Contributor)]
public class PutLoadCase(PutLoadCaseCommandHandler putLoadCaseCommandHandler)
    : BeamOsModelResourceWithIntIdBaseEndpoint<PutLoadCaseCommand, LoadCaseData, LoadCase>
{
    public override async Task<Result<LoadCase>> ExecuteRequestAsync(
        PutLoadCaseCommand req,
        CancellationToken ct = default
    ) => await putLoadCaseCommandHandler.ExecuteAsync(req, ct);
}
