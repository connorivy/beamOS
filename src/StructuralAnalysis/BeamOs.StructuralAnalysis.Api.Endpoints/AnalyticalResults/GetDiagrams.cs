using BeamOs.Common.Api;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Api.Endpoints.OpenSees;
using BeamOs.StructuralAnalysis.Application.AnalyticalResults;
using BeamOs.StructuralAnalysis.Contracts.AnalyticalResults.Diagrams;

namespace BeamOs.StructuralAnalysis.Api.Endpoints.AnalyticalResults;

[BeamOsRoute(RouteConstants.ModelRoutePrefixWithTrailingSlash + "result-sets/{id}/diagrams")]
[BeamOsEndpointType(Http.Get)]
[BeamOsRequiredAuthorizationLevel(UserAuthorizationLevel.Reviewer)]
public class GetDiagrams(GetDiagramsCommandHandler getDiagramsCommandHandler)
    : BeamOsModelResourceQueryBaseEndpoint<AnalyticalResultsResponse>
{
    public override async Task<Result<AnalyticalResultsResponse>> ExecuteRequestAsync(
        ModelEntityRequest req,
        CancellationToken ct = default
    ) =>
        await getDiagramsCommandHandler.ExecuteAsync(
            new() { Id = req.Id, ModelId = req.ModelId },
            ct
        );
}
