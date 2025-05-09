using BeamOs.Common.Api;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Application.OpenSees;
using BeamOs.StructuralAnalysis.Contracts.AnalyticalResults.Diagrams;

namespace BeamOs.StructuralAnalysis.Api.Endpoints.OpenSees;

[BeamOsRoute(RouteConstants.ModelRoutePrefixWithTrailingSlash + "analyze/optimize-sections")]
[BeamOsEndpointType(Http.Post)]
[BeamOsRequiredAuthorizationLevel(UserAuthorizationLevel.Contributor)]
public class OptimizeSections()
    : BeamOsBaseEndpoint<ModelIdRequest, AnalyticalResultsResponse>
{
    public override async Task<Result<AnalyticalResultsResponse>> ExecuteRequestAsync(
        ModelIdRequest req,
        CancellationToken ct = default
    ) =>
        await runOpenSeesCommandHandler.ExecuteAsync(
            new()
            {
                ModelId = req.ModelId,
                UnitsOverride = req.Body.UnitsOverride,
                LoadCombinationIds = req.Body.LoadCombinationIds,
            },
            ct
        );
}
