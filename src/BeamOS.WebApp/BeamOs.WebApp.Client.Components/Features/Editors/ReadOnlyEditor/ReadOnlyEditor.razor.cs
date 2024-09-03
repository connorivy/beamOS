using BeamOs.CodeGen.Apis.EditorApi;
using BeamOs.Common.Api;
using BeamOs.Common.Events;
using BeamOs.WebApp.Client.Components.Components.Editor;
using BeamOs.WebApp.Client.Components.Components.Editor.CommandHandlers;
using BeamOs.WebApp.Client.Components.Components.Editor.Commands;
using BeamOs.WebApp.Client.Components.Repositories;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;

namespace BeamOs.WebApp.Client.Components.Features.Editors.ReadOnlyEditor;

public partial class ReadOnlyEditor : ComponentBase, IAsyncDisposable
{
    [Parameter]
    public string? Class { get; set; }

    [Parameter]
    public string CanvasId { get; init; } = GetCanvasId();

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

    public static string GetCanvasId() => "id" + Guid.NewGuid().ToString("N");

    public IEditorApiAlpha? EditorApiAlpha { get; private set; }
    const string physicalModelId = "00000000-0000-0000-0000-000000000000";

    private HubConnection hubConnection;

    private List<IIntegrationEvent> integrationEvents = [];

    protected override void OnInitialized()
    {
        EventEmitter.VisibleStateChanged += this.EventEmitter_VisibleStateChanged;
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

    protected async ValueTask DisposeAsyncCore(bool disposing)
    {
        EventEmitter.VisibleStateChanged -= this.EventEmitter_VisibleStateChanged;

        if (this.EditorComponentState.EditorApi is IAsyncDisposable asyncDisposable)
        {
            await asyncDisposable.DisposeAsync();
        }

        this.EditorComponentStateRepository.RemoveEditorComponentStateForCanvasId(this.CanvasId);
    }

    public async ValueTask DisposeAsync()
    {
        await this.DisposeAsyncCore(true);
    }
}
