using System.Diagnostics.CodeAnalysis;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Contracts.Common;

namespace BeamOs.StructuralAnalysis.Contracts.PhysicalModel.MomentLoad;

public record CreateMomentLoadRequest
{
    public required int NodeId { get; init; }
    public required Torque Torque { get; init; }
    public required Vector3 AxisDirection { get; init; }
    public int? Id { get; init; }

    public CreateMomentLoadRequest() { }

    [SetsRequiredMembers]
    public CreateMomentLoadRequest(int nodeId, Torque torque, Vector3 axisDirection, int? id = null)
    {
        this.NodeId = nodeId;
        this.Torque = torque;
        this.AxisDirection = axisDirection;
        this.Id = id;
    }
}

public record MomentLoadData
{
    public required int NodeId { get; init; }
    public required Torque Torque { get; init; }
    public required Vector3 AxisDirection { get; init; }

    public MomentLoadData() { }

    [SetsRequiredMembers]
    public MomentLoadData(int nodeId, Torque torque, Vector3 axisDirection)
    {
        this.NodeId = nodeId;
        this.Torque = torque;
        this.AxisDirection = axisDirection;
    }
}

public record PutMomentLoadRequest : MomentLoadData, IHasIntId
{
    public int Id { get; init; }
}
