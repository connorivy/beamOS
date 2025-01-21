using System.Collections.Immutable;
using BeamOs.CodeGen.EditorApi;
using BeamOs.WebApp.Components.Layout;
using Fluxor;
using Fluxor.Blazor.Web.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;

namespace BeamOs.WebApp.Components.Features.Editors.ReadOnlyEditor;

public partial class ReadOnlyEditor(
    IEditorApiProxyFactory editorApiProxyFactory,
    IState<AllEditorComponentState> allEditorComponentState,
    IStateSelection<AllEditorComponentState, EditorComponentState> state,
    IDispatcher dispatcher,
    LoadModelCommandHandler loadModelCommandHandler,
    ILogger<ReadOnlyEditor> logger
) : FluxorComponent
{
    [Parameter]
    public string? Class { get; set; }

    [Parameter]
    public string CanvasId { get; init; } = CreateCanvasId();

    [Parameter]
    public Guid? ModelId { get; set; }

    protected bool IsReadOnly { get; } = true;

    //[Inject]
    //protected LoadModelByIdCommandHandler LoadModelCommandHandler { get; init; }

    //[Inject]
    //protected IStateRepository<EditorComponentState> EditorComponentStateRepository { get; init; }

    //[Inject]
    //protected ChangeComponentStateCommandHandler<EditorComponentState> ChangeComponentStateCommandHandler { get; init; }

    //public EditorComponentState EditorComponentState =>
    //    this.EditorComponentStateRepository.GetOrSetComponentStateByCanvasId(this.CanvasId);

    //[Inject]
    //private IDispatcher Dispatcher { get; init; }

    public static string CreateCanvasId() => "id" + Guid.NewGuid().ToString("N");

    private IEditorApiAlpha? EditorApiAlpha;

    //const string physicalModelId = "00000000-0000-0000-0000-000000000000";

    //private HubConnection hubConnection;

    //private List<IIntegrationEvent> integrationEvents = [];

    //protected override void OnInitialized()
    //{
    //    EventEmitter.VisibleStateChanged += this.EventEmitter_VisibleStateChanged;

    //    this.SubscribeToAction<ChangeSelectionCommand>(async c =>
    //    {
    //        if (c.CanvasId != this.CanvasId)
    //        {
    //            return;
    //        }
    //        await this.ChangeComponentState(
    //            state => state with { SelectedObjects = c.SelectedObjects }
    //        );
    //    });

    //    this.SubscribeToAction<SetColorFilterCommand>(async command =>
    //    {
    //        if (command.CanvasId != this.CanvasId)
    //        {
    //            return;
    //        }

    //        await this.EditorApiAlpha.SetColorFilterAsync(command.Command);
    //    });

    //    this.SubscribeToAction<RemoveColorFilterCommand>(async command =>
    //    {
    //        if (command.CanvasId != this.CanvasId)
    //        {
    //            return;
    //        }

    //        await this.EditorApiAlpha.ClearFiltersAsync(command.Command);
    //    });

    //    base.OnInitialized();
    //}

    //private void EventEmitter_VisibleStateChanged(object? sender, EventArgs _) =>
    //    this.InvokeAsync(this.StateHasChanged);

    protected override void OnInitialized()
    {
        base.OnInitialized();
        dispatcher.Dispatch(new EditorCreated(this.CanvasId));
        state.Select(s => s.EditorState[this.CanvasId]);
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            this.EditorApiAlpha ??= await editorApiProxyFactory.Create(
                this.CanvasId,
                this.IsReadOnly
            );
            dispatcher.Dispatch(new EditorApiCreated(this.CanvasId, this.EditorApiAlpha));

            if (this.ModelId is not null)
            {
                dispatcher.Dispatch(new EditorLoadingBegin(this.CanvasId, "Fetching Data"));
                var result = await loadModelCommandHandler.ExecuteAsync(
                    new LoadModelCommand(this.CanvasId, this.ModelId.Value)
                );
                dispatcher.Dispatch(new EditorLoadingEnd(this.CanvasId));

                if (result.IsError)
                {
                    logger.LogError(result.Error.ToString());
                }
            }
            //await this.ChangeComponentState(state => state with { LoadingText = "Fetching Data" });
            //await this.LoadModel(physicalModelId);

            //await this.ChangeComponentState(state => state with { IsLoading = false });
        }
        await base.OnAfterRenderAsync(firstRender);
    }

    //protected async Task<Result> ChangeComponentState(
    //    Func<EditorComponentState, EditorComponentState> stateMutation
    //)
    //{
    //    return await this.ChangeComponentStateCommandHandler.ExecuteAsync(
    //        new(this.CanvasId, stateMutation)
    //    );
    //}

    //protected override async ValueTask DisposeAsyncCore(bool disposing)
    //{
    //    EventEmitter.VisibleStateChanged -= this.EventEmitter_VisibleStateChanged;
    //    if (this.EditorComponentState.EditorApi is IAsyncDisposable asyncDisposable)
    //    {
    //        await asyncDisposable.DisposeAsync();
    //    }

    //    base.DisposeAsyncCore(disposing);
    //}

    protected override async ValueTask DisposeAsyncCore(bool disposing)
    {
        await base.DisposeAsyncCore(disposing);
        if (state.Value.EditorApi is IAsyncDisposable disposable)
        {
            await disposable.DisposeAsync();
        }
        dispatcher.Dispatch(new EditorDisposed(this.CanvasId));
    }
}

