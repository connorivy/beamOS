using BeamOs.Common.Domain.Interfaces;
using BeamOs.Domain.Common.ValueObjects;

namespace BeamOs.Domain.AnalyticalResults.ModelResultAggregate.ValueObjects;

public sealed class ModelResultId(Guid? id = null)
    : GuidBasedId(id),
        IConstructable<ModelResultId, Guid>
{
    public static ModelResultId Construct(Guid t1) => new(t1);
}
