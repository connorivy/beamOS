using BeamOs.Domain.PhysicalModel.ModelAggregate.ValueObjects;
using BeamOs.Domain.PhysicalModel.NodeAggregate;
using BeamOs.Domain.PhysicalModel.NodeAggregate.ValueObjects;

namespace BeamOs.Application.PhysicalModel.Nodes.Interfaces;

public interface INodeRepository : IRepository<NodeId, Node>
{
    public Task<List<Node>> GetNodesInModel(ModelId modelId, CancellationToken ct = default);
    public Task<ModelId> GetModelId(NodeId nodeId, CancellationToken ct = default);
}
