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

public readonly record struct ModelChangeRequestId
{
    public Guid Id { get; }

    public ModelChangeRequestId()
        : this(null) { }

    public ModelChangeRequestId(Guid? id = null)
    {
        this.Id = id ?? Guid.NewGuid();
    }

    public static implicit operator Guid(ModelChangeRequestId id) => id.Id;

    public static implicit operator ModelChangeRequestId(Guid id) => new(id);
}
