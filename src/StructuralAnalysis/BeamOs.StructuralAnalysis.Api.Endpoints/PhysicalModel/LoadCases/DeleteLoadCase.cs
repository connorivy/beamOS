using BeamOs.Common.Api;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.LoadCases;
using BeamOs.StructuralAnalysis.Contracts.Common;

namespace BeamOs.StructuralAnalysis.Api.Endpoints.PhysicalModel.LoadCases;

[BeamOsRoute(RouteConstants.ModelRoutePrefixWithTrailingSlash + "load-cases/{id}")]
[BeamOsEndpointType(Http.Delete)]
[BeamOsRequiredAuthorizationLevel(UserAuthorizationLevel.Contributor)]
public class DeleteLoadCase(DeleteLoadCaseCommandHandler deleteLoadCaseCommandHandler)
    : BeamOsModelResourceQueryBaseEndpoint<ModelEntityResponse>
{
    public override async Task<Result<ModelEntityResponse>> ExecuteRequestAsync(
        ModelResourceWithIntIdRequest req,
        CancellationToken ct = default
    ) => await deleteLoadCaseCommandHandler.ExecuteAsync(req, ct);
}
