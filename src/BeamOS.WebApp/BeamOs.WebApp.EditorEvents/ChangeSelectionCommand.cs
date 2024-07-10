using BeamOs.WebApp.Client.Events.Interfaces;

namespace BeamOs.WebApp.Client.EditorCommands;

public record struct ChangeSelectionCommand(string CanvasId, SelectedObject[] SelectedObjects)
    : IEditorCommand;

public record SelectedObject(string Id, string TypeName);
