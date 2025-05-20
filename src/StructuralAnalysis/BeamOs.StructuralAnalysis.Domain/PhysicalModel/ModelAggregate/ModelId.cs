using System.Globalization;
using BeamOs.Common.Domain.Models;

namespace BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelAggregate;

public readonly record struct ModelId
{
    public Guid Id { get; }

    public ModelId()
        : this(null) { }

    public ModelId(Guid? id = null)
    {
        this.Id = id ?? Guid.NewGuid();
    }

    public static implicit operator Guid(ModelId id) => id.Id;

    public static implicit operator ModelId(Guid id) => new(id);
}

public readonly record struct ModelProposalId : IIntBasedId
{
    public int Id { get; init; }

    public ModelProposalId(int id)
    {
        this.Id = id;
    }

    public static implicit operator int(ModelProposalId id) => id.Id;

    public static implicit operator ModelProposalId(int id) => new(id);

    public override string ToString() => this.Id.ToString(CultureInfo.InvariantCulture);
}

public readonly record struct ProposalIssueId : IIntBasedId
{
    public int Id { get; init; }

    public ProposalIssueId(int id)
    {
        this.Id = id;
    }

    public static implicit operator int(ProposalIssueId id) => id.Id;

    public static implicit operator ProposalIssueId(int id) => new(id);

    public override string ToString() => this.Id.ToString(CultureInfo.InvariantCulture);
}
