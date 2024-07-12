using System.Text.Json;
using BeamOs.ApiClient;
using BeamOs.CodeGen.Apis.EditorApi;
using BeamOs.Common.Api;
using BeamOs.Common.Events;
using BeamOS.WebApp.Client.Components.Editor.CommandHandlers;
using BeamOS.WebApp.Client.Components.Editor.Commands;
using BeamOs.WebApp.Client.EditorCommands;
using BeamOS.WebApp.Client.Features.Common.Flux;
using BeamOS.WebApp.Client.Features.KeyBindings.UndoRedo;
using BeamOS.WebApp.Client.Repositories;
using BeamOS.WebApp.Client.State;
using Fluxor.Blazor.Web.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;

namespace BeamOS.WebApp.Client.Components.Editor;

public partial class EditorComponent : FluxorComponent
{
    [Parameter]
    public string? Class { get; set; }

    [Inject]
    private IApiAlphaClient ApiAlphaClient { get; init; }

    [Inject]
    private EditorApiProxyFactory EditorApiProxyFactory { get; init; }

    [Inject]
    private UndoRedoFunctionality UndoRedoFunctionality { get; init; }

    [Inject]
    private HistoryManager HistoryManager { get; init; }

    [Inject]
    private NavigationManager NavigationManager { get; init; }

    [Inject]
    private LoadModelCommandHandler LoadModelCommandHandler { get; init; }

    [Inject]
    private IStateRepository<EditorComponentState> EditorComponentStateRepository { get; init; }

    [Inject]
    private ChangeComponentStateCommandHandler<EditorComponentState> ChangeComponentStateCommandHandler { get; init; }

    public EditorComponentState EditorComponentState =>
        this.EditorComponentStateRepository.GetOrSetComponentStateByCanvasId(this.ElementId);

    public string ElementId { get; } = "id" + Guid.NewGuid().ToString("N");
    public IEditorApiAlpha? EditorApiAlpha { get; private set; }
    const string physicalModelId = "00000000-0000-0000-0000-000000000000";

    private HubConnection hubConnection;

    private List<IIntegrationEvent> integrationEvents = [];

    protected override async Task OnInitializedAsync()
    {
        EventEmitter.VisibleStateChanged += this.EventEmitter_VisibleStateChanged;
        this.hubConnection = new HubConnectionBuilder()
            .WithUrl(
                this.NavigationManager.ToAbsoluteUri(
                    IStructuralAnalysisHubClient.HubEndpointPattern
                )
            )
            .Build();

        this.hubConnection.On<IntegrationEventWithTypeName>(
            "StructuralAnalysisIntegrationEventFired",
            @event =>
            {
                Type? eventType = Type.GetType($"{@event.typeFullName},BeamOs.IntegrationEvents");
                var strongEvent = JsonSerializer.Deserialize(
                    @event.IntegrationEvent,
                    eventType,
                    new JsonSerializerOptions() { PropertyNameCaseInsensitive = true }
                );

                //this.Dispatcher.Dispatch(new DbEvent((IIntegrationEvent)strongEvent));
            }
        );

        await this.hubConnection.StartAsync();
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
            await this.LoadModel(physicalModelId);

            await this.ChangeComponentState(state => state with { IsLoading = false });
        }
        await base.OnAfterRenderAsync(firstRender);
    }

    public async Task LoadModel(string modelId)
    {
        await this.LoadModelCommandHandler.ExecuteAsync(
            new LoadModelCommand(this.ElementId, modelId)
        );
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

    protected override async ValueTask DisposeAsyncCore(bool disposing)
    {
        EventEmitter.VisibleStateChanged -= this.EventEmitter_VisibleStateChanged;

        if (this.EditorComponentState.EditorApi is IAsyncDisposable asyncDisposable)
        {
            await asyncDisposable.DisposeAsync();
        }

        this.EditorComponentStateRepository.RemoveEditorComponentStateForCanvasId(this.ElementId);
        //await this.EditorApiAlpha.DisposeAsync();
        this.UndoRedoFunctionality?.Dispose();
        await this.hubConnection.DisposeAsync();
        await base.DisposeAsyncCore(disposing);
    }
}

public record EditorComponentState(
    bool IsLoading,
    string LoadingText,
    string? LoadedModelId,
    IEditorApiAlpha? EditorApi,
    SelectedObject[] SelectedObjects
)
{
    public EditorComponentState()
        : this(true, "Loading beamOS editor", null, null, []) { }
}
