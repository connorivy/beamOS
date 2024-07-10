using BeamOs.WebApp.Client.Events.Interfaces;

namespace BeamOs.WebApp.Client.Actions.EditorActions;

public record struct ChangeSelectionAction(string CanvasId, SelectedObject[] SelectedObjects)
    : IEditorAction;

public record SelectedObject(string Id, string TypeName);
