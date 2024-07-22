using BeamOs.Common.Events;
using BeamOs.IntegrationEvents;

namespace BeamOs.Api.Common;

public class DummyEventBus : IEventBus
{
    public Task PublishAsync(IIntegrationEvent integrationEvent) => Task.CompletedTask;
}
