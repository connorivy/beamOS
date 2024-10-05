using System.Diagnostics.CodeAnalysis;
using BeamOs.Contracts.Common;

namespace BeamOs.Contracts.PhysicalModel.MomentLoad;

public record CreateMomentLoadRequest
{
    public required string NodeId { get; init; }
    public required TorqueContract Torque { get; init; }
    public required Vector3 AxisDirection { get; init; }

    public CreateMomentLoadRequest() { }

    [SetsRequiredMembers]
    public CreateMomentLoadRequest(string nodeId, TorqueContract torque, Vector3 axisDirection)
    {
        this.NodeId = nodeId;
        this.Torque = torque;
        this.AxisDirection = axisDirection;
    }
}
