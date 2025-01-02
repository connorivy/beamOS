using BeamOs.Common.Api;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Application.OpenSees;
using Microsoft.AspNetCore.Mvc;

namespace BeamOs.StructuralAnalysis.Api.Endpoints.OpenSees;

[BeamOsRoute(RouteConstants.ModelRoutePrefixWithTrailingSlash + "analyze/opensees")]
[BeamOsEndpointType(Http.Post)]
[BeamOsRequiredAuthorizationLevel(UserAuthorizationLevel.Contributor)]
public class RunOpenSeesAnalysis(RunOpenSeesCommandHandler runOpenSeesCommandHandler)
    : BeamOsBaseEndpoint<OpenSeesRequest, int>
{
    public override async Task<Result<int>> ExecuteRequestAsync(
        OpenSeesRequest req,
        CancellationToken ct = default
    ) => await runOpenSeesCommandHandler.ExecuteAsync(req.ModelId, ct);
}

public readonly struct OpenSeesRequest
{
    [FromRoute]
    public Guid ModelId { get; init; }
}
