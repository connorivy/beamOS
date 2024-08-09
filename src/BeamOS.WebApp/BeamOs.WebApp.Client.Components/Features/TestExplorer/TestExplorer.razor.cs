using BeamOS.Tests.Common.Fixtures;
using BeamOS.Tests.Common.Interfaces;
using BeamOS.Tests.Common.Traits;
using BeamOs.Tests.TestRunner;
using BeamOs.WebApp.Client.Components.Features.Editors.ReadOnlyEditor;
using Fluxor;
using Fluxor.Blazor.Web.Components;
using Microsoft.AspNetCore.Components;

namespace BeamOs.WebApp.Client.Components.Features.TestExplorer;

public partial class TestExplorer : FluxorComponent
{
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
        base.OnInitialized();
        this.SubscribeToAction<ExecutionTestAction>(_ => this.isLoadingAssertionResults = true);
        this.SubscribeToAction<ExecutionTestActionResult>(arg =>
        {
            if (arg.TestId != this.TestExplorerState.Value.SelectedTestInfo?.Id)
            {
                return;
            }

            switch (arg.Result)
            {
                case TestResult<double[]> testResultDoubleArray:
                    this.ResetAssertionResults();
                    this.AssertionResultArray = new(
                        testResultDoubleArray.ExpectedValue,
                        testResultDoubleArray.CalculatedValue
                    );
                    this.ComparedValueName = testResultDoubleArray.ComparedValueName;
                    break;
                case TestResult<double[,]> testResultDoubleMatrix:
                    this.ResetAssertionResults();
                    this.AssertionResultMatrix = new(
                        testResultDoubleMatrix.ExpectedValue,
                        testResultDoubleMatrix.CalculatedValue
                    );
                    this.ComparedValueName = testResultDoubleMatrix.ComparedValueName;
                    break;
                default:
                    this.ResetAssertionResults();
                    break;
            }
            this.isLoadingAssertionResults = false;
            this.StateHasChanged();
        });

        //foreach (var testInfo in this.TestInfoProvider.TestInfos.Values)
        //{
        //    this.Dispatcher.Dispatch(new CreateSingleTestStateAction(testInfo.Id));
        //}
    }

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
            await this.TestFixtureDisplayer.Display(fixtureBase, this.readOnlyEditor.ElementId);
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
}
