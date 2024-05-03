using BeamOs.Contracts.Common;

namespace BeamOs.Contracts.PhysicalModel.PointLoad;

public record PointLoadResponse(
    string Id,
    string ModelId,
    string NodeId,
    UnitValueDto Force,
    Vector3 NormalizedDirection
);
