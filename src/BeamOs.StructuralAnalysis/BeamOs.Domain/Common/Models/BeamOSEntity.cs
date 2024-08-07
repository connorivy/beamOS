using BeamOs.Common.Events;
using BeamOs.Domain.Common.Interfaces;

namespace BeamOs.Domain.Common.Models;

public abstract class BeamOSEntity<TId>
    : IEquatable<BeamOSEntity<TId>>,
        IBeamOsDomainObject,
        IHasIntegrationEvents
    where TId : notnull
{
    public TId Id { get; private set; }

    protected BeamOSEntity(TId id)
    {
        this.Id = id;
    }

    private readonly List<IIntegrationEvent> integrationEvents = [];
    public IReadOnlyList<IIntegrationEvent> IntegrationEvents =>
        this.integrationEvents.AsReadOnly();

    public void AddEvent(IIntegrationEvent @event) => this.integrationEvents.Add(@event);

    public void ClearIntegrationEvents() => this.integrationEvents.Clear();

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
