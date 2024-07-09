using BeamOs.CodeGen.Apis.EditorApi;
using BeamOs.WebApp.Client.Actions.EditorActions;
using BeamOS.WebApp.Client.Components.Editor.CommandHandlers;
using BeamOs.WebApp.Client.Events.Interfaces;
using Fluxor;
using Microsoft.JSInterop;

namespace BeamOS.WebApp.Client;

public class EditorEventsApi(IDispatcher dispatcher, MoveNodeCommandHandler moveNodeCommandHandler)
    : IEditorEventsApi
{
    private Task DispatchAllEditorCommands(IClientAction action)
    {
        if (action is IClientActionWithSource actionWithSource)
        {
            dispatcher.Dispatch(actionWithSource.WithSource(ClientActionSource.Editor));
        }
        else
        {
            dispatcher.Dispatch(action);
        }
        return Task.CompletedTask;
    }

    [JSInvokable]
    public Task DispatchChangeSelectionActionAsync(ChangeSelectionAction body) =>
        this.DispatchAllEditorCommands(body);

    public Task DispatchChangeSelectionActionAsync(
        ChangeSelectionAction body,
        CancellationToken cancellationToken
    ) => throw new NotImplementedException();

    [JSInvokable]
    public async Task DispatchMoveNodeActionAsync(MoveNodeAction body) =>
        await moveNodeCommandHandler.ExecuteAsync(body);

    public Task DispatchMoveNodeActionAsync(
        MoveNodeAction body,
        CancellationToken cancellationToken
    ) => throw new NotImplementedException();
}
