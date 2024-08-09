using BeamOs.Common.Events;
using BeamOs.WebApp.Client.Components.State;

namespace BeamOs.WebApp.Client.Components.Components.Editor;

public interface IStructuralAnalysisHubClient
{
    public const string HubEndpointPattern = "/StructuralAnalysisHub";
    public Task StructuralAnalysisIntegrationEventFired(IIntegrationEvent integrationEvent);
    public Task StructuralAnalysisIntegrationEventFired(
        IntegrationEventWithTypeName integrationEventWithTypeName
    );
}
