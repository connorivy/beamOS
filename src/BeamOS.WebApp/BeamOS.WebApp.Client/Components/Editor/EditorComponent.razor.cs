using System.Text.Json;
using BeamOs.ApiClient;
using BeamOs.CodeGen.Apis.EditorApi;
using BeamOs.Contracts.Common;
using BeamOs.Contracts.PhysicalModel.Model;
using BeamOs.IntegrationEvents.Common;
using BeamOs.IntegrationEvents.PhysicalModel.Nodes;
using BeamOS.WebApp.Client.Features.KeyBindings.UndoRedo;
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
    private IDispatcher Dispatcher { get; init; }

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

                this.Dispatcher.Dispatch(
                    new StatefulIntegrationEvent()
                    {
                        IntegrationEvent = (IIntegrationEvent)strongEvent,
                        DbUpdated = true
                    }
                );
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
            // SubscribeToEditorActions(EditorApiAlpha);

            this.SubscribeToAction<StatefulIntegrationEvent>(
                async e => await this.HandleStatefulIntegrationEvent(e)
            );

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
        ModelResponseHydrated response = await this.ApiAlphaClient.GetModelHydratedAsync(
            modelId,
            PreconfiguredUnits.N_M
        );
        var x = await this.EditorApiAlpha.CreateModelHydratedAsync(response);
    }

    private async Task HandleStatefulIntegrationEvent(
        StatefulIntegrationEvent statefulIntegrationEvent
    )
    {
        if (this.EventIsServerResponseToUserInteraction(statefulIntegrationEvent))
        {
            return;
        }

        // if db is not updated, then the event originated from the client, not the server
        if (!statefulIntegrationEvent.DbUpdated)
        {
            this.integrationEvents.Add(statefulIntegrationEvent.IntegrationEvent);
        }

        switch (statefulIntegrationEvent.IntegrationEvent)
        {
            case NodeMovedEvent nodeMovedEvent:
                if (!statefulIntegrationEvent.EditorUpdated)
                {
                    await this.EditorApiAlpha.ReduceNodeMovedEventAsync(nodeMovedEvent);
                }
                if (!statefulIntegrationEvent.DbUpdated)
                {
                    await this.ApiAlphaClient.PatchNodeAsync(
                        new()
                        {
                            NodeId = nodeMovedEvent.NodeId.ToString(),
                            LocationPoint = new()
                            {
                                LengthUnit = "Meter",
                                XCoordinate = nodeMovedEvent.NewLocation.X,
                                YCoordinate = nodeMovedEvent.NewLocation.Y,
                                ZCoordinate = nodeMovedEvent.NewLocation.Z
                            }
                        },
                        nodeMovedEvent.NodeId.ToString()
                    );
                }
                break;
            default:
                break;
        }
    }

    private bool EventIsServerResponseToUserInteraction(
        StatefulIntegrationEvent statefulIntegrationEvent
    )
    {
        if (!statefulIntegrationEvent.DbUpdated)
        {
            return false;
        }

        for (int i = 0; i < this.integrationEvents.Count; i++)
        {
            // compare with .Equals() because we need to compare with the value object comparison
            if (this.integrationEvents[i].Equals(statefulIntegrationEvent.IntegrationEvent))
            {
                this.integrationEvents.RemoveAt(i);
                return true;
            }
        }

        return false;
    }

    protected override async ValueTask DisposeAsyncCore(bool disposing)
    {
        this.UndoRedoFunctionality?.Dispose();
        await this.hubConnection.DisposeAsync();
        await base.DisposeAsyncCore(disposing);
    }

    [FeatureState]
    public record EditorComponentState(
        bool IsLoading,
        string LoadingText,
        ModelResponse? visibleModel
    )
    {
        private EditorComponentState()
            : this(true, "Loading beamOS editor", null) { }
    }
}
