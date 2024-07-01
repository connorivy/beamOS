using System.Text.Json.Serialization;

namespace BeamOs.WebApp.EditorEvents;

public interface IUndoable
{
    [JsonIgnore]
    public bool IsUndoAction { get; init; }

    [JsonIgnore]
    public bool IsRedoAction { get; init; }
    public IUndoable GetUndoAction();
}
