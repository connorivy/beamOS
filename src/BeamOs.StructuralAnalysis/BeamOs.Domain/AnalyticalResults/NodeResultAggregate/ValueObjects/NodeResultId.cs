using BeamOs.Domain.Common.Interfaces;
using BeamOs.Domain.Common.ValueObjects;

namespace BeamOs.Domain.AnalyticalResults.AnalyticalNodeAggregate.ValueObjects;

public sealed class NodeResultId(Guid? id = null)
    : GuidBasedId(id),
        IConstructable<NodeResultId, Guid>
{
    public static NodeResultId Construct(Guid t1) => new(t1);
}
