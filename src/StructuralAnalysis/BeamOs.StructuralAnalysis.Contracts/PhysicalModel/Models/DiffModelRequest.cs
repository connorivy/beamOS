namespace BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Models;

public record DiffModelRequest
{
    public required Guid TargetModelId { get; init; }
}
