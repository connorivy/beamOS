using BeamOS.Common.Contracts;

namespace BeamOS.PhysicalModel.Contracts.MomentLoad;

public record MomentLoadResponse(
    string Id,
    string NodeId,
    UnitValueDTO Torque,
    Vector3 NormalizedAxisDirection
);
