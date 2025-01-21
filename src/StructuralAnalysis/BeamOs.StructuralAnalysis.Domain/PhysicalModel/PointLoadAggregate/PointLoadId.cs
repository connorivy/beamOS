using System.Globalization;
using BeamOs.Common.Domain.Models;

namespace BeamOs.StructuralAnalysis.Domain.PhysicalModel.PointLoadAggregate;

public readonly struct PointLoadId(int id) : IIntBasedId
{
    public int Id { get; } = id;

    public static explicit operator int(PointLoadId id) => id.Id;

    public static explicit operator PointLoadId(int id) => new(id);

    public override string ToString() => this.Id.ToString(CultureInfo.InvariantCulture);
}
