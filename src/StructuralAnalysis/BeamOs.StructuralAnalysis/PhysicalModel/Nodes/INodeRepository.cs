using BeamOs.StructuralAnalysis.Application.Common;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.NodeAggregate;

namespace BeamOs.StructuralAnalysis.Application.PhysicalModel.Nodes;

internal interface INodeRepository : IModelResourceRepository<NodeId, Node>
{
    // public Task<Node> Update(PatchNodeCommand patchCommand);
    // public Task<List<Node>> GetAll();
}

// internal interface INodeRepository : IModelResourceRepositoryIn<NodeId, Node> { }

internal interface IInternalNodeRepository : IModelResourceRepository<NodeId, InternalNode> { }

internal interface INodeProposalRepository : IProposalRepository<NodeProposalId, NodeProposal> { }

internal interface INodeDefinitionRepository : IModelResourceRepository<NodeId, NodeDefinition> { }
