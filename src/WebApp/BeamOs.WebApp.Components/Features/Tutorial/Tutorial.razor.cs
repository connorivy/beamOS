using BeamOs.StructuralAnalysis;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Models;
using BeamOs.WebApp.Components.Features.Editor;
using Fluxor;
using Fluxor.Blazor.Web.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using MudBlazor;

namespace BeamOs.WebApp.Components.Features.Tutorial;

public partial class Tutorial(
    IState<TutorialState> tutorialState,
    BeamOsResultApiClient apiClient,
    IDialogService dialogService,
    IDispatcher dispatcher,
    IState<EditorComponentState> editorState,
    NavigationManager navigationManager
) : FluxorComponent, IDisposable
{
    private const string TutorialRoute = "/tutorial";
    private EditorComponent? editorComponent;
    private bool hasShownDialogForCurrentNavigation;

    public static CreateModelRequest DefaultCreateModelRequest =>
        new()
        {
            Name = "Tutorial Model",
            Description = "This model was created as part of the BeamOS tutorial.",
            Settings = new ModelSettings(UnitSettingsContract.K_IN),
        };

    protected override async Task OnInitializedAsync()
    {
        var createModelTask = apiClient.Models.Temp.CreateTempModelAsync(
            tutorialState.Value.TutorialModelRequest
        );
        await base.OnInitializedAsync();

        // Subscribe to location changes to reset dialog state
        navigationManager.LocationChanged += OnLocationChanged;

        var modelResponse = await createModelTask;
    }

    private Task? ShowDialogTask;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        Console.WriteLine(
            $"Tutorial OnAfterRenderAsync called. firstRender: {firstRender}, hasShownDialogForCurrentNavigation: {hasShownDialogForCurrentNavigation}"
        );
        await base.OnAfterRenderAsync(firstRender);

        // Show welcome dialog only on first render after navigation to this page
        if (firstRender)
        {
            hasShownDialogForCurrentNavigation = true;

            var dialogParameters = new DialogParameters();
            var dialogOptions = new DialogOptions
            {
                BackdropClick = false,
                CloseOnEscapeKey = true,
                CloseButton = true,
            };

            ShowDialogTask = dialogService.ShowAsync<TutorialWelcomeDialog>(
                null,
                dialogParameters,
                dialogOptions
            );
        }
        if (ShowDialogTask is not null)
        {
            await ShowDialogTask;
        }
    }

    private void OnLocationChanged(object? sender, LocationChangedEventArgs e)
    {
        // Reset the dialog flag only when navigating to the tutorial page
        if (
            Uri.TryCreate(e.Location, UriKind.Absolute, out var uri)
            && uri.AbsolutePath == TutorialRoute
        )
        {
            hasShownDialogForCurrentNavigation = false;
        }
    }

    public void Dispose()
    {
        navigationManager.LocationChanged -= OnLocationChanged;
        GC.SuppressFinalize(this);
    }
}
