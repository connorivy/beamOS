using BeamOs.Common.Api;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Application.Common;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.LoadCombinations;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.LoadCombinations;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.LoadCombinations;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelAggregate;
using LoadCombination = BeamOs.StructuralAnalysis.Contracts.PhysicalModel.LoadCombinations.LoadCombination;

namespace BeamOs.StructuralAnalysis.Api.Endpoints.PhysicalModel.LoadCombinations;

[BeamOsRoute(RouteConstants.ModelRoutePrefixWithTrailingSlash + "load-combinations/{id}")]
[BeamOsEndpointType(Http.Put)]
[BeamOsRequiredAuthorizationLevel(UserAuthorizationLevel.Contributor)]
public class PutLoadCombination(PutLoadCombinationCommandHandler putLoadCombinationCommandHandler)
    : BeamOsModelResourceWithIntIdBaseEndpoint<
        PutLoadCombinationCommand,
        LoadCombinationData,
        LoadCombination
    >
{
    public override async Task<Result<LoadCombination>> ExecuteRequestAsync(
        PutLoadCombinationCommand req,
        CancellationToken ct = default
    ) => await putLoadCombinationCommandHandler.ExecuteAsync(req, ct);
}

public sealed class PutLoadCombinationCommandHandler(
    ILoadCombinationRepository repository,
    IStructuralAnalysisUnitOfWork unitOfWork
)
    : PutCommandHandlerBase<
        LoadCombinationId,
        Domain.PhysicalModel.LoadCombinations.LoadCombination,
        PutLoadCombinationCommand,
        LoadCombination
    >(repository, unitOfWork)
{
    protected override Domain.PhysicalModel.LoadCombinations.LoadCombination ToDomainObject(
        PutLoadCombinationCommand putCommand
    ) => putCommand.ToDomainObject();

    protected override LoadCombination ToResponse(
        Domain.PhysicalModel.LoadCombinations.LoadCombination entity
    ) => entity.ToResponse();
}

public readonly struct PutLoadCombinationCommand
    : IModelResourceWithIntIdRequest<LoadCombinationData>
{
    public Guid ModelId { get; init; }
    public LoadCombinationData Body { get; init; }
    public Dictionary<int, double> LoadCaseFactors => this.Body.LoadCaseFactors;
    public int Id { get; init; }

    public PutLoadCombinationCommand() { }

    public PutLoadCombinationCommand(ModelId modelId, LoadCombination putElement1DRequest)
    {
        this.Id = putElement1DRequest.Id;
        this.ModelId = modelId;
        this.Body = putElement1DRequest;
    }
}
