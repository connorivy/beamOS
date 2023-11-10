using BeamOS.Common.Domain.ValueObjects;

namespace BeamOS.PhysicalModel.Domain.PointLoadAggregate.ValueObjects;
public class PointLoadId(Guid? id = null) : GuidBasedId(id)
{
}
