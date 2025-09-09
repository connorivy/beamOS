using BeamOs.Common.Api;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Application.OpenSees;
using BeamOs.StructuralAnalysis.Contracts.AnalyticalResults.Diagrams;
using BeamOs.StructuralAnalysis.Contracts.Common;

namespace BeamOs.StructuralAnalysis.Api.Endpoints.OpenSees;

[BeamOsRoute(RouteConstants.ModelRoutePrefixWithTrailingSlash + "analyze/opensees")]
[BeamOsEndpointType(Http.Post)]
[BeamOsRequiredAuthorizationLevel(UserAuthorizationLevel.Contributor)]
public class RunOpenSeesAnalysis(RunOpenSeesAnalysisCommandHandler runOpenSeesCommandHandler)
    : BeamOsModelResourceBaseEndpoint<
        ModelResourceRequest<RunDsmRequest>,
        RunDsmRequest,
        AnalyticalResultsResponse
    >
{
    public override async Task<Result<AnalyticalResultsResponse>> ExecuteRequestAsync(
        ModelResourceRequest<RunDsmRequest> req,
        CancellationToken ct = default
    ) =>
        await runOpenSeesCommandHandler.ExecuteAsync(
            new()
            {
                ModelId = req.ModelId,
                UnitsOverride = req.Body?.UnitsOverride,
                LoadCombinationIds = req.Body?.LoadCombinationIds,
            },
            ct
        );
}
