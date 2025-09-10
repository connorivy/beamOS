using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Contracts.Common;

namespace BeamOs.StructuralAnalysis.Contracts.PhysicalModel.MomentLoads;

public record MomentLoadResponse(
    int Id,
    int NodeId,
    int LoadCaseId,
    Guid ModelId,
    TorqueContract Torque,
    Vector3 AxisDirection
) : IModelEntity
{
    public MomentLoadData ToData() => new(NodeId, LoadCaseId, Torque, AxisDirection);
}
