using BeamOs.CodeGen.Apis.EditorApi;
using BeamOs.WebApp.Client.Components.Components.Editor.CommandHandlers;
using BeamOs.WebApp.Client.EditorCommands;
using Microsoft.JSInterop;

namespace BeamOs.WebApp.Client.Components;

public class EditorEventsApi(
    MoveNodeCommandHandler moveNodeCommandHandler,
    ChangeSelectionCommandHandler changeSelectionCommandHandler
) : IEditorEventsApi
{
    [JSInvokable]
    public async Task DispatchChangeSelectionCommandAsync(ChangeSelectionCommand body) =>
        await changeSelectionCommandHandler.ExecuteAsync(body);

    public Task DispatchChangeSelectionCommandAsync(
        ChangeSelectionCommand body,
        CancellationToken cancellationToken
    ) => throw new NotImplementedException();

    [JSInvokable]
    public async Task DispatchMoveNodeCommandAsync(MoveNodeCommand body) =>
        await moveNodeCommandHandler.ExecuteAsync(body);

    public Task DispatchMoveNodeCommandAsync(
        MoveNodeCommand body,
        CancellationToken cancellationToken
    ) => throw new NotImplementedException();
}
