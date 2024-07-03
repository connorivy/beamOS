using BeamOs.IntegrationEvents;
using BeamOs.IntegrationEvents.Common;
using BeamOS.WebApp.Client.Components.Editor;
using Microsoft.AspNetCore.SignalR;

namespace BeamOS.WebApp.Hubs;

public class StructuralAnalysisHub : Hub<IStructuralAnalysisHubClient> { }

public class StructuralAnalysisHubEventBus(
    IHubContext<StructuralAnalysisHub, IStructuralAnalysisHubClient> hubContext
) : IEventBus
{
    public async Task PublishAsync(IIntegrationEvent integrationEvent) =>
        await hubContext.Clients.All.StructuralAnalysisIntegrationEventFired(integrationEvent);
}
