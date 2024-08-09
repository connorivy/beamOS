using BeamOs.Contracts.PhysicalModel.Model;
using BeamOs.WebApp.Client.Components.Components.Editor;
using BeamOs.WebApp.Client.Components.Components.Editor.CommandHandlers;
using BeamOs.WebApp.Client.Components.Repositories;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;

namespace BeamOs.WebApp.Client.Components.Features.Scratchpad;

public partial class Scratchpad : ComponentBase, IAsyncDisposable
{
    [Parameter]
    public required string Id { get; init; }

    [Inject]
    private NavigationManager NavigationManager { get; init; }

    [Inject]
    private AddEntityContractToEditorCommandHandler AddEntityContractToEditorCommandHandler { get; init; }

    [Inject]
    private IStateRepository<EditorComponentState> EditorComponentStateRepository { get; init; }

    public EditorComponentState EditorComponentState =>
        this.EditorComponentStateRepository.GetOrSetComponentStateByCanvasId(
            this.editorComponent.ElementId
        );

    private EditorComponent editorComponent;
    private HubConnection? hubConnection;

    protected override async Task OnInitializedAsync()
    {
        EventEmitter.VisibleStateChanged += this.EventEmitter_VisibleStateChanged;
        this.hubConnection = new HubConnectionBuilder()
            .WithUrl(this.NavigationManager.ToAbsoluteUri(IScratchpadHubClient.HubEndpointPattern))
            .Build();

        this.hubConnection.On<ModelResponse>(
            nameof(IScratchpadHubClient.LoadEntityInViewer),
            async entity =>
            {
                await this.editorComponent.EditorApiAlpha.ClearAsync();
                await this.AddEntityContractToEditorCommandHandler.ExecuteAsync(
                    new(this.editorComponent.ElementId, entity)
                );
            }
        );

        await this.hubConnection.StartAsync();
        await base.OnInitializedAsync();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);
        if (firstRender)
        {
            while (this.EditorComponentState.IsLoading)
            {
                await Task.Delay(100);
            }
            await this.editorComponent.EditorApiAlpha.ClearAsync();
        }
    }

    private void EventEmitter_VisibleStateChanged(object? sender, EventArgs _) =>
        this.InvokeAsync(this.StateHasChanged);

    public async ValueTask DisposeAsync()
    {
        await this.hubConnection.DisposeAsync();
        EventEmitter.VisibleStateChanged -= this.EventEmitter_VisibleStateChanged;
    }
}
