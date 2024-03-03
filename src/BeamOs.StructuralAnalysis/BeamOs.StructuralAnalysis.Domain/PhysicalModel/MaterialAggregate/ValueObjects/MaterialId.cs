using BeamOs.Domain.Common.ValueObjects;

namespace BeamOs.Domain.PhysicalModel.MaterialAggregate.ValueObjects;

public class MaterialId(Guid? id = null) : GuidBasedId(id) { }
