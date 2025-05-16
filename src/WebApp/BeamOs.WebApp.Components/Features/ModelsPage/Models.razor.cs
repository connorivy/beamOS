using BeamOs.CodeGen.StructuralAnalysisApiClient;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Models;
using BeamOs.WebApp.Components.Features.Dialogs;
using BeamOs.WebApp.Components.Pages;
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
    private NavigationManager NavigationManager { get; init; }

    [Inject]
    private IDialogService DialogService { get; init; }

    private string SearchTerm { get; set; } = string.Empty;

    private List<ModelInfoResponse> RecentlyModifiedModels = new List<ModelInfoResponse>();

    private List<ModelInfoResponse> FilteredModels =>
        this
            .ModelState.Value.UserModelResponses.Where(model =>
                model.Name.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase)
            )
            .ToList();

    protected override async Task OnInitializedAsync()
    {
        var authState = await this.AuthenticationStateTask;
        List<ModelInfoResponse>? userModels = null;
        if (authState?.User.Identity is not null && authState.User.Identity.IsAuthenticated)
        {
            var modelResponses = await this.StructuralAnalysisApiClient.GetModelsAsync();

            if (modelResponses.IsSuccess)
            {
                userModels = modelResponses.Value;
                this.Dispatcher.Dispatch(new UserModelsLoaded(modelResponses.Value));
            }
        }

        this.RecentlyModifiedModels =
        [
            .. userModels?.Take(4) ?? [],
            .. ModelPageState.SampleModelResponses.Take(4 - (userModels?.Count ?? 0)),
        ];

        this.Dispatcher.Dispatch(new ModelsDoneLoading());

        await base.OnInitializedAsync();
    }

    private void RedirectToLogin()
    {
        NavigationManager.NavigateTo("/login");
    }

    private Color GetBadgeColor(string role)
    {
        return role switch
        {
            "Owner" => Color.Primary,
            "Contributor" => Color.Secondary,
            "Reviewer" => Color.Info,
            "Sample" => Color.Warning,
            _ => Color.Default,
        };
    }

    private async Task ShowCreateModelDialog()
    {
        var options = new DialogOptions { CloseOnEscapeKey = true };

        var dialog = await this
            .DialogService.ShowAsync<CreateModelDialog>("Create Model", options)
            .ConfigureAwait(true);

        var result = await dialog.Result;

        if (!result.Canceled)
        {
            var modelResponse = result.Data as Result<ModelResponse>;
            if (modelResponse.IsSuccess)
            {
                NavigationManager.NavigateTo(ModelEditor.GetRelativeUrl(modelResponse.Value.Id));
            }
        }
    }
}
