using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Contracts.Common;

namespace BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Node;

public record NodeResponse(int Id, Guid ModelId, Point LocationPoint, Restraint Restraint)
    : IModelEntity;

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
    Error
}
