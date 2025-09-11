using System.Globalization;
using BeamOs.Common.Domain.Models;

namespace BeamOs.StructuralAnalysis.Domain.PhysicalModel.MaterialAggregate;

internal readonly record struct MaterialId : IIntBasedId
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

internal readonly record struct MaterialProposalId : IIntBasedId
{
    public int Id { get; init; }

    public MaterialProposalId(int id)
    {
        this.Id = id;
    }

    public static implicit operator int(MaterialProposalId id) => id.Id;

    public static implicit operator MaterialProposalId(int id) => new(id);

    public override string ToString() => this.Id.ToString(CultureInfo.InvariantCulture);
}
