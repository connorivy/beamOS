using BeamOS.PhysicalModel.Domain.Common.ValueObjects;

namespace BeamOS.PhysicalModel.Domain.NodeAggregate.ValueObjects;
public class NodeId(Guid? id = null) : NodeBaseId(id)
{
}
