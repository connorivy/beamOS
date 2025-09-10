using System.Globalization;
using BeamOs.Common.Domain.Models;

namespace BeamOs.StructuralAnalysis.Domain.PhysicalModel.Element1dAggregate;

public readonly record struct Element1dId : IIntBasedId
{
    public int Id { get; init; }

    public Element1dId(int id)
    {
        this.Id = id;
    }

    public static implicit operator int(Element1dId id) => id.Id;

    public static implicit operator Element1dId(int id) => new(id);

    public override string ToString() => this.Id.ToString(CultureInfo.InvariantCulture);
}

public readonly record struct Element1dProposalId : IIntBasedId
{
    public int Id { get; init; }

    public Element1dProposalId(int id)
    {
        this.Id = id;
    }

    public static implicit operator int(Element1dProposalId id) => id.Id;

    public static implicit operator Element1dProposalId(int id) => new(id);

    public override string ToString() => this.Id.ToString(CultureInfo.InvariantCulture);
}
