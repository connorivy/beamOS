using BeamOs.Domain.Common.Interfaces;
using BeamOs.Domain.Common.ValueObjects;

namespace BeamOs.Domain.PhysicalModel.Element1DAggregate.ValueObjects;

public class Element1DId(Guid? id = null) : GuidBasedId(id), IConstructable<Element1DId, Guid>
{
    public static Element1DId Construct(Guid t1) => new(t1);
}
