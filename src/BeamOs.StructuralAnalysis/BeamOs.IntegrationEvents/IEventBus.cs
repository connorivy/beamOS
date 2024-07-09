using BeamOs.Common.Events;

namespace BeamOs.IntegrationEvents;

public interface IEventBus
{
    Task PublishAsync(IIntegrationEvent integrationEvent);
}
