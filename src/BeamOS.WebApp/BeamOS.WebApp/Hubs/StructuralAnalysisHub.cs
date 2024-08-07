using System.Text.Json;
using BeamOs.Common.Events;
using BeamOs.IntegrationEvents;
using BeamOs.WebApp.Client.Components.Components.Editor;
using BeamOs.WebApp.Client.Components.State;
using Microsoft.AspNetCore.SignalR;

namespace BeamOS.WebApp.Hubs;

public class StructuralAnalysisHub : Hub<IStructuralAnalysisHubClient> { }

public class StructuralAnalysisHubEventBus(
    IHubContext<StructuralAnalysisHub, IStructuralAnalysisHubClient> hubContext
) : IEventBus
{
    public async Task PublishAsync(IIntegrationEvent integrationEvent) =>
        await hubContext
            .Clients
            .All
            .StructuralAnalysisIntegrationEventFired(
                new IntegrationEventWithTypeName
                {
                    IntegrationEvent = JsonSerializer.SerializeToElement(
                        integrationEvent,
                        integrationEvent.GetType()
                    ),
                    typeFullName = integrationEvent.GetType().FullName
                }
            );
}
