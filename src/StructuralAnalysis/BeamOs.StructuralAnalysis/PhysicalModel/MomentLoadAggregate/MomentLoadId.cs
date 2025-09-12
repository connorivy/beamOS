using System.Globalization;
using BeamOs.Common.Domain.Models;

namespace BeamOs.StructuralAnalysis.Domain.PhysicalModel.MomentLoadAggregate;

internal readonly record struct MomentLoadId : IIntBasedId
{
    public int Id { get; init; }

    public MomentLoadId(int id)
    {
        this.Id = id;
    }

    public static implicit operator int(MomentLoadId id) => id.Id;

    public static implicit operator MomentLoadId(int id) => new(id);

    public override string ToString() => this.Id.ToString(CultureInfo.InvariantCulture);
}
