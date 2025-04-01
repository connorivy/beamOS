using BeamOs.StructuralAnalysis.Application.Common;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.NodeAggregate;

namespace BeamOs.StructuralAnalysis.Application.PhysicalModel.Nodes;

public class DeleteNodeCommandHandler(
    INodeRepository repo,
    IStructuralAnalysisUnitOfWork unitOfWork
) : DeleteModelEntityCommandHandler<NodeId, Node>(repo, unitOfWork) { }

public readonly struct DeleteNodeCommand
{
    public Guid ModelId { get; init; }
    public int Id { get; init; }
}
