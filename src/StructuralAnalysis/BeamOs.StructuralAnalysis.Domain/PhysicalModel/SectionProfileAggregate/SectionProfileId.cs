using System.Globalization;
using BeamOs.Common.Domain.Models;

namespace BeamOs.StructuralAnalysis.Domain.PhysicalModel.SectionProfileAggregate;

public readonly struct SectionProfileId(int id) : IIntBasedId
{
    public int Id { get; } = id;

    public static explicit operator int(SectionProfileId id) => id.Id;

    public static explicit operator SectionProfileId(int id) => new(id);

    public override string ToString() => this.Id.ToString(CultureInfo.InvariantCulture);
}
