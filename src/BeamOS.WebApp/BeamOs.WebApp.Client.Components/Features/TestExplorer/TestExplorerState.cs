using BeamOS.Tests.Common.Interfaces;
using BeamOs.Tests.TestRunner;
using BeamOs.WebApp.Client.Components.Components.Editor;
using BeamOs.WebApp.Client.EditorCommands;
using Fluxor;

namespace BeamOs.WebApp.Client.Components.Features.TestExplorer;

[FeatureState]
public record TestExplorerState(
    string? CanvasId,
    string? ModelId,
    TestInfo? SelectedTestInfo,
    string? SelectedTrait,
    List<TestInfo>? SelectedProblemTests,
    bool ShowProblemSource,
    bool ShowTestSelector,
    bool ShowTestResults,
    Dictionary<string, List<TestResult2>>? TestResults,
    List<TestResult2>? SelectedTestResults,
    SourceInfo? SelectedSourceInfo
)
{
    private TestExplorerState()
        : this(null, null, null, null, null, true, true, true, null, null, null) { }
}

public readonly record struct CanvasIdSet(string CanvasId);

public readonly record struct ChangeSelectedTestInfoAction(TestInfo? SelectedTestInfo);

public readonly record struct ChangeSelectedTraitAction(string SelectedTrait);

public readonly record struct ChangeSelectedProblemTests(List<TestInfo>? SelectedProblemTests);

public readonly record struct ChangeShowProblemSource(bool Value);

public readonly record struct ChangeShowTestSelector(bool Value);

public readonly record struct ChangeShowTestResults(bool Value);

public readonly record struct ChangeSelectedSourceInfo(SourceInfo? SourceInfo);

//public readonly record struct ChangeTestResultsDict(Dictionary<string, List<TestResult2>>? Value);

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
    public static TestExplorerState ModelLoadedReducer(
        TestExplorerState state,
        ModelLoaded modelLoadedAction
    )
    {
        if (state.SelectedTestResults is null)
        {
            return state with
            {
                ModelId = modelLoadedAction.ModelId,
                SelectedTestResults = state
                    .TestResults
                    ?.GetValueOrDefault(modelLoadedAction.ModelId)
            };
        }
        return state with { ModelId = modelLoadedAction.ModelId };
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

    //[ReducerMethod]
    //public static TestExplorerState ChangeTestResultsDict(
    //    TestExplorerState state,
    //    ChangeTestResultsDict changeTestResultsDict
    //)
    //{
    //    return state with { TestResults = changeTestResultsDict.Value };
    //}

    [ReducerMethod]
    public static TestExplorerState ChangeSelectionCommandReducer(
        TestExplorerState state,
        ChangeSelectionCommand changeSelectionCommand
    )
    {
        if (changeSelectionCommand.CanvasId != state.CanvasId)
        {
            return state;
        }

        if (changeSelectionCommand.SelectedObjects.Length == 0)
        {
            if (state.ModelId is not null)
            {
                return state with
                {
                    SelectedTestResults = state.TestResults?.GetValueOrDefault(state.ModelId)
                };
            }
            return state with { SelectedTestResults = null };
        }

        return state with
        {
            SelectedTestResults = state
                .TestResults
                ?.GetValueOrDefault(changeSelectionCommand.SelectedObjects[0].Id)
        };
    }

    [ReducerMethod]
    public static TestExplorerState ChangeSelectedSourceInfo(
        TestExplorerState state,
        ChangeSelectedSourceInfo changeSelectedSourceInfo
    )
    {
        return state with { SelectedSourceInfo = changeSelectedSourceInfo.SourceInfo };
    }
}
