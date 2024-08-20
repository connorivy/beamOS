namespace BeamOs.Common.Events;

public interface IHasDomainEvents
{
    public IReadOnlyList<IDomainEvent> DomainEvents { get; }

    public void AddEvent(IDomainEvent @event);
    public void ClearDomainEvents();
}
