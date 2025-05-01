using System.Diagnostics.CodeAnalysis;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Contracts.Common;

namespace BeamOs.StructuralAnalysis.Contracts.PhysicalModel.PointLoad;

public record CreatePointLoadRequest
{
    public required int NodeId { get; init; }
    public required int LoadCaseId { get; init; }
    public required Force Force { get; init; }
    public required Vector3 Direction { get; init; }
    public int? Id { get; init; }

    public CreatePointLoadRequest() { }

    [SetsRequiredMembers]
    public CreatePointLoadRequest(
        int nodeId,
        int loadCaseId,
        Force force,
        Vector3 direction,
        int? id = null
    )
    {
        this.NodeId = nodeId;
        this.LoadCaseId = loadCaseId;
        this.Force = force;
        this.Direction = direction;
        this.Id = id;
    }
}

public record PointLoadData
{
    public required int NodeId { get; init; }
    public required int LoadCaseId { get; init; }
    public required Force Force { get; init; }
    public required Vector3 Direction { get; init; }

    public PointLoadData() { }

    [SetsRequiredMembers]
    public PointLoadData(int nodeId, int loadCaseId, Force force, Vector3 direction)
    {
        this.NodeId = nodeId;
        this.LoadCaseId = loadCaseId;
        this.Force = force;
        this.Direction = direction;
    }
}

public record PointLoad : PointLoadData, IHasIntId
{
    public required int Id { get; init; }

    public PointLoad() { }

    [SetsRequiredMembers]
    public PointLoad(int id, int nodeId, int loadCaseId, Force force, Vector3 direction)
    {
        this.Id = id;
        this.NodeId = nodeId;
        this.LoadCaseId = loadCaseId;
        this.Force = force;
        this.Direction = direction;
    }
}

public record PutPointLoadRequest : PointLoadData, IHasIntId
{
    public required int Id { get; init; }
}
