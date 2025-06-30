using BeamOs.StructuralAnalysis.Application.Common;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.NodeAggregate;

namespace BeamOs.StructuralAnalysis.Application.PhysicalModel.Nodes;

public class DeleteNodeCommandHandler(
    INodeDefinitionRepository repo,
    IStructuralAnalysisUnitOfWork unitOfWork
) : DeleteModelEntityCommandHandler<NodeId, Node>(repo, unitOfWork) { }

// public class GetNodeCommandHandler(INodeDefinitionRepository repo)
//     : GetModelEntityCommandHandler<NodeId, Node, NodeResponse>(repo)
// {
//     protected override NodeResponse MapToResponse(Node entity) => entity.ToResponse();
// }

public class DeleteInternalNodeCommandHandler(
    INodeDefinitionRepository repo,
    IStructuralAnalysisUnitOfWork unitOfWork
) : DeleteModelEntityCommandHandler<NodeId, InternalNode>(repo, unitOfWork) { }

// public class GetInternalNodeCommandHandler(INodeDefinitionRepository repo)
//     : GetModelEntityCommandHandler<NodeId, InternalNode, InternalNodeContract>(repo)
// {
//     protected override InternalNodeContract MapToResponse(InternalNode entity) =>
//         entity.ToResponse();
// }
