namespace BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelAggregate;

public readonly struct ModelId(Guid? id = null)
//: IEquatable<ModelId>, IEquatable<Guid>
{
    public Guid Id { get; } = id ?? Guid.NewGuid();

    public static implicit operator Guid(ModelId id) => id.Id;

    public static implicit operator ModelId(Guid id) => new(id);

    //public bool Equals(ModelId other) => other.Id == this.Id;

    //public override bool Equals(object? obj)
    //{
    //    return obj is ModelId id1 && this.Equals(id1);
    //}

    //public override int GetHashCode() => this.Id.GetHashCode();

    //public bool Equals(Guid other) => other == this.Id;

    //public static bool operator ==(ModelId p1, ModelId p2) => p1.Equals(p2);

    //public static bool operator !=(ModelId p1, ModelId p2) => !(p1 == p2);
}
