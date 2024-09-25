using System.Diagnostics.CodeAnalysis;
using BeamOs.Contracts.Common;

namespace BeamOs.Contracts.PhysicalModel.PointLoad;

//public record CreatePointLoadRequest(string NodeId, UnitValueDto Force, Vector3 Direction);

public record CreatePointLoadRequest
{
    public required string NodeId { get; init; }
    public required ForceContract Force { get; init; }
    public required Vector3 Direction { get; init; }

    public CreatePointLoadRequest() { }

    [SetsRequiredMembers]
    public CreatePointLoadRequest(string nodeId, ForceContract force, Vector3 direction)
    {
        this.NodeId = nodeId;
        this.Force = force;
        this.Direction = direction;
    }
}
