using BeamOs.Common.Api;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Application.DirectStiffnessMethod;
using BeamOs.StructuralAnalysis.Contracts.AnalyticalResults.Diagrams;

namespace BeamOs.StructuralAnalysis.Api.Endpoints.DirectStiffnessMethod;

[BeamOsRoute(RouteConstants.ModelRoutePrefixWithTrailingSlash + "analyze/dsm")]
[BeamOsEndpointType(Http.Post)]
[BeamOsRequiredAuthorizationLevel(UserAuthorizationLevel.Contributor)]
public class RunDirectStiffnessMethod(RunDirectStiffnessMethodCommandHandler dsmCommandHandler)
    : BeamOsBaseEndpoint<RunDsmRequest, AnalyticalResultsResponse>
{
    public override async Task<Result<AnalyticalResultsResponse>> ExecuteRequestAsync(
        RunDsmRequest req,
        CancellationToken ct = default
    ) =>
        await dsmCommandHandler.ExecuteAsync(
            new() { ModelId = req.ModelId, UnitsOverride = req.UnitsOverride },
            ct
        );
}

public record RunDsmRequest : IHasModelId
{
    [FromRoute]
    public Guid ModelId { get; init; }

    [FromQuery]
    public string? UnitsOverride { get; init; } = null;
}
