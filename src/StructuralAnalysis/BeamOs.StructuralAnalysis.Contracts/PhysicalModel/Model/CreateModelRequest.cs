namespace BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Model;

public record CreateModelRequest : ModelData
{
    public Guid? Id { get; init; }
}

public record ModelData
{
    public required string Name { get; init; }
    public required string Description { get; init; }
    public required ModelSettings Settings { get; init; }
}

public record Model : ModelData
{
    public required Guid Id { get; init; }
}
