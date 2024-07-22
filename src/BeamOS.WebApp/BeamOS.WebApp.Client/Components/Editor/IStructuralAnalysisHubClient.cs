using BeamOs.Common.Events;
using BeamOS.WebApp.Client.State;

namespace BeamOS.WebApp.Client.Components.Editor;

public interface IStructuralAnalysisHubClient
{
    public const string HubEndpointPattern = "/StructuralAnalysisHub";
    public Task StructuralAnalysisIntegrationEventFired(IIntegrationEvent integrationEvent);
    public Task StructuralAnalysisIntegrationEventFired(
        IntegrationEventWithTypeName integrationEventWithTypeName
    );
}
