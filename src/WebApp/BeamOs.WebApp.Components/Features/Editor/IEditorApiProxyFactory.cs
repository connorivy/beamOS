using BeamOs.CodeGen.EditorApi;
//using BeamOs.WebApp.Components.Features.Editor;
using Microsoft.JSInterop;

namespace BeamOs.WebApp.Components.Features.Editor;

public interface IEditorApiProxyFactory
{
    Task<IEditorApiAlpha> Create(string canvasId, bool isReadOnly);
}

public class EditorApiProxyFactory(
    IJSRuntime js,
    EditorEventsApi editorEventsApi
//ChangeComponentStateCommandHandler<EditorComponentState> changeComponentStateCommandHandler
) : IEditorApiProxyFactory
{
    public async Task<IEditorApiAlpha> Create(string canvasId, bool isReadOnly)
    {
        return await EditorApiProxy.Create(
            js,
            editorEventsApi,
            //changeComponentStateCommandHandler,
            canvasId,
            true // for now, disable editor based model edits
        );
    }
}

//public class EditorEventsApi(
//    MoveNodeCommandHandler moveNodeCommandHandler,
//    ChangeSelectionCommandHandler changeSelectionCommandHandler
//) : IEditorEventsApi
//{
//    [JSInvokable]
//    public async Task DispatchChangeSelectionCommandAsync(ChangeSelectionCommand body) =>
//        await changeSelectionCommandHandler.ExecuteAsync(body);

//    public Task DispatchChangeSelectionCommandAsync(
//        ChangeSelectionCommand body,
//        CancellationToken cancellationToken
//    ) => throw new NotImplementedException();

//    [JSInvokable]
//    public async Task DispatchMoveNodeCommandAsync(MoveNodeCommand body) =>
//        await moveNodeCommandHandler.ExecuteAsync(body);

//    public Task DispatchMoveNodeCommandAsync(
//        MoveNodeCommand body,
//        CancellationToken cancellationToken
//    ) => throw new NotImplementedException();
//}
