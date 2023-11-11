using BeamOS.Common.Domain.ValueObjects;
using BeamOS.PhysicalModel.Domain.Common.Interfaces;

namespace BeamOS.PhysicalModel.Domain.ModelAggregate.ValueObjects;
public class ModelId(Guid? id = null) : GuidBasedId(id), IConstructable<ModelId, Guid>
{
    public static ModelId Construct(Guid id) => new(id);
}
