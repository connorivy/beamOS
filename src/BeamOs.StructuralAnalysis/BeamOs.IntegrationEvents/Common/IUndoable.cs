using System.Text.Json.Serialization;

namespace BeamOs.IntegrationEvents.Common;

public interface IUndoable : IIntegrationEvent
{
    [JsonIgnore]
    public bool HistoryNeedsUpdating { get; init; }
    public IUndoable GetUndoAction();
}
