using System.Text.Json.Serialization;

namespace BeamOs.IntegrationEvents.Common;

public interface IIntegrationEvent
{
    [JsonIgnore]
    public bool DbNeedsUpdating { get; init; }
}
