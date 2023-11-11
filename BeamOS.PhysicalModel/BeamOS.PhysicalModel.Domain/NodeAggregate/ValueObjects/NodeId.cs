using BeamOS.PhysicalModel.Domain.Common.Interfaces;
using BeamOS.PhysicalModel.Domain.Common.ValueObjects;

namespace BeamOS.PhysicalModel.Domain.NodeAggregate.ValueObjects;
public class NodeId(Guid? id = null) : NodeBaseId(id), IConstructable<NodeId, Guid>
{
    public static NodeId Construct(Guid t1) => new(t1);
}
