namespace BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Model;

public record CreateModelRequest
{
    public required string Name { get; init; }
    public required string Description { get; init; }
    public required PhysicalModelSettings Settings { get; init; }
    public Guid? Id { get; init; }
}
