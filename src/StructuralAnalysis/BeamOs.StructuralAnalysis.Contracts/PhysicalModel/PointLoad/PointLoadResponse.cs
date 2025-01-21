using BeamOs.StructuralAnalysis.Contracts.Common;

namespace BeamOs.StructuralAnalysis.Contracts.PhysicalModel.PointLoad;

public record PointLoadResponse(
    int Id,
    int NodeId,
    Guid ModelId,
    ForceContract Force,
    Vector3 Direction
) : IModelEntity;
