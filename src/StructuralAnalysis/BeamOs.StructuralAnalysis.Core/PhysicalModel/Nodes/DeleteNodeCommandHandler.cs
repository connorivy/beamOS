using BeamOs.StructuralAnalysis.Application.Common;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.NodeAggregate;

namespace BeamOs.StructuralAnalysis.Application.PhysicalModel.Nodes;

internal class DeleteNodeCommandHandler(
    INodeDefinitionRepository repo,
    IStructuralAnalysisUnitOfWork unitOfWork
) : DeleteModelEntityCommandHandler<NodeId, Node>(repo, unitOfWork) { }

// internal class GetNodeCommandHandler(INodeDefinitionRepository repo)
//     : GetModelEntityCommandHandler<NodeId, Node, NodeResponse>(repo)
// {
//     protected override NodeResponse MapToResponse(Node entity) => entity.ToResponse();
// }

internal class DeleteInternalNodeCommandHandler(
    INodeDefinitionRepository repo,
    IStructuralAnalysisUnitOfWork unitOfWork
) : DeleteModelEntityCommandHandler<NodeId, InternalNode>(repo, unitOfWork) { }

// internal class GetInternalNodeCommandHandler(INodeDefinitionRepository repo)
//     : GetModelEntityCommandHandler<NodeId, InternalNode, InternalNodeContract>(repo)
// {
//     protected override InternalNodeContract MapToResponse(InternalNode entity) =>
//         entity.ToResponse();
// }
