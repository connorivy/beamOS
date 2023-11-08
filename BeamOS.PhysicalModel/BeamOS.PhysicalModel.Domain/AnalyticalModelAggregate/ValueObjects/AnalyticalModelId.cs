using BeamOS.Common.Domain.ValueObjects;

namespace BeamOS.PhysicalModel.Domain.AnalyticalModelAggregate.ValueObjects;
public class AnalyticalModelId(Guid? modelId = null) : GuidBasedId(modelId)
{
    public static AnalyticalModelId CreateUnique()
    {
        return new(Guid.NewGuid());
    }
}
