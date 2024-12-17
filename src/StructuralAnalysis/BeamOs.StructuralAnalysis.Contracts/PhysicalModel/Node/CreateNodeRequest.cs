using BeamOs.StructuralAnalysis.Contracts.Common;

namespace BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Node;

public record CreateNodeRequest(Point LocationPoint, Restraint? Restraint, int? Id = null);
