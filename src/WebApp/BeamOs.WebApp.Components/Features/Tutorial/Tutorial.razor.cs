using BeamOs.CodeGen.StructuralAnalysisApiClient;
using BeamOs.StructuralAnalysis;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Models;
using BeamOs.WebApp.Components.Features.Editor;
using BeamOs.WebApp.Components.Layout;
using BeamOs.WebApp.Components.Pages;
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
    private EditorComponent? editorComponent;
    private bool hasShownDialogForCurrentNavigation = false;

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

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);

        // Show welcome dialog on every render if it hasn't been shown yet for this navigation
        if (!hasShownDialogForCurrentNavigation)
        {
            hasShownDialogForCurrentNavigation = true;

            var dialogParameters = new DialogParameters();
            var dialogOptions = new DialogOptions { CloseOnEscapeKey = true, CloseButton = true };

            await dialogService.ShowAsync<TutorialWelcomeDialog>(
                "Welcome to the BeamOS Tutorial",
                dialogParameters,
                dialogOptions
            );
        }
    }

    private void OnLocationChanged(object? sender, LocationChangedEventArgs e)
    {
        // Reset the dialog flag when navigating to this page
        hasShownDialogForCurrentNavigation = false;
    }

    public void Dispose()
    {
        navigationManager.LocationChanged -= OnLocationChanged;
        GC.SuppressFinalize(this);
    }
}
