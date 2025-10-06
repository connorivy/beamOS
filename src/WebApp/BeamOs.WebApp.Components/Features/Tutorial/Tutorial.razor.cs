using BeamOs.CodeGen.StructuralAnalysisApiClient;
using BeamOs.StructuralAnalysis;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Models;
using BeamOs.WebApp.Components.Features.Editor;
using BeamOs.WebApp.Components.Layout;
using BeamOs.WebApp.Components.Pages;
using Fluxor;
using Fluxor.Blazor.Web.Components;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace BeamOs.WebApp.Components.Features.Tutorial;

public partial class Tutorial(
    IState<TutorialState> tutorialState,
    BeamOsResultApiClient apiClient,
    IDialogService dialogService,
    IDispatcher dispatcher,
    IState<EditorComponentState> editorState
) : FluxorComponent
{
    private EditorComponent? editorComponent;

    public static CreateModelRequest DefaultCreateModelRequest =>
        new()
        {
            Name = "Tutorial Model",
            Description = "This model was created as part of the BeamOS tutorial.",
            Settings = new ModelSettings(UnitSettingsContract.K_IN),
        };

    protected override async Task OnInitializedAsync()
    {
        var createModelTask = apiClient.Models.CreateModelAsync(
            tutorialState.Value.TutorialModelRequest
        );
        await base.OnInitializedAsync();

        // Show welcome dialog
        var dialogParameters = new DialogParameters();
        var dialogOptions = new DialogOptions { CloseOnEscapeKey = true };

        await dialogService.ShowAsync<TutorialWelcomeDialog>(
            "Welcome to the BeamOS Tutorial",
            dialogParameters,
            dialogOptions
        );

        var modelResponse = await createModelTask;
    }
}
