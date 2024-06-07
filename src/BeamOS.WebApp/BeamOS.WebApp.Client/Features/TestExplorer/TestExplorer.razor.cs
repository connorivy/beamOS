using BeamOS.Tests.Common;
using BeamOS.Tests.Common.Interfaces;
using BeamOS.Tests.Common.Traits;
using BeamOs.Tests.TestRunner;
using BeamOS.WebApp.Client.Components;
using BeamOS.WebApp.Client.Pages;
using Fluxor;
using Fluxor.Blazor.Web.Components;
using Microsoft.AspNetCore.Components;

namespace BeamOS.WebApp.Client.Features.TestExplorer;

public partial class TestExplorer : FluxorComponent
{
    [Inject]
    private IState<TestExplorerState> TestExplorerState { get; init; }

    [Inject]
    private IDispatcher Dispatcher { get; init; }

    [Inject]
    private TestInfoProvider TestInfoProvider { get; init; }

    private bool open = true;
    private EditorComponent? editorComponent;
    private string? nameOfAssertionResult { get; set; }

    public double[] ExpectedValues { get; set; } = [];
    public double[]? CalculatedValues { get; set; }

    private double? GetDifferenceOrNull(double expected, double? calculated)
    {
        if (calculated is not double typedCalculated)
        {
            return null;
        }
        return Math.Round(expected - typedCalculated, 5);
    }

    string searchTerm = "";
    TestInfo[] testInfos => this.TestInfoProvider.TestInfos;
    TestInfo? selectedTestInfo;
    const string defaultTrait = ProblemSourceAttribute.TRAIT_NAME;
    string selectedTrait => this.TestExplorerState.Value.SelectedTrait ?? defaultTrait;

    protected override void OnInitialized()
    {
        base.OnInitialized();
    }

    private async Task OnSelectedTestInfoChanged(TestInfo? testInfo)
    {
        ITestFixtureDisplayable? displayable = testInfo?.GetDisplayable();
        if (displayable != this.TestExplorerState.Value.SelectedTestInfo?.GetDisplayable())
        {
            await this.editorComponent.EditorApiAlpha.ClearAsync();
            if (displayable is not null)
            {
                await displayable.Display(this.editorComponent.EditorApiAlpha);
            }
        }

        Asserter.DoubleArrayAssertedEqual += this.OnDoubleArrayAssertedEqual;
        try
        {
            await testInfo.RunTest();
        }
        finally
        {
            Asserter.DoubleArrayAssertedEqual -= this.OnDoubleArrayAssertedEqual;
        }

        ChangeSelectedTestInfoAction action = new(testInfo);
        this.Dispatcher.Dispatch(action);
    }

    private void OnSelectedTraitChanged(IEnumerable<string> selectedTraits)
    {
        ChangeSelectedTraitAction action = new(selectedTraits.First());
        this.Dispatcher.Dispatch(action);
    }

    private void OnDoubleArrayAssertedEqual(
        object? _,
        ComparedObjectEventArgs<double[]> comparedObjectEventArgs
    )
    {
        this.ExpectedValues = comparedObjectEventArgs.Expected;
        this.CalculatedValues = comparedObjectEventArgs.Calculated;
        this.nameOfAssertionResult = comparedObjectEventArgs.ComparedObjectName;
        this.StateHasChanged();
    }
}
