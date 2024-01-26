using BeamOS.Common.Domain.Interfaces;
using BeamOS.Common.Domain.ValueObjects;

namespace BeamOS.DirectStiffnessMethod.Domain.AnalyticalModelAggregate.ValueObjects;

public class AnalyticalModelId(Guid? id = null)
    : GuidBasedId(id),
        IConstructable<AnalyticalModelId, Guid>
{
    public static AnalyticalModelId Construct(Guid t1) => new(t1);
}
