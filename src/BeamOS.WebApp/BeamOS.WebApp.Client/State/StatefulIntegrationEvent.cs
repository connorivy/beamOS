using BeamOs.IntegrationEvents.Common;

namespace BeamOS.WebApp.Client.State;

public readonly record struct StatefulIntegrationEvent
{
    public bool HistoryUpdated { get; init; }

    public bool EditorUpdated { get; init; }

    public bool DbUpdated { get; init; }

    public required IIntegrationEvent IntegrationEvent { get; init; }
}
