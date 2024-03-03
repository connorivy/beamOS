using BeamOs.Domain.Common.Interfaces;

namespace BeamOs.Domain.Common.Models;

public abstract class BeamOSEntity<TId> : IEquatable<BeamOSEntity<TId>>, IBeamOsDomainObject
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

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    protected BeamOSEntity() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
}
