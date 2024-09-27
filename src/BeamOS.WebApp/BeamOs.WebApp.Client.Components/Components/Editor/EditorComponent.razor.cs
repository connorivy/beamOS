using BeamOs.ApiClient;
using BeamOs.CodeGen.Apis.EditorApi;
using BeamOs.CodeGen.Apis.StructuralAnalysisApi;
using BeamOs.Common.Api;
using BeamOs.Common.Events;
using BeamOs.Contracts.AnalyticalModel;
using BeamOs.Contracts.AnalyticalModel.AnalyticalNode;
using BeamOs.Contracts.AnalyticalModel.Forces;
using BeamOs.Contracts.PhysicalModel.Common;
using BeamOs.WebApp.Client.Components.Components.Editor.CommandHandlers;
using BeamOs.WebApp.Client.Components.Components.Editor.Commands;
using BeamOs.WebApp.Client.Components.Features.Common.Flux;
using BeamOs.WebApp.Client.Components.Features.KeyBindings.UndoRedo;
using BeamOs.WebApp.Client.Components.Repositories;
using BeamOs.WebApp.Client.Components.State;
using BeamOs.WebApp.Client.EditorCommands;
using Fluxor;
using Fluxor.Blazor.Web.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.JSInterop;

namespace BeamOs.WebApp.Client.Components.Components.Editor;

public partial class EditorComponent : FluxorComponent
{
    private IDisposable? undoRedoFunctionality;

    [Parameter]
    public string? Class { get; set; }

    [Inject]
    private IStructuralAnalysisApiAlphaClient StructuralAnalysisApiClient { get; set; }

    [Inject]
    private IEditorApiProxyFactory EditorApiProxyFactory { get; init; }

    [Inject]
    private HistoryManager HistoryManager { get; init; }

    [Inject]
    private NavigationManager NavigationManager { get; init; }

    [Inject]
    private AddEntityContractToCacheCommandHandler AddEntityContractToCacheCommandHandler { get; init; }

    [Inject]
    private LoadModelByIdCommandHandler LoadModelCommandHandler { get; init; }

    [Inject]
    private IStateRepository<EditorComponentState> EditorComponentStateRepository { get; init; }

    [Inject]
    private ChangeComponentStateCommandHandler<EditorComponentState> ChangeComponentStateCommandHandler { get; init; }

    [Inject]
    private IDispatcher Dispatcher { get; init; }

    [Inject]
    private IJSRuntime JS { get; init; }

    public EditorComponentState EditorComponentState =>
        this.EditorComponentStateRepository.GetOrSetComponentStateByCanvasId(this.ElementId);

    public string ElementId { get; } = "id" + Guid.NewGuid().ToString("N");
    public IEditorApiAlpha? EditorApiAlpha { get; private set; }

    [Parameter]
    public string ModelId { get; set; } = "ddb1e60a-df17-48b0-810a-60e425acf640";

    private List<IIntegrationEvent> integrationEvents = [];

    protected override async Task OnInitializedAsync()
    {
        EventEmitter.VisibleStateChanged += this.EventEmitter_VisibleStateChanged;

        await base.OnInitializedAsync();
    }

    private void EventEmitter_VisibleStateChanged(object? sender, EventArgs _) =>
        this.InvokeAsync(this.StateHasChanged);

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            this.EditorApiAlpha ??= await this.EditorApiProxyFactory.Create(this.ElementId, false);

            await this.ChangeComponentState(state => state with { LoadingText = "Fetching Data" });
            await this.LoadModel(this.ModelId);
            await this.CacheAllNodeResults();
            await this.ChangeComponentState(state => state with { IsLoading = false });

            await JS.InvokeVoidAsync("initializeCanvasById", this.ElementId);
        }
        await base.OnAfterRenderAsync(firstRender);
    }

    protected override void OnParametersSet()
    {
        this.undoRedoFunctionality = UndoRedoFunctionality.SubscribeToUndoRedo(this.HistoryManager);
        base.OnParametersSet();
    }

    public async Task LoadModel(string modelId)
    {
        this.ModelId = modelId;
        await this.LoadModelCommandHandler.ExecuteAsync(
            new LoadModelCommand(this.ElementId, modelId)
        );
        this.Dispatcher.Dispatch(new ModelLoaded(modelId));
    }

    private async Task CacheAllNodeResults()
    {
        var allForces = await this.StructuralAnalysisApiClient.GetNodeResultsAsync(
            new(this.ModelId)
        );

        foreach (NodeResultResponse force in allForces)
        {
            await this.AddEntityContractToCacheCommandHandler.ExecuteAsync(
                new(this.ModelId, new NodeResultResponseEntity(force))
            );
        }
    }

    public record NodeResultResponseEntity : BeamOsEntityContractBase
    {
        public static string ResultId(string nodeId) => $"r{nodeId}";

        public ForcesResponse Forces { get; init; }
        public DisplacementsResponse Displacements { get; init; }

        public NodeResultResponseEntity(NodeResultResponse response)
            : base(ResultId(response.NodeId))
        {
            this.Forces = response.Forces;
            this.Displacements = response.Displacements;
        }
    }

    private bool EventIsServerResponseToUserInteraction(DbEvent integrationEvent)
    {
        for (int i = 0; i < this.integrationEvents.Count; i++)
        {
            if (this.integrationEvents[i].AlmostEquals(integrationEvent.IntegrationEvent))
            {
                this.integrationEvents.RemoveAt(i);
                return true;
            }
        }

        return false;
    }

    private async Task<Result> ChangeComponentState(
        Func<EditorComponentState, EditorComponentState> stateMutation
    )
    {
        return await this.ChangeComponentStateCommandHandler.ExecuteAsync(
            new(this.ElementId, stateMutation)
        );
    }

    private void OnMouseDown(MouseEventArgs args) { }

    protected override async ValueTask DisposeAsyncCore(bool disposing)
    {
        EventEmitter.VisibleStateChanged -= this.EventEmitter_VisibleStateChanged;

        if (this.EditorComponentState.EditorApi is IAsyncDisposable asyncDisposable)
        {
            await asyncDisposable.DisposeAsync();
        }

        this.EditorComponentStateRepository.RemoveEditorComponentStateForCanvasId(this.ElementId);
        //await this.EditorApiAlpha.DisposeAsync();
        this.undoRedoFunctionality?.Dispose();
        await base.DisposeAsyncCore(disposing);
    }
}

public record EditorComponentState(
    bool IsLoading,
    string LoadingText,
    string? CanvasId,
    string? LoadedModelId,
    IEditorApiAlpha? EditorApi,
    SelectedObject[] SelectedObjects,
    bool IsShowingOverlay
)
{
    public EditorComponentState()
        : this(true, "Loading beamOS editor", null, null, null, [], false) { }
}
