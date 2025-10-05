using BeamOs.CodeGen.StructuralAnalysisApiClient;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Models;
using BeamOs.WebApp.Components.Features.Editor;
using BeamOs.WebApp.Components.Layout;
using Fluxor;
using Fluxor.Blazor.Web.Components;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace BeamOs.WebApp.Components.Features.Tutorial;

public partial class Tutorial : FluxorComponent
{
    private Guid TutorialModelId { get; set; }

    [Inject]
    private IStructuralAnalysisApiClientV1 StructuralAnalysisApiClient { get; set; }

    [Inject]
    private IDialogService DialogService { get; set; }

    [Inject]
    private IDispatcher dispatcher { get; set; }

    private EditorComponent? editorComponent;

    protected override void OnInitialized()
    {
        base.OnInitialized();
        dispatcher.Dispatch(new OpenDrawer());
        TutorialModelId = Guid.CreateVersion7();
    }

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        // Create the tutorial model
        var createModelRequest = new CreateModelRequest
        {
            Id = TutorialModelId,
            Name = "Tutorial",
            Description = "Learn the basics of BeamOS with this interactive tutorial",
            Settings = new ModelSettings(
                unitSettings: new UnitSettingsContract
                {
                    LengthUnit = LengthUnitContract.Foot,
                    ForceUnit = ForceUnitContract.KilopoundForce,
                    AngleUnit = AngleUnitContract.Radian,
                },
                analysisSettings: new AnalysisSettings
                {
                    Element1DAnalysisType = Element1dAnalysisType.Timoshenko,
                },
                yAxisUp: true
            )
        };

        await this.StructuralAnalysisApiClient.CreateModelAsync(createModelRequest);

        // Show welcome dialog
        var dialogParameters = new DialogParameters();
        var dialogOptions = new DialogOptions 
        { 
            CloseOnEscapeKey = true
        };

        await this.DialogService.ShowAsync<TutorialWelcomeDialog>(
            "Welcome to the BeamOS Tutorial",
            dialogParameters,
            dialogOptions
        );
    }

    protected override ValueTask DisposeAsyncCore(bool disposing)
    {
        dispatcher.Dispatch(new CloseDrawer());
        return base.DisposeAsyncCore(disposing);
    }
}
