using BeamOs.StructuralAnalysis.Application.Common;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Node;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.NodeAggregate;

namespace BeamOs.StructuralAnalysis.Application.PhysicalModel.Nodes;

public class DeleteNodeCommandHandler(
    INodeRepository repo,
    IStructuralAnalysisUnitOfWork unitOfWork
) : DeleteModelEntityCommandHandler<NodeId, Node>(repo, unitOfWork) { }

public class GetNodeCommandHandler(INodeRepository repo)
    : GetModelEntityCommandHandler<NodeId, Node, NodeResponse>(repo)
{
    protected override NodeResponse MapToResponse(Node entity) => entity.ToResponse();
}
