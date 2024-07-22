using BeamOs.WebApp.Client.Events.Interfaces;

namespace BeamOS.WebApp.Client.Components.Editor.Commands;

public record struct ChangeComponentStateCommand(
    string CanvasId,
    EditorComponentState NewEditorComponentState
) : IClientCommand;

public record struct ChangeComponentStateCommand<TState>(
    string CanvasId,
    Func<TState, TState> StateMutation
) : IClientCommand;
