namespace BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Models;

public record CreateModelRequest : ModelInfoData
{
    public Guid? Id { get; init; }
    public CreateModelOptions Options { get; init; } = new();
}

public record ModelInfoData
{
    public required string Name { get; init; }
    public required string Description { get; init; }
    public required ModelSettings Settings { get; init; }
}

public record ModelInfo : ModelInfoData
{
    public required Guid Id { get; init; }
    public required DateTimeOffset LastModified { get; init; }
}

public record CreateModelOptions
{
    public bool IsTempModel { get; init; }
}
