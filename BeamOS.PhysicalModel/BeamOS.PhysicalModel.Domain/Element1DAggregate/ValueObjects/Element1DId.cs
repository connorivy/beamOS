using BeamOS.Common.Domain.ValueObjects;

namespace BeamOS.PhysicalModel.Domain.Element1DAggregate.ValueObjects;
public class Element1DId(Guid? id = null) : GuidBasedId(id)
{
}
