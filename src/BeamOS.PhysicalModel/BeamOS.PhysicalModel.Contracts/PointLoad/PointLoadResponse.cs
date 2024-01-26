using BeamOS.Common.Contracts;

namespace BeamOS.PhysicalModel.Contracts.PointLoad;

public record PointLoadResponse(
    string Id,
    string NodeId,
    UnitValueDTO Force,
    Vector3 NormalizedDirection
);
