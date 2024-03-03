using BeamOs.Domain.Common.Interfaces;
using BeamOs.Domain.Common.ValueObjects;

namespace BeamOs.Domain.DirectStiffnessMethod.AnalyticalModelAggregate.ValueObjects;

public class AnalyticalModelId(Guid? id = null)
    : GuidBasedId(id),
        IConstructable<AnalyticalModelId, Guid>
{
    public static AnalyticalModelId Construct(Guid t1) => new(t1);
}
