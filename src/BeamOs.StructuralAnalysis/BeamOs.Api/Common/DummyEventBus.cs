using BeamOs.IntegrationEvents;
using BeamOs.IntegrationEvents.Common;

namespace BeamOs.Api.Common;

public class DummyEventBus : IEventBus
{
    public Task PublishAsync(IIntegrationEvent integrationEvent) => Task.CompletedTask;
}
