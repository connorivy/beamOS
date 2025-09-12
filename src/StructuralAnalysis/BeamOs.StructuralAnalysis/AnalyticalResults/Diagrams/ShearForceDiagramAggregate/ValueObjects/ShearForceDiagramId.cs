using System.Globalization;
using BeamOs.Common.Domain.Models;

namespace BeamOs.StructuralAnalysis.Domain.AnalyticalResults.Diagrams.ShearForceDiagramAggregate.ValueObjects;

internal readonly record struct ShearForceDiagramId : IIntBasedId
{
    public int Id { get; init; }

    public ShearForceDiagramId(int id)
    {
        this.Id = id;
    }

    public static implicit operator int(ShearForceDiagramId id) => id.Id;

    public static implicit operator ShearForceDiagramId(int id) => new(id);

    public override string ToString() => this.Id.ToString(CultureInfo.InvariantCulture);
}
