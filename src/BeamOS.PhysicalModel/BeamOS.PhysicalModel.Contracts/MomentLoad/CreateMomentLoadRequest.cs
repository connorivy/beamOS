using BeamOS.Common.Contracts;

namespace BeamOS.PhysicalModel.Contracts.MomentLoad;

public record CreateMomentLoadRequest(string NodeId, UnitValueDTO Torque, Vector3 AxisDirection);
