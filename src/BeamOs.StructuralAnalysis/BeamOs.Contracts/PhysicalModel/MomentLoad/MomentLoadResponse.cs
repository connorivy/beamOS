using BeamOs.Contracts.Common;

namespace BeamOs.Contracts.PhysicalModel.MomentLoad;

public record MomentLoadResponse(
    string Id,
    string NodeId,
    UnitValueDto Torque,
    Vector3 AxisDirection
);
