global using ModelId = System.Guid;
using System.Globalization;
using BeamOs.Common.Domain.Models;

namespace BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelAggregate;

// internal readonly record struct ModelId
// {
//     public Guid Id { get; }

//     public ModelId()
//         : this(null) { }

//     public ModelId(Guid? id = null)
//     {
//         this.Id = id ?? Guid.CreateVersion7();
//     }

//     public static implicit operator Guid(ModelId id) => id.Id;

//     public static implicit operator ModelId(Guid id) => new(id);
// }

internal readonly record struct ModelProposalId : IIntBasedId
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

internal readonly record struct ProposalIssueId : IIntBasedId
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

internal readonly record struct ModelEntityDeleteProposalId : IIntBasedId
{
    public int Id { get; init; }

    public ModelEntityDeleteProposalId(int id)
    {
        this.Id = id;
    }

    public static implicit operator int(ModelEntityDeleteProposalId id) => id.Id;

    public static implicit operator ModelEntityDeleteProposalId(int id) => new(id);

    public override string ToString() => this.Id.ToString(CultureInfo.InvariantCulture);
}
