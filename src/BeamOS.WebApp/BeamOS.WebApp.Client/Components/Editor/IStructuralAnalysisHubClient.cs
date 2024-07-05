using BeamOs.IntegrationEvents.Common;
using BeamOS.WebApp.Client.State;

namespace BeamOS.WebApp.Client.Components.Editor;

public interface IStructuralAnalysisHubClient
{
    public const string HubEndpointPattern = "/StructuralAnalysisHub";
    public Task StructuralAnalysisIntegrationEventFired(IIntegrationEvent integrationEvent);
    public Task StructuralAnalysisIntegrationEventFired(
        StatefulIntegrationEvent statefulIntegrationEventntegrationEvent
    );
    public Task StructuralAnalysisIntegrationEventFired(
        IntegrationEventWithTypeName integrationEventWithTypeName
    );
}