public record struct EditorApiCreated(string CanvasId, IEditorApiAlpha EditorApiAlpha);

public record struct EditorCreated(string CanvasId);

public record struct EditorDisposed(string CanvasId);

public record struct EditorLoadingBegin(string CanvasId, string LoadingText);

public record struct EditorLoadingEnd(string CanvasId);

[FeatureState]
public record EditorComponentState(string? LoadingText, bool IsLoading, IEditorApiAlpha? EditorApi)
{
    public EditorComponentState()
        : this(null, false, null) { }
}

//public static class EditorComponentStateReducers
//{
//    [ReducerMethod]
//    public static EditorComponentState EditorCreatedReducer(
//        EditorComponentState state,
//        EditorApiCreated action
//    )
//    {
//        return state with { EditorApi = action.EditorApiAlpha };
//    }
//}

[FeatureState]
public record AllEditorComponentState(ImmutableDictionary<string, EditorComponentState> EditorState)
{
    public AllEditorComponentState()
        : this(ImmutableDictionary<string, EditorComponentState>.Empty) { }
}

public static class AllEditorComponentStateReducers
{
    [ReducerMethod]
    public static AllEditorComponentState EditorCreatedReducer(
        AllEditorComponentState state,
        EditorCreated action
    )
    {
        return state with { EditorState = state.EditorState.Add(action.CanvasId, new()) };
    }

    [ReducerMethod]
    public static AllEditorComponentState EditorDisposedReducer(
        AllEditorComponentState state,
        EditorDisposed action
    )
    {
        return state with { EditorState = state.EditorState.Remove(action.CanvasId) };
    }

    [ReducerMethod]
    public static AllEditorComponentState EditorCreatedReducer(
        AllEditorComponentState state,
        EditorApiCreated action
    )
    {
        var currentEditorState = state.EditorState[action.CanvasId];
        return state with
        {
            EditorState = state
                .EditorState
                .Remove(action.CanvasId)
                .Add(action.CanvasId, currentEditorState with { EditorApi = action.EditorApiAlpha })
        };
    }

    [ReducerMethod]
    public static AllEditorComponentState EditorLoadingBeginReducer(
        AllEditorComponentState state,
        EditorLoadingBegin action
    )
    {
        var currentEditorState = state.EditorState[action.CanvasId];
        return state with
        {
            EditorState = state
                .EditorState
                .Remove(action.CanvasId)
                .Add(
                    action.CanvasId,
                    currentEditorState with
                    {
                        IsLoading = true,
                        LoadingText = action.LoadingText
                    }
                )
        };
    }

    [ReducerMethod]
    public static AllEditorComponentState EditorLoadingBeginReducer(
        AllEditorComponentState state,
        EditorLoadingEnd action
    )
    {
        var currentEditorState = state.EditorState[action.CanvasId];
        return state with
        {
            EditorState = state
                .EditorState
                .Remove(action.CanvasId)
                .Add(action.CanvasId, currentEditorState with { IsLoading = false })
        };
    }
}
