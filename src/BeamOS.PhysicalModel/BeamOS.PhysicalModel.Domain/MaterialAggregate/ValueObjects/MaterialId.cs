using BeamOS.Common.Domain.ValueObjects;

namespace BeamOS.PhysicalModel.Domain.MaterialAggregate.ValueObjects;

public class MaterialId(Guid? id = null) : GuidBasedId(id) { }
