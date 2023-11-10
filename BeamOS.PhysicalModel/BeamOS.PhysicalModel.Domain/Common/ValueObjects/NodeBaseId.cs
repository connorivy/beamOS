using BeamOS.Common.Domain.ValueObjects;

namespace BeamOS.PhysicalModel.Domain.Common.ValueObjects;
public class NodeBaseId(Guid? id = null) : GuidBasedId(id)
{
}
