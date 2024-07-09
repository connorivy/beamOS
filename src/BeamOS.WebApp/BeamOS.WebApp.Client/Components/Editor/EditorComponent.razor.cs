using System.Collections.Immutable;
using System.Text.Json;
using BeamOs.ApiClient;
using BeamOs.CodeGen.Apis.EditorApi;
using BeamOs.Common.Events;
using BeamOs.Contracts.Common;
using BeamOs.Contracts.PhysicalModel.Element1d;
using BeamOs.Contracts.PhysicalModel.Model;
using BeamOs.Contracts.PhysicalModel.Node;
using BeamOs.IntegrationEvents;
using BeamOs.IntegrationEvents.PhysicalModel.Nodes;
using BeamOs.WebApp.Client.Actions.EditorActions;
using BeamOS.WebApp.Client.Components.Editor.CommandHandlers;
using BeamOS.WebApp.Client.Components.Editor.Commands;
using BeamOS.WebApp.Client.Components.Editor.Flux.Actions;
using BeamOS.WebApp.Client.Components.Editor.Flux.Events;
using BeamOs.WebApp.Client.Events.Interfaces;
using BeamOS.WebApp.Client.Features.Common.Flux;
using BeamOS.WebApp.Client.Features.KeyBindings.UndoRedo;
using BeamOS.WebApp.Client.Repositories;
using BeamOS.WebApp.Client.State;
using Fluxor;
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

    private bool isLoading = true;
    private string loadingText = "Loading beamOS editor";
    public string elementId { get; } = "id" + Guid.NewGuid().ToString("N");
    public IEditorApiAlpha? EditorApiAlpha { get; private set; }
    const string physicalModelId = "00000000-0000-0000-0000-000000000000";

    private HubConnection hubConnection;

    private List<IIntegrationEvent> integrationEvents = [];

    protected override async Task OnInitializedAsync()
    {
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

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            this.isLoading = true;
            this.EditorApiAlpha ??= await this.EditorApiProxyFactory.Create(this.elementId);

            this.loadingText = "Fetching Data";
            this.StateHasChanged();
            await this.LoadModel(physicalModelId);
            this.isLoading = false;
            this.StateHasChanged();
        }
        await base.OnAfterRenderAsync(firstRender);
    }

    public async Task LoadModel(string modelId)
    {
        await this.LoadModelCommandHandler.ExecuteAsync(
            new LoadModelCommand(this.elementId, modelId)
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

    protected override async ValueTask DisposeAsyncCore(bool disposing)
    {
        await this.EditorApiAlpha.DisposeAsync();
        this.UndoRedoFunctionality?.Dispose();
        await this.hubConnection.DisposeAsync();
        await base.DisposeAsyncCore(disposing);
    }

    [FeatureState]
    public record EditorComponentState(
        bool IsLoading,
        string LoadingText,
        string? LoadedModelId,
        SelectedObject[] SelectedObjects,
        Dictionary<string, NodeResponse> NodeIdToResponsesDict,
        Dictionary<string, Element1DResponse> Element1dIdToResponsesDict
    )
    {
        private EditorComponentState()
            : this(true, "Loading beamOS editor", null, [], [], []) { }
    }

    [FeatureState]
    public record EditorComponentStates(
        ImmutableDictionary<string, EditorComponentState> CanvasIdToEditorComponentStates
    )
    {
        private EditorComponentStates()
            : this(ImmutableDictionary<string, EditorComponentState>.Empty) { }
    }
}
