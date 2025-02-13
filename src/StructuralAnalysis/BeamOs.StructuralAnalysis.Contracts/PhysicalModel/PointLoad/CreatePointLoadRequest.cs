using System.Diagnostics.CodeAnalysis;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Contracts.Common;

namespace BeamOs.StructuralAnalysis.Contracts.PhysicalModel.PointLoad;

public record CreatePointLoadRequest
{
    public required int NodeId { get; init; }
    public required ForceContract Force { get; init; }
    public required Vector3 Direction { get; init; }
    public int? Id { get; init; }

    public CreatePointLoadRequest() { }

    [SetsRequiredMembers]
    public CreatePointLoadRequest(
        int nodeId,
        ForceContract force,
        Vector3 direction,
        int? id = null
    )
    {
        this.NodeId = nodeId;
        this.Force = force;
        this.Direction = direction;
        this.Id = id;
    }
}

public record PointLoadRequestData
{
    public required int NodeId { get; init; }
    public required ForceContract Force { get; init; }
    public required Vector3 Direction { get; init; }

    public PointLoadRequestData() { }

    [SetsRequiredMembers]
    public PointLoadRequestData(int nodeId, ForceContract force, Vector3 direction)
    {
        this.NodeId = nodeId;
        this.Force = force;
        this.Direction = direction;
    }
}

public record PutPointLoadRequest : PointLoadRequestData, IHasIntId
{
    public required int Id { get; init; }
}
