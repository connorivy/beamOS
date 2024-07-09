using BeamOs.Common.Events;

namespace BeamOs.Domain.Common.Interfaces;

public interface IHasIntegrationEvents
{
    public IReadOnlyList<IIntegrationEvent> IntegrationEvents { get; }

    public void ClearIntegrationEvents();
}
