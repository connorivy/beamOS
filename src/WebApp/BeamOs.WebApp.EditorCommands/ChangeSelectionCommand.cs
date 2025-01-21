using BeamOs.WebApp.EditorCommands.Interfaces;

namespace BeamOs.WebApp.EditorCommands;

public record struct ChangeSelectionCommand(string CanvasId, SelectedObject[] SelectedObjects)
    : IEditorCommand;

public record SelectedObject(int Id, string TypeName);
