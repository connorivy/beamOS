using BeamOs.Contracts.Common;

namespace BeamOs.Contracts.PhysicalModel.MomentLoad;

public record CreateMomentLoadRequest(string NodeId, UnitValueDto Torque, Vector3 AxisDirection);
