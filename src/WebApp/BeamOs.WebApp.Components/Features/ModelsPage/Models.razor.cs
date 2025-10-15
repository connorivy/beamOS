using BeamOs.CodeGen.StructuralAnalysisApiClient;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Models;
using BeamOs.WebApp.Components.Features.Dialogs;
using BeamOs.WebApp.Components.Features.Tutorial;
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
    private IState<TutorialState> TutorialState { get; set; }

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
            .ModelState.Value.UserModelResponses
#if DEBUG
            .Where(model => model.Name != "Tutorial Model") // Hide tutorial model in debug mode
#endif
            .Where(model => model.Name.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase))
            .OrderByDescending(m => m.LastModified)
            .ToList();

    protected override async Task OnInitializedAsync()
    {
        var authState = await this.AuthenticationStateTask;
        ICollection<ModelInfoResponse>? userModels = null;
        if (authState?.User.Identity is not null && authState.User.Identity.IsAuthenticated)
        {
            var modelResponses = await this.StructuralAnalysisApiClient.GetModelsAsync();

            if (modelResponses.IsSuccess)
            {
                userModels = modelResponses.Value;
                this.Dispatcher.Dispatch(new UserModelsLoaded([.. modelResponses.Value]));
            }
        }

        this.RecentlyModifiedModels = [.. userModels?.Take(4) ?? []];

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

    private string GetModelUrl(ModelInfoResponse model)
    {
        // Tutorial model navigates to /tutorial page
        if (model.Id == this.TutorialState.Value.ModelId)
        {
            return "/tutorial";
        }
        return ModelEditor.GetRelativeUrl(model.Id);
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
            var modelResponse =
                (result.Data as ApiResponse<ModelResponse>)
                ?? throw new InvalidOperationException(
                    $"Invalid dialog result of type {result.Data?.GetType()}"
                );
            if (modelResponse.IsSuccess)
            {
                NavigationManager.NavigateTo(ModelEditor.GetRelativeUrl(modelResponse.Value.Id));
            }
        }
    }
}
