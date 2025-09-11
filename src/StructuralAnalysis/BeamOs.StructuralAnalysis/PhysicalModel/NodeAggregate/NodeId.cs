using System.Globalization;
using BeamOs.Common.Domain.Models;

namespace BeamOs.StructuralAnalysis.Domain.PhysicalModel.NodeAggregate;

internal readonly record struct NodeId : IIntBasedId
{
    public int Id { get; init; }

    public NodeId(int id)
    {
        this.Id = id;
    }

    public static implicit operator int(NodeId id) => id.Id;

    public static implicit operator NodeId(int id) => new(id);

    public override string ToString() => this.Id.ToString(CultureInfo.InvariantCulture);
}

internal readonly record struct NodeProposalId : IIntBasedId
{
    public int Id { get; init; }

    public NodeProposalId(int id)
    {
        this.Id = id;
    }

    public static implicit operator int(NodeProposalId id) => id.Id;

    public static implicit operator NodeProposalId(int id) => new(id);

    public override string ToString() => this.Id.ToString(CultureInfo.InvariantCulture);
}
