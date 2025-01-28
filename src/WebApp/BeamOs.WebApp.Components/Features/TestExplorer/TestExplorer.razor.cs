using System.Collections.Immutable;
using BeamOs.StructuralAnalysis.Contracts.Common;
using BeamOs.Tests.Common;
using BeamOs.WebApp.Components.Features.Editor;
using Fluxor;
using Fluxor.Blazor.Web.Components;

namespace BeamOs.WebApp.Components.Features.TestExplorer;

public partial class TestExplorer(IState<TestExplorerState> state) : FluxorComponent
{
    private EditorComponent? editorComponent;

    protected override void OnInitialized()
    {
        base.OnInitialized();

        this.SubscribeToAction<ChangeSelectedProblemTests>(async command =>
        {
            if (this.editorComponent is null)
            {
                return;
            }

            if (command.SelectedProblemTests is null || command.SelectedProblemTests.Count == 0)
            {
                await this.editorComponent.Clear();
                return;
            }

            var testFixture = command.SelectedProblemTests.First().GetTestFixture();

            await this.editorComponent.LoadBeamOsEntity(testFixture.MapToResponse());
            //if (editorComponent.State.Value.LoadedModelId.HasValue)
        });
    }
}

public readonly record struct ChangeSelectedProblemTests(List<TestInfo>? SelectedProblemTests);

public readonly record struct ChangeTestResultsDict(
    Dictionary<BeamOsObjectType, Dictionary<string, List<TestResult>>>? Results
);

public readonly record struct TestResultComputed(TestResult TestResult);

public readonly record struct ChangeShowProblemSource(bool Value);

public readonly record struct ChangeShowTestSelector(bool Value);

public readonly record struct ChangeShowTestResults(bool Value);

public readonly record struct ChangeSelectedSourceInfo(SourceInfo? SourceInfo);

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
    ImmutableDictionary<
        BeamOsObjectType,
        ImmutableDictionary<string, List<TestResult>>
    >? TestResults,
    //Dictionary<string, List<TestResult2>>? TestResults,
    //List<TestResult2>? SelectedTestResults,
    SourceInfo? SelectedSourceInfo
)
{
    private TestExplorerState()
        : this(null, null, null, null, null, true, true, true, null, null) { }
}

public static class TestExplorerStateReducers
{
    [ReducerMethod]
    public static TestExplorerState Reducer(
        TestExplorerState state,
        ChangeSelectedProblemTests action
    )
    {
        return state with
        {
            SelectedProblemTests = action.SelectedProblemTests,
            TestResults = ImmutableDictionary<
                BeamOsObjectType,
                ImmutableDictionary<string, List<TestResult>>
            >.Empty
        };
    }

    [ReducerMethod]
    public static TestExplorerState Reducer(TestExplorerState state, TestResultComputed action)
    {
        if (
            !state
                .TestResults
                .TryGetValue(action.TestResult.BeamOsObjectType, out var resultsForType)
        )
        {
            resultsForType = ImmutableDictionary<string, List<TestResult>>.Empty;
            //newDict = newDict.Add(action.TestResult.BeamOsObjectType, resultsForType);
        }
        if (!resultsForType.TryGetValue(action.TestResult.BeamOsObjectId, out var resultsList))
        {
            resultsList =  [];
            //resultsForType.Add(action.TestResult.BeamOsObjectId, resultsList);
        }
        resultsList.Add(action.TestResult);

        return state with
        {
            TestResults = state
                .TestResults
                .Remove(action.TestResult.BeamOsObjectType)
                .Add(
                    action.TestResult.BeamOsObjectType,
                    resultsForType
                        .Remove(action.TestResult.BeamOsObjectId)
                        .Add(action.TestResult.BeamOsObjectId, resultsList)
                )
        };
    }
}
