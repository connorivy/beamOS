using BeamOs.Common.Domain.Interfaces;
using BeamOs.Domain.Common.ValueObjects;

namespace BeamOs.Domain.PhysicalModel.PointLoadAggregate.ValueObjects;

public class PointLoadId(Guid? id = null) : GuidBasedId(id), IConstructable<PointLoadId, Guid>
{
    public static PointLoadId Construct(Guid t1) => new(t1);
}
