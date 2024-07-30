using BeamOs.Domain.Common.Interfaces;
using BeamOs.Domain.Common.ValueObjects;

namespace BeamOs.Domain.AnalyticalResults.AnalyticalModelAggregate.ValueObjects;

public sealed class ModelResultId(Guid? id = null)
    : GuidBasedId(id),
        IConstructable<ModelResultId, Guid>
{
    public static ModelResultId Construct(Guid t1) => new(t1);
}
