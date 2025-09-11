using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Application.Common;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.LoadCombinations;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.LoadCombinations;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelAggregate;

namespace BeamOs.StructuralAnalysis.Application.PhysicalModel.LoadCombinations;

internal readonly struct PutLoadCombinationCommand
    : IModelResourceWithIntIdRequest<LoadCombinationData>
{
    public Guid ModelId { get; init; }
    public LoadCombinationData Body { get; init; }
    public Dictionary<int, double> LoadCaseFactors => this.Body.LoadCaseFactors;
    public int Id { get; init; }

    public PutLoadCombinationCommand() { }

    public PutLoadCombinationCommand(ModelId modelId, LoadCombinationContract putElement1DRequest)
    {
        this.Id = putElement1DRequest.Id;
        this.ModelId = modelId;
        this.Body = putElement1DRequest;
    }
}

internal sealed class PutLoadCombinationCommandHandler(
    ILoadCombinationRepository repository,
    IStructuralAnalysisUnitOfWork unitOfWork
)
    : PutCommandHandlerBase<
        LoadCombinationId,
        LoadCombination,
        PutLoadCombinationCommand,
        LoadCombinationContract
    >(repository, unitOfWork)
{
    protected override LoadCombination ToDomainObject(PutLoadCombinationCommand putCommand) =>
        putCommand.ToDomainObject();

    protected override LoadCombinationContract ToResponse(LoadCombination entity) =>
        entity.ToResponse();
}

internal sealed class BatchPutLoadCombinationCommandHandler(
    ILoadCombinationRepository repository,
    IStructuralAnalysisUnitOfWork unitOfWork
)
    : BatchPutCommandHandler<
        LoadCombinationId,
        Domain.PhysicalModel.LoadCombinations.LoadCombination,
        BatchPutLoadCombinationCommand,
        LoadCombinationContract
    >(repository, unitOfWork)
{
    protected override Domain.PhysicalModel.LoadCombinations.LoadCombination ToDomainObject(
        ModelId modelId,
        LoadCombinationContract putRequest
    ) => new PutLoadCombinationCommand(modelId, putRequest).ToDomainObject();
}

internal readonly struct BatchPutLoadCombinationCommand
    : IModelResourceRequest<LoadCombinationContract[]>
{
    public Guid ModelId { get; init; }
    public LoadCombinationContract[] Body { get; init; }
}
