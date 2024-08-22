using System.Collections.ObjectModel;
using BeamOs.ApiClient;
using BeamOs.Contracts.PhysicalModel.Model;
using Fluxor;
using Fluxor.Blazor.Web.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

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

    protected override async Task OnInitializedAsync()
    {
        var authState = await this.AuthenticationStateTask;
        var modelResponses = await this.ApiAlphaClient.GetModelsAsync();
        this.Dispatcher.Dispatch(
            new UserModelsLoaded(
                modelResponses as IReadOnlyCollection<ModelResponse>
                    ?? new ReadOnlyCollection<ModelResponse>([.. modelResponses])
            )
        );
        await base.OnInitializedAsync();
    }
}
