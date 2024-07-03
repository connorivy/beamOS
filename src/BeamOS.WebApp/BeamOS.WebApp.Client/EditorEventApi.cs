using BeamOs.CodeGen.Apis.EditorApi;
using BeamOs.IntegrationEvents.Common;
using BeamOs.IntegrationEvents.PhysicalModel.Nodes;
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
        this.dispatcher.Dispatch(action with { EditorNeedsUpdating = false });
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
