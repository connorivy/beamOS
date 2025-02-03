using BeamOs.CodeGen.StructuralAnalysisApiClient;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Model;
using BeamOs.WebApp.Components.Features.Dialogs;
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

    private List<ModelInfoResponse> FilteredModels =>
        this.ModelState
            .Value
            .ModelResponses
            .Where(model => model.Name.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase))
            .ToList();

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
            _ => Color.Default
        };
    }

    private Task ShowCreateModelDialog()
    {
        var options = new DialogOptions { CloseOnEscapeKey = true };

        return this.DialogService.ShowAsync<CreateModelDialog>("Create Model", options);
    }
}
