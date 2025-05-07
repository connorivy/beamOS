using BeamOs.Common.Api;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.LoadCombinations;

namespace BeamOs.StructuralAnalysis.Api.Endpoints.PhysicalModel.LoadCombinations;

[BeamOsRoute(RouteConstants.ModelRoutePrefixWithTrailingSlash + "load-combinations")]
[BeamOsEndpointType(Http.Post)]
[BeamOsRequiredAuthorizationLevel(UserAuthorizationLevel.Contributor)]
public class CreateLoadCombination(
    CreateLoadCombinationCommandHandler createLoadCombinationCommandHandler
)
    : BeamOsModelResourceBaseEndpoint<
        CreateLoadCombinationCommand,
        LoadCombinationData,
        LoadCombination
    >
{
    public override async Task<Result<LoadCombination>> ExecuteRequestAsync(
        CreateLoadCombinationCommand req,
        CancellationToken ct = default
    ) => await createLoadCombinationCommandHandler.ExecuteAsync(req, ct);
}

public readonly struct CreateLoadCombinationCommand : IModelResourceRequest<LoadCombinationData>
{
    public Guid ModelId { get; init; }
    public LoadCombinationData Body { get; init; }
    public Dictionary<int, double> LoadCaseFactors => this.Body.LoadCaseFactors;
}
