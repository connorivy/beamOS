using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Contracts.Common;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Element1ds;

namespace BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Models;

public record PatchModelRequest
{
    public List<Element1dByLocationRequest>? Element1dsToAddOrUpdateByExternalId { get; init; }
    public PatchOperationOptions Options { get; init; }
}

public record PatchModelResponse
{
    public List<OperationStatus>? Element1dsToAddOrUpdateByExternalIdResults { get; init; }
}

public record Element1dByLocationRequest
{
    public required string ExternalId { get; init; }
    public required Point StartNodeLocation { get; init; }
    public required Point EndNodeLocation { get; init; }
}

public record OperationStatus
{
    public required BeamOsObjectType ObjectType { get; init; }
    public string? ExternalId { get; init; }
    public int? Id { get; init; }
    public required Result Result { get; init; }
}

public record PatchOperationOptions
{
    public ResolutionStrategy NodeResolutionStrategy { get; init; } = ResolutionStrategy.Error;
    public ResolutionStrategy Element1dResolutionStrategy { get; init; } = ResolutionStrategy.Error;
}

public enum ResolutionStrategy : byte
{
    Undefined = 0,
    Error,
    OverwriteExisting,
    KeepExisting,
}
