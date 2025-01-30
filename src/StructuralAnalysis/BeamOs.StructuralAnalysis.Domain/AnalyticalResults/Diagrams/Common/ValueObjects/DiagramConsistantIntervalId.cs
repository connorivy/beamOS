using System.Globalization;
using BeamOs.Common.Domain.Models;

namespace BeamOs.StructuralAnalysis.Domain.AnalyticalResults.Diagrams.Common.ValueObjects;

public readonly record struct DiagramConsistantIntervalId : IIntBasedId
{
    public int Id { get; init; }

    public DiagramConsistantIntervalId(int id)
    {
        this.Id = id;
    }

    public static implicit operator int(DiagramConsistantIntervalId id) => id.Id;

    public static implicit operator DiagramConsistantIntervalId(int id) => new(id);

    public override string ToString() => this.Id.ToString(CultureInfo.InvariantCulture);
}
