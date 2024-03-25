using BeamOs.Domain.Common.Interfaces;
using BeamOs.Domain.Common.ValueObjects;

namespace BeamOs.Domain.PhysicalModel.MomentLoadAggregate.ValueObjects;

public class MomentLoadId(Guid? id = null) : GuidBasedId(id), IConstructable<MomentLoadId, Guid>
{
    public static MomentLoadId Construct(Guid t1) => new(t1);
}
