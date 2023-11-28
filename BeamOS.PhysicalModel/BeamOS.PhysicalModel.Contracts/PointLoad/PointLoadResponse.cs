using BeamOS.Common.Contracts;
using BeamOS.PhysicalModel.Contracts.Common;

namespace BeamOS.PhysicalModel.Contracts.PointLoad;
public record PointLoadResponse(
    string Id,
    string NodeId,
    UnitValueDTO Force,
    Vector3 NormalizedDirection);
