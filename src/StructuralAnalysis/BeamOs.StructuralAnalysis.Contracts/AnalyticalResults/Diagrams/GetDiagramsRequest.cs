using BeamOs.Common.Contracts;

namespace BeamOs.StructuralAnalysis.Contracts.AnalyticalResults.Diagrams;

public record GetDiagramsRequest : IHasModelId
{
    [FromRoute]
    public Guid ModelId { get; init; }

    [FromRoute]
    public int Id { get; init; }

    [FromQuery]
    public string? UnitsOverride { get; init; }
}
