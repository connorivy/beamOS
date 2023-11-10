using BeamOS.Common.Domain.ValueObjects;

namespace BeamOS.PhysicalModel.Domain.ModelAggregate.ValueObjects;
public class ModelId(Guid? id = null) : GuidBasedId(id)
{
}
