using BeamOs.Common.Api;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Api.Endpoints.DirectStiffnessMethod;
using BeamOs.StructuralAnalysis.Application.OpenSees;
using BeamOs.StructuralAnalysis.Contracts.AnalyticalResults.Diagrams;
using BeamOs.StructuralAnalysis.Contracts.Common;

namespace BeamOs.StructuralAnalysis.Api.Endpoints.OpenSees;

[BeamOsRoute(RouteConstants.ModelRoutePrefixWithTrailingSlash + "analyze/opensees")]
[BeamOsEndpointType(Http.Post)]
[BeamOsRequiredAuthorizationLevel(UserAuthorizationLevel.Contributor)]
public class RunOpenSeesAnalysis(RunOpenSeesCommandHandler runOpenSeesCommandHandler)
    : BeamOsBaseEndpoint<ModelIdAndBody<RunDsmRequest>, AnalyticalResultsResponse>
{
    public override async Task<Result<AnalyticalResultsResponse>> ExecuteRequestAsync(
        ModelIdAndBody<RunDsmRequest> req,
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

public readonly struct ModelIdRequest : IHasModelId
{
    [FromRoute]
    public Guid ModelId { get; init; }
}

public readonly struct ModelEntityRequest : IModelEntity
{
    [FromRoute]
    public Guid ModelId { get; init; }

    [FromRoute]
    public int Id { get; init; }
}
