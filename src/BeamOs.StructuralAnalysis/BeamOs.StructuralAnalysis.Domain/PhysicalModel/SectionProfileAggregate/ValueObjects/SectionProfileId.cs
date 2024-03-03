using BeamOs.Domain.Common.ValueObjects;

namespace BeamOs.Domain.PhysicalModel.SectionProfileAggregate.ValueObjects;

public class SectionProfileId(Guid? id = null) : GuidBasedId(id) { }
