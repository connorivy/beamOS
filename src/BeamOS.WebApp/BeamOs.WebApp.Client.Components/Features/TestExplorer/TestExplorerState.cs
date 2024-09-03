using BeamOs.Tests.TestRunner;
using Fluxor;

namespace BeamOs.WebApp.Client.Components.Features.TestExplorer;

[FeatureState]
public record TestExplorerState(
    string? CanvasId,
    TestInfo? SelectedTestInfo,
    string? SelectedTrait,
    List<TestInfo>? SelectedProblemTests,
    bool ShowProblemSource,
    bool ShowTestSelector,
    bool ShowTestResults
)
{
    private TestExplorerState()
        : this(null, null, null, null, true, true, true) { }
}

public readonly record struct CanvasIdSet(string CanvasId);

public readonly record struct ChangeSelectedTestInfoAction(TestInfo? SelectedTestInfo);

public readonly record struct ChangeSelectedTraitAction(string SelectedTrait);

public readonly record struct ChangeSelectedProblemTests(List<TestInfo>? SelectedProblemTests);

public readonly record struct ChangeShowProblemSource(bool Value);

public readonly record struct ChangeShowTestSelector(bool Value);

public readonly record struct ChangeShowTestResults(bool Value);

public static class TestExplorerStateFluxor
{
    [ReducerMethod]
    public static TestExplorerState CanvasIdSetReducer(
        TestExplorerState state,
        CanvasIdSet canvasIdSetAction
    )
    {
        return state with { CanvasId = canvasIdSetAction.CanvasId };
    }

    [ReducerMethod]
    public static TestExplorerState ChangeSelectedTestInfoReducer(
        TestExplorerState state,
        ChangeSelectedTestInfoAction changeSelectedTestInfoAction
    )
    {
        return state with { SelectedTestInfo = changeSelectedTestInfoAction.SelectedTestInfo };
    }

    [ReducerMethod]
    public static TestExplorerState ChangeSelectedTraitReducer(
        TestExplorerState state,
        ChangeSelectedTraitAction changeSelectedTraitAction
    )
    {
        return state with { SelectedTrait = changeSelectedTraitAction.SelectedTrait };
    }

    [ReducerMethod]
    public static TestExplorerState ChangeSelectedTraitReducer(
        TestExplorerState state,
        ChangeSelectedProblemTests changeSelectedProblemTests
    )
    {
        return state with
        {
            SelectedProblemTests = changeSelectedProblemTests.SelectedProblemTests
        };
    }

    [ReducerMethod]
    public static TestExplorerState ChangeShowProblemSource(
        TestExplorerState state,
        ChangeShowProblemSource changeShowProblemSource
    )
    {
        return state with { ShowProblemSource = changeShowProblemSource.Value };
    }

    [ReducerMethod]
    public static TestExplorerState ChangeShowTestSelector(
        TestExplorerState state,
        ChangeShowTestSelector changeShowTestSelector
    )
    {
        return state with { ShowTestSelector = changeShowTestSelector.Value };
    }

    [ReducerMethod]
    public static TestExplorerState ChangeShowTestResults(
        TestExplorerState state,
        ChangeShowTestResults changeShowTestResults
    )
    {
        return state with { ShowTestResults = changeShowTestResults.Value };
    }
}
