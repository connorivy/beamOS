using System.Collections.ObjectModel;
using BeamOs.ApiClient;
using BeamOs.Contracts.PhysicalModel.Model;
using BeamOs.WebApp.Client.Components.Features.Common.Dialogs;
using Fluxor;
using Fluxor.Blazor.Web.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;

namespace BeamOs.WebApp.Client.Components.Features.ModelsPage;

public partial class Models : FluxorComponent
{
    [CascadingParameter]
    public Task<AuthenticationState> AuthenticationStateTask { get; set; }

    [Inject]
    private IApiAlphaClient ApiAlphaClient { get; set; }

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
            var modelResponses = await this.ApiAlphaClient.GetModelsAsync();
            this.Dispatcher.Dispatch(
                new UserModelsLoaded(
                    modelResponses as IReadOnlyCollection<ModelResponse>
                        ?? new ReadOnlyCollection<ModelResponse>([.. modelResponses])
                )
            );
        }

        await base.OnInitializedAsync();
    }
}
