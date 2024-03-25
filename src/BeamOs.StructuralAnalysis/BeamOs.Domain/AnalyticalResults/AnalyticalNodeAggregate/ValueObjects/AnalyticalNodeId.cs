using BeamOs.Domain.Common.Interfaces;
using BeamOs.Domain.Common.ValueObjects;

namespace BeamOs.Domain.AnalyticalResults.AnalyticalNodeAggregate.ValueObjects;

public sealed class AnalyticalNodeId(Guid? id = null)
    : GuidBasedId(id),
        IConstructable<AnalyticalNodeId, Guid>
{
    public static AnalyticalNodeId Construct(Guid t1) => new(t1);
}
