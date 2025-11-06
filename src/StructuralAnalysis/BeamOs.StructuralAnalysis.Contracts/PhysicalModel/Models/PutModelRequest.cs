namespace BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Models;

public record PutModelRequest
{
    public List<Element1dByLocationRequest>? Element1dsToAddOrUpdateByExternalId { get; init; }
    public PatchOperationOptions Options { get; init; }
}

public record PutModelResponse
{
    public List<OperationStatus>? Element1dsToAddOrUpdateByExternalIdResults { get; init; }
}
