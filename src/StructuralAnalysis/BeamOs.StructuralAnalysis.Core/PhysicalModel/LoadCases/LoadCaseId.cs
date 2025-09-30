using System.Globalization;
using BeamOs.Common.Domain.Models;

namespace BeamOs.StructuralAnalysis.Domain.PhysicalModel.LoadCases;

internal readonly record struct LoadCaseId : IIntBasedId
{
    public int Id { get; init; }

    public LoadCaseId(int id)
    {
        this.Id = id;
    }

    public static implicit operator int(LoadCaseId id) => id.Id;

    public static implicit operator LoadCaseId(int id) => new(id);

    public override string ToString() => this.Id.ToString(CultureInfo.InvariantCulture);
}
