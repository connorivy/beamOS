using System.Globalization;
using BeamOs.Common.Domain.Models;

namespace BeamOs.StructuralAnalysis.Domain.PhysicalModel.MomentLoadAggregate;

public readonly struct MomentLoadId(int id) : IIntBasedId
{
    public int Id { get; } = id;

    public static explicit operator int(MomentLoadId id) => id.Id;

    public static explicit operator MomentLoadId(int id) => new(id);

    public override string ToString() => this.Id.ToString(CultureInfo.InvariantCulture);
}
