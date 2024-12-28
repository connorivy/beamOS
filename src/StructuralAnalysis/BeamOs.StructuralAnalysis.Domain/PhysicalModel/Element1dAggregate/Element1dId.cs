namespace BeamOs.StructuralAnalysis.Domain.PhysicalModel.Element1dAggregate;

public readonly struct Element1dId(int id)
{
    public int Id { get; } = id;

    public static implicit operator int(Element1dId id) => id.Id;

    public static implicit operator Element1dId(int id) => new(id);
}
