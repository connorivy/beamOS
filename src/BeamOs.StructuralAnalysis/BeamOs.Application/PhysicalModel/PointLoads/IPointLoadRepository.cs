using BeamOs.Application.Common.Interfaces.Repositories;
using BeamOs.Domain.PhysicalModel.NodeAggregate;
using BeamOs.Domain.PhysicalModel.NodeAggregate.ValueObjects;
using BeamOs.Domain.PhysicalModel.PointLoadAggregate;
using BeamOs.Domain.PhysicalModel.PointLoadAggregate.ValueObjects;

namespace BeamOs.Application.PhysicalModel.PointLoads;

public interface IPointLoadRepository : IRepository<PointLoadId, PointLoad>
{
    public Task<List<PointLoad>> GetPointLoadsBelongingToNode(NodeId nodeId);
    public Task<List<PointLoad>> GetPointLoadsBelongingToNodes(IList<NodeId> nodeIds);
    public Task<List<PointLoad>> GetPointLoadsBelongingToNodes(IList<Node> nodes);
}
