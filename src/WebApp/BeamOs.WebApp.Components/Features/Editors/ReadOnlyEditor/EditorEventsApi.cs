using BeamOs.CodeGen.EditorApi;
using BeamOs.WebApp.EditorCommands;
using Microsoft.JSInterop;

namespace BeamOs.WebApp.Components.Features.Editors.ReadOnlyEditor;

public class EditorEventsApi(
//MoveNodeCommandHandler moveNodeCommandHandler,
//ChangeSelectionCommandHandler changeSelectionCommandHandler
) : IEditorEventsApi
{
    [JSInvokable]
    public Task DispatchChangeSelectionCommandAsync(ChangeSelectionCommand body) =>
        Task.CompletedTask;

    //await changeSelectionCommandHandler.ExecuteAsync(body);

    public Task DispatchChangeSelectionCommandAsync(
        ChangeSelectionCommand body,
        CancellationToken cancellationToken
    ) => throw new NotImplementedException();

    [JSInvokable]
    public Task DispatchMoveNodeCommandAsync(MoveNodeCommand body) => Task.CompletedTask;

    //await moveNodeCommandHandler.ExecuteAsync(body);

    public Task DispatchMoveNodeCommandAsync(
        MoveNodeCommand body,
        CancellationToken cancellationToken
    ) => throw new NotImplementedException();
}
