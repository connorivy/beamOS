using BeamOs.Common.Events;

namespace BeamOs.Common.Domain.Interfaces;

public interface IHasIntegrationEvents
{
    public IReadOnlyList<IIntegrationEvent> IntegrationEvents { get; }

    public void ClearIntegrationEvents();
}
