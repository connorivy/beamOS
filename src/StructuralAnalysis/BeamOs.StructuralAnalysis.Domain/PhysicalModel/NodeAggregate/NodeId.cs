using System.Globalization;
using BeamOs.Common.Domain.Models;

namespace BeamOs.StructuralAnalysis.Domain.PhysicalModel.NodeAggregate;

public readonly record struct NodeId : IIntBasedId
{
    public int Id { get; }

    public NodeId(int id)
    {
        this.Id = id;
    }

    public static implicit operator int(NodeId id) => id.Id;

    public static implicit operator NodeId(int id) => new(id);

    public override string ToString() => this.Id.ToString(CultureInfo.InvariantCulture);
}
