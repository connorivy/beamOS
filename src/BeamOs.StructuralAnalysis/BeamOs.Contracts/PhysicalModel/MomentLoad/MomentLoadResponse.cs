using BeamOs.Contracts.Common;

namespace BeamOs.Contracts.PhysicalModel.MomentLoad;

public record MomentLoadResponse(
    string Id,
    string ModelId,
    string NodeId,
    TorqueContract Torque,
    Vector3 AxisDirection
);
