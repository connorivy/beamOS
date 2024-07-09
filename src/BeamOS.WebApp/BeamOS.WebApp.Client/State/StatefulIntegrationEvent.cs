using System.Text.Json;
using BeamOs.Common.Events;

namespace BeamOS.WebApp.Client.State;

public readonly record struct StatefulIntegrationEvent
{
    public bool HistoryUpdated { get; init; }

    public bool EditorUpdated { get; init; }

    public bool DbUpdated { get; init; }

    public required IIntegrationEvent IntegrationEvent { get; init; }
}

public readonly record struct IntegrationEventWithTypeName
{
#pragma warning disable IDE1006 // Naming Styles
    public required string typeFullName { get; init; }
#pragma warning restore IDE1006 // Naming Styles
    public required JsonElement IntegrationEvent { get; init; }
}
