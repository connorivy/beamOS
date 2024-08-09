using BeamOs.WebApp.Client.Components.Components.Editor;
using BeamOs.WebApp.Client.Events.Interfaces;

namespace BeamOs.WebApp.Client.Components.Components.Editor.Commands;

public record struct ChangeComponentStateCommand(
    string CanvasId,
    EditorComponentState NewEditorComponentState
) : IClientCommand;

public record struct ChangeComponentStateCommand<TState>(
    string CanvasId,
    Func<TState, TState> StateMutation
) : IClientCommand;
