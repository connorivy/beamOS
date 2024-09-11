using BeamOs.Contracts.Editor;
using BeamOs.Tests.TestRunner;
using BeamOs.WebApp.Client.Components.Components.Editor;
using BeamOs.WebApp.Client.EditorCommands;
using Fluxor;

namespace BeamOs.WebApp.Client.Components.Features.TestExplorer.Components;

[FeatureState]
public record TestSelectorState(
    string? CanvasId,
    string? ModelId,
    string? SelectedObjectId,
    string? SelectedObjectType,
    Dictionary<string, List<TestResult2>>? AllTestResults,
    List<TestResult2>? SelectedTestResults,
    int TotalNumTests,
    bool EditorFilterApplied
)
{
    public TestSelectorState()
        : this(null, null, null, null, null, null, 0, false) { }
}

public readonly record struct ChangeShowTestResults(bool Value);

public readonly record struct ChangeTestResultsDict(Dictionary<string, List<TestResult2>>? Value);

public record struct SetColorFilterCommand(string CanvasId, SetColorFilter Command);

public record struct RemoveColorFilterCommand(string CanvasId, ClearFilters Command);

public record struct ChangeEditorFilterApplied(bool Value);

public static class TestSelectorStateFluxor
{
    [ReducerMethod]
    public static TestSelectorState CanvasIdSetReducer(
        TestSelectorState state,
        CanvasIdSet canvasIdSetAction
    )
    {
        return state with { CanvasId = canvasIdSetAction.CanvasId };
    }

    [ReducerMethod]
    public static TestSelectorState ModelLoadedReducer(
        TestSelectorState state,
        ModelLoaded modelLoaded
    )
    {
        return state with { ModelId = modelLoaded.ModelId };
    }

    [ReducerMethod]
    public static TestSelectorState ReducerMethod(
        TestSelectorState state,
        ChangeSelectionCommand changeSelectionCommand
    )
    {
        if (changeSelectionCommand.CanvasId != state.CanvasId)
        {
            return state;
        }

        string selectedObjId =
            changeSelectionCommand.SelectedObjects.FirstOrDefault()?.Id ?? state.ModelId ?? "";

        return state with
        {
            SelectedObjectId = selectedObjId,
            SelectedObjectType =
                changeSelectionCommand.SelectedObjects.FirstOrDefault()?.TypeName ?? "Model",
            SelectedTestResults = state.AllTestResults?.GetValueOrDefault(selectedObjId)
        };
    }

    [ReducerMethod]
    public static TestSelectorState ChangeTestResultsDict(
        TestSelectorState state,
        ChangeTestResultsDict changeTestResultsDict
    )
    {
        return state with
        {
            AllTestResults = changeTestResultsDict.Value,
            TotalNumTests = changeTestResultsDict.Value?.SelectMany(kvp => kvp.Value).Count() ?? 0,
            SelectedTestResults = changeTestResultsDict
                .Value
                ?.GetValueOrDefault(state.ModelId ?? ""),
            SelectedObjectId = state.ModelId,
            SelectedObjectType = "Model",
            EditorFilterApplied = false
        };
    }

    [ReducerMethod]
    public static TestSelectorState ChangeEditorFilterApplied(
        TestSelectorState state,
        ChangeEditorFilterApplied changeEditorFilterApplied
    )
    {
        return state with { EditorFilterApplied = changeEditorFilterApplied.Value, };
    }
}
