using System.Text.Json.Serialization;

namespace BeamOs.IntegrationEvents.Common;

public interface IEditorAction
{
    [JsonIgnore]
    public bool EditorNeedsUpdating { get; init; }
}
