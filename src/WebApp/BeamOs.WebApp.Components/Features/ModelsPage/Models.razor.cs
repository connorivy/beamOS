using BeamOs.CodeGen.StructuralAnalysisApiClient;
using Fluxor;
using Fluxor.Blazor.Web.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;

namespace BeamOs.WebApp.Components.Features.ModelsPage;

public partial class Models : FluxorComponent
{
    [CascadingParameter]
    public Task<AuthenticationState> AuthenticationStateTask { get; set; }

    [Inject]
    private IStructuralAnalysisApiClientV1 StructuralAnalysisApiClient { get; set; }

    [Inject]
    private IState<ModelPageState> ModelState { get; set; }

    [Inject]
    private IDispatcher Dispatcher { get; init; }

    [Inject]
    private IDialogService DialogService { get; init; }

    protected override async Task OnInitializedAsync()
    {
        var authState = await this.AuthenticationStateTask;
        if (authState?.User.Identity is not null && authState.User.Identity.IsAuthenticated)
        {
            var modelResponses = await this.StructuralAnalysisApiClient.GetModelsAsync();

            if (modelResponses.IsSuccess)
            {
                this.Dispatcher.Dispatch(new UserModelsLoaded(modelResponses.Value));
            }
        }

        await base.OnInitializedAsync();
    }

    private Task CreateModelDialog()
    {
        var options = new DialogOptions { CloseOnEscapeKey = true };

        return this.DialogService.ShowAsync<CreateModelDialog>("Create Model", options);
    }
}
