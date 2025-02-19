using BeamOs.Common.Api;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Application.OpenSees;

namespace BeamOs.StructuralAnalysis.Api.Endpoints.OpenSees;

[BeamOsRoute(RouteConstants.ModelRoutePrefixWithTrailingSlash + "analyze/opensees")]
[BeamOsEndpointType(Http.Post)]
[BeamOsRequiredAuthorizationLevel(UserAuthorizationLevel.Contributor)]
public class RunOpenSeesAnalysis(RunOpenSeesCommandHandler runOpenSeesCommandHandler)
    : BeamOsModelIdRequestBaseEndpoint<int>
{
    public override async Task<Result<int>> ExecuteRequestAsync(
        ModelIdRequest req,
        CancellationToken ct = default
    ) => await runOpenSeesCommandHandler.ExecuteAsync(req.ModelId, ct);
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
