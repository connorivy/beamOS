namespace BeamOs.StructuralAnalysis.Domain.PhysicalModel.Element1dAggregate;

public readonly record struct Element1dId
{
    public int Id { get; }

    public Element1dId(int id)
    {
        this.Id = id;
    }

    public static implicit operator int(Element1dId id) => id.Id;

    public static implicit operator Element1dId(int id) => new(id);
}
