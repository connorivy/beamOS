using System.Diagnostics.CodeAnalysis;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Contracts.Common;

namespace BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Nodes;

public record NodeDefinition(int Id);

public record NodeResponse : NodeData, IModelEntity
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

    public NodeData ToNodeData() => this;
}

// public record NodeModelResponse : NodeData, IHasIntId
// {
//     public NodeModelResponse() { }

//     [SetsRequiredMembers]
//     public NodeModelResponse(int id, Point locationPoint, Restraint restraint)
//     {
//         this.Id = id;
//         this.LocationPoint = locationPoint;
//         this.Restraint = restraint;
//     }

//     [SetsRequiredMembers]
//     public NodeModelResponse(int id, NodeData data)
//         : this(id, data.LocationPoint, data.Restraint) { }

//     public required int Id { get; init; }
// }

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
