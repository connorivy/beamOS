using BeamOS.Common.Domain.Interfaces;
using BeamOS.Common.Domain.ValueObjects;

namespace BeamOS.PhysicalModel.Domain.Element1DAggregate.ValueObjects;
public class Element1DId(Guid? id = null) : GuidBasedId(id), IConstructable<Element1DId, Guid>
{
    public static Element1DId Construct(Guid t1) => new(t1);
}
