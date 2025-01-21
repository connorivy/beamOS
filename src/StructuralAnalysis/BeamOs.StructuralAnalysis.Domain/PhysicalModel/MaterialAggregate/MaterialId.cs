using System.Globalization;
using BeamOs.Common.Domain.Models;

namespace BeamOs.StructuralAnalysis.Domain.PhysicalModel.MaterialAggregate;

public readonly struct MaterialId(int id) : IIntBasedId
{
    public int Id { get; } = id;

    public static implicit operator int(MaterialId id) => id.Id;

    public static implicit operator MaterialId(int id) => new(id);

    public override string ToString() => this.Id.ToString(CultureInfo.InvariantCulture);
}
