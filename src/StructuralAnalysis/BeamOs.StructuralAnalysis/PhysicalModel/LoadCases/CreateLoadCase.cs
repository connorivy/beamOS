using BeamOs.Common.Api;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.LoadCases;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.LoadCases;

namespace BeamOs.StructuralAnalysis.Api.Endpoints.PhysicalModel.LoadCases;

[BeamOsRoute(RouteConstants.ModelRoutePrefixWithTrailingSlash + "load-cases")]
[BeamOsEndpointType(Http.Post)]
[BeamOsRequiredAuthorizationLevel(UserAuthorizationLevel.Contributor)]
internal class CreateLoadCase(CreateLoadCaseCommandHandler createLoadCaseCommandHandler)
    : BeamOsModelResourceBaseEndpoint<LoadCaseData, LoadCaseContract>
{
    public override async Task<Result<LoadCaseContract>> ExecuteRequestAsync(
        ModelResourceRequest<LoadCaseData> req,
        CancellationToken ct = default
    ) => await createLoadCaseCommandHandler.ExecuteAsync(req, ct);
}
