using BeamOS.Common.Domain.Interfaces;
using BeamOS.Common.Domain.ValueObjects;

namespace BeamOS.DirectStiffnessMethod.Domain.AnalyticalElement1DAggregate.ValueObjects;

public class AnalyticalElement1DId(Guid? id = null)
    : GuidBasedId(id),
        IConstructable<AnalyticalElement1DId, Guid>
{
    public static AnalyticalElement1DId Construct(Guid id) => new(id);
}
