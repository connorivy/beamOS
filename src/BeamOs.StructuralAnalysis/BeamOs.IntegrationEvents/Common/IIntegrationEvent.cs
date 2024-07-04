using System.Text.Json.Serialization;

namespace BeamOs.IntegrationEvents.Common;

public interface IIntegrationEvent
{
    public bool DbUpdated { get; init; }

    public string FullType { get; }
}
