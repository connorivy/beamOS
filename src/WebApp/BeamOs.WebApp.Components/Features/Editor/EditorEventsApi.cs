using BeamOs.CodeGen.EditorApi;
using BeamOs.Common.Contracts;
using BeamOs.WebApp.EditorCommands;
using Fluxor;
using Microsoft.JSInterop;

namespace BeamOs.WebApp.Components.Features.Editor;

public class EditorEventsApi(
    //MoveNodeCommandHandler moveNodeCommandHandler,
    //ChangeSelectionCommandHandler changeSelectionCommandHandler
    IDispatcher dispatcher
) : IEditorEventsApi
{
    [JSInvokable]
    public Task DispatchChangeSelectionCommandAsync(ChangeSelectionCommand body)
    {
        dispatcher.Dispatch(body);
        return Task.CompletedTask;
    }

    //await changeSelectionCommandHandler.ExecuteAsync(body);

    public Task DispatchChangeSelectionCommandAsync(
        ChangeSelectionCommand body,
        CancellationToken cancellationToken
    ) => throw new NotImplementedException();

    [JSInvokable]
    public Task DispatchMoveNodeCommandAsync(MoveNodeCommand body)
    {
        dispatcher.Dispatch(body);
        return Task.CompletedTask;
    }

    //await moveNodeCommandHandler.ExecuteAsync(body);

    public Task DispatchMoveNodeCommandAsync(
        MoveNodeCommand body,
        CancellationToken cancellationToken
    ) => throw new NotImplementedException();
}
