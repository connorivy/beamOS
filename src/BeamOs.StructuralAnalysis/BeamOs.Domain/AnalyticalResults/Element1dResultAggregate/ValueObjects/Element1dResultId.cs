using BeamOs.Common.Domain.Interfaces;
using BeamOs.Domain.Common.ValueObjects;

namespace BeamOs.Domain.AnalyticalResults.Element1dResultAggregate.ValueObjects;

public sealed class Element1dResultId(Guid? id = null)
    : GuidBasedId(id),
        IConstructable<Element1dResultId, Guid>
{
    public static Element1dResultId Construct(Guid t1) => new(t1);
}
