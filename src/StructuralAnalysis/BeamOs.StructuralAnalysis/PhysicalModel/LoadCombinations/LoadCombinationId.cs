using System.Globalization;
using BeamOs.Common.Domain.Models;

namespace BeamOs.StructuralAnalysis.Domain.PhysicalModel.LoadCombinations;

public readonly record struct LoadCombinationId : IIntBasedId
{
    public int Id { get; init; }

    public LoadCombinationId(int id)
    {
        this.Id = id;
    }

    public static implicit operator int(LoadCombinationId id) => id.Id;

    public static implicit operator LoadCombinationId(int id) => new(id);

    public override string ToString() => this.Id.ToString(CultureInfo.InvariantCulture);
}
