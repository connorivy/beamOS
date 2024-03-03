using BeamOs.Domain.Common.ValueObjects;

namespace BeamOs.Domain.PhysicalModel.Common.ValueObjects;

public class NodeBaseId(Guid? id = null) : GuidBasedId(id) { }
