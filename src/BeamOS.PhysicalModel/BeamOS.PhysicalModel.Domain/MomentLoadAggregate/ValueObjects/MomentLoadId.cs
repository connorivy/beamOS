using BeamOS.Common.Domain.Interfaces;
using BeamOS.Common.Domain.ValueObjects;

namespace BeamOS.PhysicalModel.Domain.MomentLoadAggregate.ValueObjects;

public class MomentLoadId(Guid? id = null) : GuidBasedId(id), IConstructable<MomentLoadId, Guid>
{
    public static MomentLoadId Construct(Guid t1) => new(t1);
}
