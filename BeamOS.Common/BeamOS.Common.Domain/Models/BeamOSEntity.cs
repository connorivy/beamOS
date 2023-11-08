namespace BeamOS.Common.Domain.Models;
public abstract class BeamOSEntity<TId> : IEquatable<BeamOSEntity<TId>>
    where TId : notnull
{
    public TId Id { get; protected set; }

    protected BeamOSEntity(TId id)
    {
        this.Id = id;
    }

    public override bool Equals(object? obj)
    {
        return obj is BeamOSEntity<TId> entity && this.Id.Equals(entity.Id);
    }

    public static bool operator ==(BeamOSEntity<TId> left, BeamOSEntity<TId> right)
    {
        return Equals(left, right);
    }
    public static bool operator !=(BeamOSEntity<TId> left, BeamOSEntity<TId> right)
    {
        return !Equals(left, right);
    }

    public override int GetHashCode()
    {
        return this.Id.GetHashCode();
    }

    public bool Equals(BeamOSEntity<TId>? other)
    {
        return this.Equals((object?)other);
    }
}
