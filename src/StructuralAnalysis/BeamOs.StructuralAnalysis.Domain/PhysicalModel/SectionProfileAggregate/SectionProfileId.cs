using System.Globalization;
using BeamOs.Common.Domain.Models;

namespace BeamOs.StructuralAnalysis.Domain.PhysicalModel.SectionProfileAggregate;

public readonly record struct SectionProfileId : IIntBasedId
{
    public int Id { get; init; }

    public SectionProfileId(int id)
    {
        this.Id = id;
    }

    public static implicit operator int(SectionProfileId id) => id.Id;

    public static implicit operator SectionProfileId(int id) => new(id);

    public override string ToString() => this.Id.ToString(CultureInfo.InvariantCulture);
}
