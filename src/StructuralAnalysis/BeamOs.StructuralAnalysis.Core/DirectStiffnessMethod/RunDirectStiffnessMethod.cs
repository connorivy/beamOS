using BeamOs.Common.Api;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Application.DirectStiffnessMethod;
using BeamOs.StructuralAnalysis.Contracts.AnalyticalResults.Diagrams;
using BeamOs.StructuralAnalysis.Contracts.Common;

namespace BeamOs.StructuralAnalysis.Api.Endpoints.DirectStiffnessMethod;

[BeamOsRoute(RouteConstants.ModelRoutePrefixWithTrailingSlash + "analyze/dsm")]
[BeamOsEndpointType(Http.Post)]
[BeamOsRequiredAuthorizationLevel(UserAuthorizationLevel.Contributor)]
internal class RunDirectStiffnessMethod(RunDirectStiffnessMethodCommandHandler dsmCommandHandler)
    : BeamOsModelResourceBaseEndpoint<RunDsmRequest, AnalyticalResultsResponse>
{
    public override async Task<Result<AnalyticalResultsResponse>> ExecuteRequestAsync(
        ModelResourceRequest<RunDsmRequest> req,
        CancellationToken ct = default
    ) =>
        await dsmCommandHandler.ExecuteAsync(
            new()
            {
                ModelId = req.ModelId,
                UnitsOverride = req.Body?.UnitsOverride,
                LoadCombinationIds = req.Body?.LoadCombinationIds,
            },
            ct
        );
}
