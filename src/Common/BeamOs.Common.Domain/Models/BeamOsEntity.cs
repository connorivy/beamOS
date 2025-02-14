namespace BeamOs.Common.Domain.Models;

public abstract class BeamOsEntity<TId>
    : IEquatable<BeamOsEntity<TId>>,
        IBeamOsDomainObject,
        IHasDomainEvents
    where TId : struct
{
    public virtual TId Id { get; protected set; }

    protected BeamOsEntity(TId id)
    {
        this.Id = id;
    }

    private readonly List<IDomainEvent> domainEvents = [];
    public IReadOnlyList<IDomainEvent> DomainEvents => this.domainEvents.AsReadOnly();

    // todo : make nullable
    // https://github.com/dotnet/efcore/issues/31376
    //public CustomData CustomData { get; set; } = new();

    public void AddEvent(IDomainEvent @event) => this.domainEvents.Add(@event);

    public void ClearDomainEvents() => this.domainEvents.Clear();

    public override bool Equals(object? obj)
    {
        return obj is BeamOsEntity<TId> entity && this.Id.Equals(entity.Id);
    }

    public static bool operator ==(BeamOsEntity<TId> left, BeamOsEntity<TId> right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(BeamOsEntity<TId> left, BeamOsEntity<TId> right)
    {
        return !Equals(left, right);
    }

    public override int GetHashCode()
    {
        return this.Id.GetHashCode();
    }

    public bool Equals(BeamOsEntity<TId>? other)
    {
        return this.Equals((object?)other);
    }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    protected BeamOsEntity() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
}

public interface IHasDomainEvents
{
    public IReadOnlyList<IDomainEvent> DomainEvents { get; }

    public void AddEvent(IDomainEvent @event);
    public void ClearDomainEvents();
}

public interface IDomainEvent { }
