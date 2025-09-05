using BeamOs.WebApp.Components.Features.Editor;
using Fluxor;
using Fluxor.Blazor.Web.Components;
using Microsoft.AspNetCore.Components;
using static BeamOs.WebApp.Components.Pages.ModelEditor;

namespace BeamOs.WebApp.Components.Features.BugReports;

public partial class ModelRepairScenarioCreator(
    IState<ModelRepairBugReportState> state,
    IDispatcher dispatcher,
    IState<EditorComponentState> editorState
) : FluxorComponent
{
    [Parameter]
    public Guid ModelId { get; set; }

    private string ExpectedStateDescription { get; set; } = string.Empty;

    protected override void OnInitialized()
    {
        base.OnInitialized();
        dispatcher.Dispatch(new PanelContentLockValue(true));
    }

    private void NextStep()
    {
        var currentStep = state.Value.Step;
        var nextStep = (int)state.Value.Step + 1;
        dispatcher.Dispatch(
            new ModelRepairBugReportState.ChangeStep((ModelRepairBugReportStep)nextStep)
        );
    }

    private void Cancel()
    {
        dispatcher.Dispatch(new PanelContentLockValue(false));
        dispatcher.Dispatch(new ShowLastPanelContent());
    }

    private void Submit()
    {
        dispatcher.Dispatch(new PanelContentLockValue(false));
        dispatcher.Dispatch(new ShowLastPanelContent());
    }
}

/// <summary>
/// Order of the steps matters because the flow will increment the next step number
/// </summary>
public enum ModelRepairBugReportStep
{
    Undefined = 0,
    SelectElements,
    DescribeExpectedState,
}

[FeatureState]
public record ModelRepairBugReportState(ModelRepairBugReportStep Step)
{
    public ModelRepairBugReportState()
        : this(ModelRepairBugReportStep.SelectElements) { }

    public record struct ChangeStep(ModelRepairBugReportStep Step);
}

public static class ModelRepairBugReportStateReducers
{
    [ReducerMethod]
    public static ModelRepairBugReportState ChangeStep(
        ModelRepairBugReportState state,
        ModelRepairBugReportState.ChangeStep action
    )
    {
        return state with { Step = action.Step };
    }
}
