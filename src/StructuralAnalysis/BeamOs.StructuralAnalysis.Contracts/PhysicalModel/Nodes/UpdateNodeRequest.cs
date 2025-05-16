using BeamOs.StructuralAnalysis.Contracts.Common;

namespace BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Nodes;

public record UpdateNodeRequest(
    int Id,
    PartialPoint? LocationPoint = null,
    PartialRestraint? Restraint = null
);
