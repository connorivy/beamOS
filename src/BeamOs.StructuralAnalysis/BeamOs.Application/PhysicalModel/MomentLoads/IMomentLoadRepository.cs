using BeamOs.Domain.PhysicalModel.MomentLoadAggregate;
using BeamOs.Domain.PhysicalModel.MomentLoadAggregate.ValueObjects;
using BeamOs.Domain.PhysicalModel.NodeAggregate;
using BeamOs.Domain.PhysicalModel.NodeAggregate.ValueObjects;

namespace BeamOs.Application.PhysicalModel.MomentLoads;

public interface IMomentLoadRepository : IRepository<MomentLoadId, MomentLoad>
{
    public Task<List<MomentLoad>> GetMomentLoadsBelongingToNode(NodeId nodeId);
    public Task<List<MomentLoad>> GetMomentLoadsBelongingToNodes(IList<NodeId> nodeIds);
    public Task<List<MomentLoad>> GetMomentLoadsBelongingToNodes(IList<Node> nodes);
}
