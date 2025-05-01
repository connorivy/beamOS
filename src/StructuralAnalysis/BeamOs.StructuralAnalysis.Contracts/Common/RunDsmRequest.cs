namespace BeamOs.StructuralAnalysis.Contracts.Common;

public record RunDsmRequest
{
    public string? UnitsOverride { get; init; }

    public List<int>? LoadCombinationIds { get; init; }
}
