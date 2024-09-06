using BeamOS.Tests.Common.Fixtures;
using BeamOS.Tests.Common.Traits;
using BeamOs.Tests.TestRunner;
using BeamOs.WebApp.Client.Components.Components.Editor.CommandHandlers;
using BeamOs.WebApp.Client.Components.Features.Editors.ReadOnlyEditor;
using BeamOs.WebApp.Client.Components.Layout;
using Fluxor;
using Fluxor.Blazor.Web.Components;
using Microsoft.AspNetCore.Components;

namespace BeamOs.WebApp.Client.Components.Features.TestExplorer;

public partial class TestExplorer : FluxorComponent
{
    public const string Href = "/test-explorer";

    [Inject]
    private IState<TestExplorerState> TestExplorerState { get; init; }

    [Inject]
    private IDispatcher Dispatcher { get; init; }

    [Inject]
    private TestInfoProvider TestInfoProvider { get; init; }

    [Inject]
    private IState<TestInfoState> TestInfoState { get; init; }

    [Inject]
    private TestFixtureDisplayer TestFixtureDisplayer { get; init; }

    private bool open = true;
    private ReadOnlyEditor readOnlyEditor;

    //private string? nameOfAssertionResult { get; set; }

    //public double[] ExpectedValues { get; set; } = [];
    //public double[]? CalculatedValues { get; set; }

    //private double? GetDifferenceOrNull(double expected, double? calculated)
    //{
    //    if (calculated is not double typedCalculated)
    //    {
    //        return null;
    //    }
    //    return Math.Round(expected - typedCalculated, 5);
    //}

    string searchTerm = "";
    const string defaultTrait = ProblemSourceAttribute.TRAIT_NAME;
    string selectedTrait => this.TestExplorerState.Value.SelectedTrait ?? defaultTrait;

    private readonly string splitterContentClass = $"s{Guid.NewGuid()}";
    private double dimension = 20;

    private bool isLoadingAssertionResults;
    private string? ComparedValueName { get; set; }
    private AssertionResult<double[]>? AssertionResultArray { get; set; }
    private AssertionResult<double[,]>? AssertionResultMatrix { get; set; }

    private void ResetAssertionResults()
    {
        this.AssertionResultArray = null;
        this.AssertionResultMatrix = null;
        this.ComparedValueName = null;
    }

    protected override void OnInitialized()
    {
        this.Dispatcher.Dispatch(new OpenDrawer());
        EventEmitter.VisibleStateChanged += this.EventEmitter_VisibleStateChanged;
        base.OnInitialized();
    }

    private void EventEmitter_VisibleStateChanged(object? sender, EventArgs _) =>
        this.InvokeAsync(this.StateHasChanged);

    private async Task OnSelectedTestInfoChanged(TestInfo? testInfo)
    {
        if (testInfo is null)
        {
            await this.readOnlyEditor.EditorApiAlpha.ClearAsync();
            return;
        }

        if (testInfo.GetTestFixture() is FixtureBase2 fixtureBase)
        {
            await this.readOnlyEditor.EditorApiAlpha.ClearAsync();
            await this.TestFixtureDisplayer.Display(fixtureBase, this.readOnlyEditor.CanvasId);
        }

        //ITestFixtureDisplayable? displayable = testInfo.GetDisplayable();
        //if (displayable != this.TestExplorerState.Value.SelectedTestInfo?.GetDisplayable())
        //{
        //    await this.readOnlyEditor.EditorApiAlpha.ClearAsync();
        //    if (displayable is not null)
        //    {
        //        await displayable.Display(this.readOnlyEditor.EditorApiAlpha);
        //    }
        //}

        this.Dispatcher.Dispatch(new ChangeSelectedTestInfoAction(testInfo));
        this.Dispatcher.Dispatch(new ExecutionTestAction(testInfo));
        //this.Dispatcher.Dispatch(new TestExecutionBegun());

        //TestResult result = await this.TestInfoStateProvider.GetOrComputeTestResult(testInfo);

        //this.Dispatcher.Dispatch(new TestExecutionCompleted(result));
    }

    private void OnSelectedTraitChanged(IEnumerable<string> selectedTraits)
    {
        ChangeSelectedTraitAction action = new(selectedTraits.First());
        this.Dispatcher.Dispatch(action);
    }

    private bool SelectedTestHasResults()
    {
        return this.AssertionResultArray is not null || this.AssertionResultMatrix is not null;
    }

    protected override ValueTask DisposeAsyncCore(bool disposing)
    {
        this.Dispatcher.Dispatch(new CloseDrawer());
        EventEmitter.VisibleStateChanged -= this.EventEmitter_VisibleStateChanged;
        return base.DisposeAsyncCore(disposing);
    }

    private string GetCanvasId()
    {
        if (this.readOnlyEditor?.CanvasId is string canvasId)
        {
            return canvasId;
        }
        var newCanvasId = ReadOnlyEditor.GetCanvasId();
        this.Dispatcher.Dispatch(new CanvasIdSet(newCanvasId));
        return newCanvasId;
    }
}
