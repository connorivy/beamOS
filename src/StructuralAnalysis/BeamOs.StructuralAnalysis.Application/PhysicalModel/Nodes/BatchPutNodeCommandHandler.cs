using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Application.Common;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Nodes;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.NodeAggregate;

namespace BeamOs.StructuralAnalysis.Application.PhysicalModel.Nodes;

public class BatchPutNodeCommandHandler(
    INodeRepository repository,
    IStructuralAnalysisUnitOfWork unitOfWork
)
    : BatchPutCommandHandler<NodeId, Node, BatchPutNodeCommand, PutNodeRequest>(
        repository,
        unitOfWork
    )
{
    protected override Node ToDomainObject(ModelId modelId, PutNodeRequest putRequest) =>
        new PutNodeCommand(modelId, putRequest).ToDomainObject();
}

public readonly struct BatchPutNodeCommand : IModelResourceRequest<PutNodeRequest[]>
{
    public Guid ModelId { get; init; }
    public PutNodeRequest[] Body { get; init; }
}
