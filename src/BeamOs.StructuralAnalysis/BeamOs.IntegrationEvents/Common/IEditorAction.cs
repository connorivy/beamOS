using System.Text.Json.Serialization;

namespace BeamOs.IntegrationEvents.Common;

public interface IEditorAction
{
    [JsonIgnore]
    public bool EditorUpdated { get; init; }
}
