using BeamOS.Common.Contracts;
using BeamOS.PhysicalModel.Contracts.Common;

namespace BeamOS.PhysicalModel.Contracts.PointLoad;
public record CreatePointLoadRequest(
    string NodeId,
    UnitValueDTO Force,
    Vector3 Direction);
