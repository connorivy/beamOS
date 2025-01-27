using System.Globalization;
using BeamOs.Common.Domain.Models;

namespace BeamOs.StructuralAnalysis.Domain.PhysicalModel.MaterialAggregate;

public readonly record struct MaterialId : IIntBasedId
{
    public int Id { get; init; }

    public MaterialId(int id)
    {
        this.Id = id;
    }

    public static implicit operator int(MaterialId id) => id.Id;

    public static implicit operator MaterialId(int id) => new(id);

    public override string ToString() => this.Id.ToString(CultureInfo.InvariantCulture);
}
