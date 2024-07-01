using BeamOs.CodeGen.Apis.EditorApi;
using BeamOs.WebApp.EditorActionsAndEvents.Nodes;
using BeamOs.WebApp.EditorEvents;
using Fluxor;
using Microsoft.JSInterop;

namespace BeamOS.WebApp.Client;

public class EditorEventsApi : IEditorEventsApi
{
    private readonly IDispatcher dispatcher;

    public EditorEventsApi(IDispatcher dispatcher)
    {
        this.dispatcher = dispatcher;
    }

    private Task HandleAllEditorCommands<T>(T action)
        where T : struct, IEditorAction
    {
        this.dispatcher.Dispatch(action with { UiAlreadyUpdated = true });
        return Task.CompletedTask;
    }

    [JSInvokable]
    public Task HandleNodeMovedEventAsync(NodeMovedEvent body)
    {
        return this.HandleAllEditorCommands(body);
    }

    public Task HandleNodeMovedEventAsync(
        NodeMovedEvent body,
        CancellationToken cancellationToken
    ) => throw new NotImplementedException();
}
