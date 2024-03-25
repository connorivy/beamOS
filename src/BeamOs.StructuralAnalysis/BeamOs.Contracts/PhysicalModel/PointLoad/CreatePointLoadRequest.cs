using BeamOs.Contracts.Common;

namespace BeamOs.Contracts.PhysicalModel.PointLoad;

public record CreatePointLoadRequest(string NodeId, UnitValueDto Force, Vector3 Direction);
