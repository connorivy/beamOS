using BeamOS.Common.Domain.ValueObjects;

namespace BeamOS.PhysicalModel.Domain.SectionProfileAggregate.ValueObjects;
public class SectionProfileId(Guid? id = null) : GuidBasedId(id)
{
}
