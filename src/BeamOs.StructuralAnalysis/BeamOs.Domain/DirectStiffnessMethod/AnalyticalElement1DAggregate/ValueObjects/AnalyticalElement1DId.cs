using BeamOs.Domain.Common.Interfaces;
using BeamOs.Domain.Common.ValueObjects;

namespace BeamOs.Domain.DirectStiffnessMethod.AnalyticalElement1DAggregate.ValueObjects;

public class AnalyticalElement1DId(Guid? id = null)
    : GuidBasedId(id),
        IConstructable<AnalyticalElement1DId, Guid>
{
    public static AnalyticalElement1DId Construct(Guid id) => new(id);
}
