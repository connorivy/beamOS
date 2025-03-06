using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Contracts.Common;

namespace BeamOs.StructuralAnalysis.Contracts.PhysicalModel.MomentLoad;

public record MomentLoadResponse(
    int Id,
    int NodeId,
    Guid ModelId,
    Torque Torque,
    Vector3 AxisDirection
) : IModelEntity;
