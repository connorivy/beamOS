using BeamOs.IntegrationEvents.Common;

namespace BeamOS.WebApp.Client.Components.Editor;

public interface IStructuralAnalysisHubClient
{
    public const string HubEndpointPattern = "/StructuralAnalysisHub";
    public Task StructuralAnalysisIntegrationEventFired(IIntegrationEvent integrationEvent);
}
