using System.Globalization;
using BeamOs.Common.Domain.Models;

namespace BeamOs.StructuralAnalysis.Domain.PhysicalModel.PointLoadAggregate;

internal readonly record struct PointLoadId : IIntBasedId
{
    public int Id { get; init; }

    public PointLoadId(int id)
    {
        this.Id = id;
    }

    public static implicit operator int(PointLoadId id) => id.Id;

    public static implicit operator PointLoadId(int id) => new(id);

    public override string ToString() => this.Id.ToString(CultureInfo.InvariantCulture);
}
