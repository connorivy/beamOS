using System.Text.Json.Serialization;

namespace BeamOs.IntegrationEvents.Common;

public interface IUndoable : IIntegrationEvent
{
    [JsonIgnore]
    public bool HistoryUpdated { get; init; }
    public IUndoable GetUndoAction();
}
