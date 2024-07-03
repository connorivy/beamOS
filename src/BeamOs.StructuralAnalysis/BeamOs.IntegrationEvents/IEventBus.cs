using BeamOs.IntegrationEvents.Common;

namespace BeamOs.IntegrationEvents;

public interface IEventBus
{
    Task PublishAsync(IIntegrationEvent integrationEvent);
}
