using System.Text.Json.Serialization;

namespace BeamOs.WebApp.EditorEvents;

public interface IEditorAction
{
    [JsonIgnore]
    public bool UiAlreadyUpdated { get; init; }
}
