using BeamOs.CodeGen.Apis.EditorApi;
using BeamOs.Common.Api;
using BeamOs.Common.Events;
using BeamOs.WebApp.Client.Components.Components.Editor;
using BeamOs.WebApp.Client.Components.Components.Editor.CommandHandlers;
using BeamOs.WebApp.Client.Components.Components.Editor.Commands;
using BeamOs.WebApp.Client.Components.Features.TestExplorer;
using BeamOs.WebApp.Client.Components.Features.TestExplorer.Components;
using BeamOs.WebApp.Client.Components.Repositories;
using BeamOs.WebApp.Client.EditorCommands;
using Fluxor;
using Fluxor.Blazor.Web.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;

namespace BeamOs.WebApp.Client.Components.Features.Editors.ReadOnlyEditor;

public partial class ReadOnlyEditor : FluxorComponent
{
    [Parameter]
    public string? Class { get; set; }

    [Parameter]
    public string CanvasId { get; init; } = CreateCanvasId();

    protected bool IsReadOnly { get; } = true;

    [Inject]
    private IEditorApiProxyFactory EditorApiProxyFactory { get; init; }

    [Inject]
    protected LoadModelByIdCommandHandler LoadModelCommandHandler { get; init; }

    [Inject]
    protected IStateRepository<EditorComponentState> EditorComponentStateRepository { get; init; }

    [Inject]
    protected ChangeComponentStateCommandHandler<EditorComponentState> ChangeComponentStateCommandHandler { get; init; }

    public EditorComponentState EditorComponentState =>
        this.EditorComponentStateRepository.GetOrSetComponentStateByCanvasId(this.CanvasId);

    [Inject]
    private IDispatcher Dispatcher { get; init; }

    public static string CreateCanvasId() => "id" + Guid.NewGuid().ToString("N");

    public IEditorApiAlpha? EditorApiAlpha { get; private set; }
    const string physicalModelId = "00000000-0000-0000-0000-000000000000";

    private HubConnection hubConnection;

    private List<IIntegrationEvent> integrationEvents = [];

    protected override void OnInitialized()
    {
        EventEmitter.VisibleStateChanged += this.EventEmitter_VisibleStateChanged;

        this.SubscribeToAction<ChangeSelectionCommand>(async c =>
        {
            if (c.CanvasId != this.CanvasId)
            {
                return;
            }
            await this.ChangeComponentState(
                state => state with { SelectedObjects = c.SelectedObjects }
            );
        });

        this.SubscribeToAction<SetColorFilterCommand>(async command =>
        {
            if (command.CanvasId != this.CanvasId)
            {
                return;
            }

            await this.EditorApiAlpha.SetColorFilterAsync(command.Command);
        });

        this.SubscribeToAction<RemoveColorFilterCommand>(async command =>
        {
            if (command.CanvasId != this.CanvasId)
            {
                return;
            }

            await this.EditorApiAlpha.ClearFiltersAsync(command.Command);
        });

        base.OnInitialized();
    }

    private void EventEmitter_VisibleStateChanged(object? sender, EventArgs _) =>
        this.InvokeAsync(this.StateHasChanged);

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            this.EditorApiAlpha ??= await this.EditorApiProxyFactory.Create(
                this.CanvasId,
                this.IsReadOnly
            );

            await this.ChangeComponentState(state => state with { LoadingText = "Fetching Data" });
            await this.LoadModel(physicalModelId);

            await this.ChangeComponentState(state => state with { IsLoading = false });
        }
        await base.OnAfterRenderAsync(firstRender);
    }

    public async Task LoadModel(string modelId)
    {
        this.Dispatcher.Dispatch(new ChangeSelectionCommand(this.CanvasId, []));
        await this.LoadModelCommandHandler.ExecuteAsync(
            new LoadModelCommand(this.CanvasId, modelId)
        );
    }

    protected async Task<Result> ChangeComponentState(
        Func<EditorComponentState, EditorComponentState> stateMutation
    )
    {
        return await this.ChangeComponentStateCommandHandler.ExecuteAsync(
            new(this.CanvasId, stateMutation)
        );
    }

    protected override async ValueTask DisposeAsyncCore(bool disposing)
    {
        EventEmitter.VisibleStateChanged -= this.EventEmitter_VisibleStateChanged;
        if (this.EditorComponentState.EditorApi is IAsyncDisposable asyncDisposable)
        {
            await asyncDisposable.DisposeAsync();
        }

        base.DisposeAsyncCore(disposing);
    }
}
