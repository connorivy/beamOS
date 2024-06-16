using BeamOs.Domain.Common.Interfaces;
using BeamOs.Domain.Common.ValueObjects;

namespace BeamOs.Domain.AnalyticalResults.AnalyticalElement1dAggregate.ValueObjects;

public sealed class Element1dResultId(Guid? id = null)
    : GuidBasedId(id),
        IConstructable<Element1dResultId, Guid>
{
    public static Element1dResultId Construct(Guid t1) => new(t1);
}
