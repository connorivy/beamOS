using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Contracts.Common;

namespace BeamOs.StructuralAnalysis.Contracts.PhysicalModel.MomentLoad;

public record MomentLoadResponse(
    int Id,
    int NodeId,
    Guid ModelId,
    TorqueContract Torque,
    Vector3 AxisDirection
) : IModelEntity;
