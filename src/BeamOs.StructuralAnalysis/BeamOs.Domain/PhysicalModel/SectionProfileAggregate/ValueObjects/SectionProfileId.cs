using BeamOs.Domain.Common.Interfaces;
using BeamOs.Domain.Common.ValueObjects;

namespace BeamOs.Domain.PhysicalModel.SectionProfileAggregate.ValueObjects;

public class SectionProfileId(Guid? id = null)
    : GuidBasedId(id),
        IConstructable<SectionProfileId, Guid>
{
    public static SectionProfileId Construct(Guid t1) => new(t1);
}
