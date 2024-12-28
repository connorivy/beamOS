using BeamOs.StructuralAnalysis.Contracts.Common;

namespace BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Node;

public record NodeResponse(int Id, Guid ModelId, Point LocationPoint, Restraint Restraint)
    : IModelEntity;
