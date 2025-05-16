using System.Diagnostics.CodeAnalysis;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Contracts.Common;

namespace BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Nodes;

public record NodeResponse : IModelEntity
{
    public NodeResponse() { }

    [SetsRequiredMembers]
    public NodeResponse(int id, Guid modelId, Point locationPoint, Restraint restraint)
    {
        this.Id = id;
        this.ModelId = modelId;
        this.LocationPoint = locationPoint;
        this.Restraint = restraint;
    }

    [SetsRequiredMembers]
    public NodeResponse(int id, Guid modelId, NodeData data)
        : this(id, modelId, data.LocationPoint, data.Restraint) { }

    public required int Id { get; init; }
    public required Guid ModelId { get; init; }
    public required Point LocationPoint { get; init; }
    public required Restraint Restraint { get; init; }

    public NodeData ToNodeData() => new(this.LocationPoint, this.Restraint);
}

public record BatchResponse
{
    public int Created { get; init; }
    public int Updated { get; init; }
    public int Deleted { get; init; }
    public int Errors { get; init; }

    public EntityStatus[] EntityStatuses { get; init; }
}

public record EntityStatus(
    int Id,
    EntityOperationStatus EntityOperationStatus,
    string ErrorMessage = null
);

public enum EntityOperationStatus
{
    Undefined = 0,
    Created,
    Updated,
    Deleted,
    Error,
}
