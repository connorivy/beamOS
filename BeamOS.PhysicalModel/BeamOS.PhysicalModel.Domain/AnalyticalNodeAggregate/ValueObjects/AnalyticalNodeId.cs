using BeamOS.Common.Domain.ValueObjects;

namespace BeamOS.PhysicalModel.Domain.AnalyticalNodeAggregate.ValueObjects;
public class AnalyticalNodeId(Guid? value = null) : GuidBasedId(value)
{
    public static AnalyticalNodeId CreateUnique()
    {
        return new(Guid.NewGuid());
    }
}
