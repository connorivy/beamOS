using System.Globalization;
using BeamOs.Common.Domain.Models;

namespace BeamOs.StructuralAnalysis.Domain.PhysicalModel.SectionProfileAggregate;

public readonly record struct SectionProfileFromLibraryId : IIntBasedId
{
    public int Id { get; init; }

    public SectionProfileFromLibraryId(int id)
    {
        this.Id = id;
    }

    public static implicit operator int(SectionProfileFromLibraryId id) => id.Id;

    public static implicit operator SectionProfileFromLibraryId(int id) => new(id);

    public override string ToString() => this.Id.ToString(CultureInfo.InvariantCulture);
}
