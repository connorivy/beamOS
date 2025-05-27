using BeamOs.Common.Api;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.LoadCases;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.LoadCases;

namespace BeamOs.StructuralAnalysis.Api.Endpoints.PhysicalModel.LoadCases;

[BeamOsRoute(RouteConstants.ModelRoutePrefixWithTrailingSlash + "load-cases")]
[BeamOsEndpointType(Http.Post)]
[BeamOsRequiredAuthorizationLevel(UserAuthorizationLevel.Contributor)]
public class CreateLoadCase(CreateLoadCaseCommandHandler createLoadCaseCommandHandler)
    : BeamOsModelResourceBaseEndpoint<CreateLoadCaseCommand, LoadCaseData, LoadCase>
{
    public override async Task<Result<LoadCase>> ExecuteRequestAsync(
        CreateLoadCaseCommand req,
        CancellationToken ct = default
    ) => await createLoadCaseCommandHandler.ExecuteAsync(req, ct);
}
