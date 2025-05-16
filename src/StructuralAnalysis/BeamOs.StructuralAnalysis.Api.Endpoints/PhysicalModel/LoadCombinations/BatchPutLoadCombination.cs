using BeamOs.Common.Api;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Application.Common;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.LoadCombinations;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Nodes;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.LoadCombinations;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelAggregate;
using LoadCombination = BeamOs.StructuralAnalysis.Contracts.PhysicalModel.LoadCombinations.LoadCombination;

namespace BeamOs.StructuralAnalysis.Api.Endpoints.PhysicalModel.LoadCombinations;

[BeamOsRoute(RouteConstants.ModelRoutePrefixWithTrailingSlash + "load-combinations")]
[BeamOsEndpointType(Http.Put)]
[BeamOsRequiredAuthorizationLevel(UserAuthorizationLevel.Contributor)]
public class BatchPutLoadCombination(
    BatchPutLoadCombinationCommandHandler putLoadCombinationCommandHandler
)
    : BeamOsModelResourceBaseEndpoint<
        BatchPutLoadCombinationCommand,
        LoadCombination[],
        BatchResponse
    >
{
    public override async Task<Result<BatchResponse>> ExecuteRequestAsync(
        BatchPutLoadCombinationCommand req,
        CancellationToken ct = default
    ) => await putLoadCombinationCommandHandler.ExecuteAsync(req, ct);
}

public sealed class BatchPutLoadCombinationCommandHandler(
    ILoadCombinationRepository repository,
    IStructuralAnalysisUnitOfWork unitOfWork
)
    : BatchPutCommandHandler<
        LoadCombinationId,
        Domain.PhysicalModel.LoadCombinations.LoadCombination,
        BatchPutLoadCombinationCommand,
        LoadCombination
    >(repository, unitOfWork)
{
    protected override Domain.PhysicalModel.LoadCombinations.LoadCombination ToDomainObject(
        ModelId modelId,
        LoadCombination putRequest
    ) => new PutLoadCombinationCommand(modelId, putRequest).ToDomainObject();
}

public readonly struct BatchPutLoadCombinationCommand : IModelResourceRequest<LoadCombination[]>
{
    public Guid ModelId { get; init; }
    public LoadCombination[] Body { get; init; }
}
