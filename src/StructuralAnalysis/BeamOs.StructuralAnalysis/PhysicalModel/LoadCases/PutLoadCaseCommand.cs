using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Application.Common;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.LoadCases;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.LoadCases;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelAggregate;

namespace BeamOs.StructuralAnalysis.Application.PhysicalModel.LoadCases;

public readonly struct PutLoadCaseCommand : IModelResourceWithIntIdRequest<LoadCaseData>
{
    public Guid ModelId { get; init; }
    public LoadCaseData Body { get; init; }
    public string Name => this.Body.Name;
    public int Id { get; init; }

    public PutLoadCaseCommand() { }

    public PutLoadCaseCommand(ModelId modelId, LoadCaseContract putElement1DRequest)
    {
        this.Id = putElement1DRequest.Id;
        this.ModelId = modelId;
        this.Body = putElement1DRequest;
    }
}

public sealed class BatchPutLoadCaseCommandHandler(
    ILoadCaseRepository repository,
    IStructuralAnalysisUnitOfWork unitOfWork
)
    : BatchPutCommandHandler<
        LoadCaseId,
        Domain.PhysicalModel.LoadCases.LoadCase,
        BatchPutLoadCaseCommand,
        LoadCaseContract
    >(repository, unitOfWork)
{
    protected override Domain.PhysicalModel.LoadCases.LoadCase ToDomainObject(
        ModelId modelId,
        LoadCaseContract putRequest
    ) => new PutLoadCaseCommand(modelId, putRequest).ToDomainObject();
}

public readonly struct BatchPutLoadCaseCommand : IModelResourceRequest<LoadCaseContract[]>
{
    public Guid ModelId { get; init; }
    public LoadCaseContract[] Body { get; init; }
}

public sealed class PutLoadCaseCommandHandler(
    ILoadCaseRepository repository,
    IStructuralAnalysisUnitOfWork unitOfWork
)
    : PutCommandHandlerBase<
        LoadCaseId,
        Domain.PhysicalModel.LoadCases.LoadCase,
        PutLoadCaseCommand,
        LoadCaseContract
    >(repository, unitOfWork)
{
    protected override Domain.PhysicalModel.LoadCases.LoadCase ToDomainObject(
        PutLoadCaseCommand putCommand
    ) => putCommand.ToDomainObject();

    protected override LoadCaseContract ToResponse(
        Domain.PhysicalModel.LoadCases.LoadCase entity
    ) => entity.ToResponse();
}
