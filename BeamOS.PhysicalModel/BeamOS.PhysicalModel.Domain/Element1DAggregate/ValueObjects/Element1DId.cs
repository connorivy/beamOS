using BeamOS.Common.Domain.ValueObjects;

namespace BeamOS.PhysicalModel.Domain.Element1DAggregate.ValueObjects;
public class Element1DId(Guid? value = null) : GuidBasedId(value)
{
    public static Element1DId CreateUnique()
    {
        return new(Guid.NewGuid());
    }
}
