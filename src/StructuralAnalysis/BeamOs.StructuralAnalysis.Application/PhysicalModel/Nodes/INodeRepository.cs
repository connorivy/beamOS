using BeamOs.StructuralAnalysis.Application.Common;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.NodeAggregate;

namespace BeamOs.StructuralAnalysis.Application.PhysicalModel.Nodes;

public interface INodeRepository : IModelResourceRepository<NodeId, Node>
{
    public Task<Node> Update(PatchNodeCommand patchCommand);
    public Task<List<Node>> GetAll();
}

public interface IInternalNodeRepository : IModelResourceRepository<NodeId, InternalNode> { }

public interface INodeProposalRepository : IProposalRepository<NodeProposalId, NodeProposal> { }
