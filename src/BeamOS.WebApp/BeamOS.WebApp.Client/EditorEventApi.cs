using BeamOs.CodeGen.Apis.EditorApi;
using BeamOs.WebApp.EditorActionsAndEvents.Nodes;
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

    [JSInvokable]
    public Task HandleNodeMovedEventAsync(NodeMovedEvent body)
    {
        this.dispatcher.Dispatch(body);
        return Task.CompletedTask;
    }

    [JSInvokable]
    public Task HandleNodeMovedEventAsync(
        NodeMovedEvent body,
        CancellationToken cancellationToken
    ) => throw new NotImplementedException();
}
