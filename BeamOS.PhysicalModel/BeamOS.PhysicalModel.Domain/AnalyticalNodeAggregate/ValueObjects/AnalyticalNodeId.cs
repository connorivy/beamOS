using BeamOS.Common.Domain.ValueObjects;
using BeamOS.PhysicalModel.Domain.Common.ValueObjects;

namespace BeamOS.PhysicalModel.Domain.AnalyticalNodeAggregate.ValueObjects;
public class AnalyticalNodeId(Guid? id = null) : NodeBaseId(id)
{
}
