using BeamOs.Common.Domain.Interfaces;
using BeamOs.Domain.Common.ValueObjects;

namespace BeamOs.Domain.AnalyticalModel.AnalyticalResultsAggregate.ValueObjects;

public sealed class AnalyticalResultsId(Guid? id = null)
    : GuidBasedId(id),
        IConstructable<AnalyticalResultsId, Guid>
{
    public static AnalyticalResultsId Construct(Guid t1) => new(t1);
}
