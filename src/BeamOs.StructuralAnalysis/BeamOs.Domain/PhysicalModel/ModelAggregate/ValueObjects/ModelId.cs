using BeamOs.Domain.Common.Interfaces;
using BeamOs.Domain.Common.ValueObjects;

namespace BeamOs.Domain.PhysicalModel.ModelAggregate.ValueObjects;

public class ModelId(Guid? id = null) : GuidBasedId(id), IConstructable<ModelId, Guid>
{
    public static ModelId Construct(Guid id) => new(id);
}
