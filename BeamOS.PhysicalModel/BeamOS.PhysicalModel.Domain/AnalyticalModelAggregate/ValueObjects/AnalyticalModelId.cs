using BeamOS.Common.Domain.ValueObjects;

namespace BeamOS.PhysicalModel.Domain.AnalyticalModelAggregate.ValueObjects;
public class AnalyticalModelId(Guid? id = null) : GuidBasedId(id)
{
}
