using BeamOs.Common.Api;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.LoadCases;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.LoadCases;

namespace BeamOs.StructuralAnalysis.Api.Endpoints.PhysicalModel.LoadCases;

[BeamOsRoute(RouteConstants.ModelRoutePrefixWithTrailingSlash + "load-cases/{id}")]
[BeamOsEndpointType(Http.Get)]
[BeamOsRequiredAuthorizationLevel(UserAuthorizationLevel.Reviewer)]
public class GetLoadCase(GetLoadCaseCommandHandler getLoadCaseCommandHandler)
    : BeamOsModelResourceQueryBaseEndpoint<LoadCase>
{
    public override async Task<Result<LoadCase>> ExecuteRequestAsync(
        ModelResourceWithIntIdRequest req,
        CancellationToken ct = default
    ) => await getLoadCaseCommandHandler.ExecuteAsync(req, ct);
}
