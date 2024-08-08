using System.Text.Json;

namespace BeamOs.WebApp.Client.Components.State;

public readonly record struct IntegrationEventWithTypeName
{
#pragma warning disable IDE1006 // Naming Styles
    public required string typeFullName { get; init; }
#pragma warning restore IDE1006 // Naming Styles
    public required JsonElement IntegrationEvent { get; init; }
}
