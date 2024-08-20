using BeamOs.Common.Domain.Interfaces;
using BeamOs.Domain.PhysicalModel.Common.ValueObjects;

namespace BeamOs.Domain.PhysicalModel.NodeAggregate.ValueObjects;

public class NodeId(Guid? id = null) : NodeBaseId(id), IConstructable<NodeId, Guid>
{
    public static NodeId Construct(Guid t1) => new(t1);
}
