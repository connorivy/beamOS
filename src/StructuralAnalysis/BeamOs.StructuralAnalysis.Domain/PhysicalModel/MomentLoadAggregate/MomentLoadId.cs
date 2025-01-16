using System.Globalization;

namespace BeamOs.StructuralAnalysis.Domain.PhysicalModel.MomentLoadAggregate;

public readonly struct MomentLoadId(int id)
{
    public int Id { get; } = id;

    public static explicit operator int(MomentLoadId id) => id.Id;

    public static explicit operator MomentLoadId(int id) => new(id);

    public override string ToString() => this.Id.ToString(CultureInfo.InvariantCulture);
}
