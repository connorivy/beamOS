namespace BeamOs.StructuralAnalysis.Domain.PhysicalModel.NodeAggregate;

public readonly struct NodeId(int id)
{
    public int Id { get; } = id;

    public static implicit operator int(NodeId id) => id.Id;

    public static implicit operator NodeId(int id) => new(id);
}
