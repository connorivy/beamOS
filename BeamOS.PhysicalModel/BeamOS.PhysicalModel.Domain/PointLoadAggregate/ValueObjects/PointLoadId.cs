using BeamOS.Common.Domain.ValueObjects;
using BeamOS.PhysicalModel.Domain.Common.Interfaces;

namespace BeamOS.PhysicalModel.Domain.PointLoadAggregate.ValueObjects;
public class PointLoadId(Guid? id = null) : GuidBasedId(id), IConstructable<PointLoadId, Guid>
{
    public static PointLoadId Construct(Guid t1) => new(t1);
}
