using System.Globalization;
using BeamOs.Common.Domain.Models;

namespace BeamOs.StructuralAnalysis.Domain.AnalyticalResults.Diagrams.MomentDiagramAggregate.ValueObjects;

public readonly record struct MomentDiagramId : IIntBasedId
{
    public int Id { get; init; }

    public MomentDiagramId(int id)
    {
        this.Id = id;
    }

    public static implicit operator int(MomentDiagramId id) => id.Id;

    public static implicit operator MomentDiagramId(int id) => new(id);

    public override string ToString() => this.Id.ToString(CultureInfo.InvariantCulture);
}
