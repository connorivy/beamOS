using BeamOs.Common.Domain.Interfaces;
using BeamOs.Domain.Common.ValueObjects;

namespace BeamOs.Domain.PhysicalModel.MaterialAggregate.ValueObjects;

public class MaterialId(Guid? id = null) : GuidBasedId(id), IConstructable<MaterialId, Guid>
{
    public static MaterialId Construct(Guid t1) => new(t1);
}
