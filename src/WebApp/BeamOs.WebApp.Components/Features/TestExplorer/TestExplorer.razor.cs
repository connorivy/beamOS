using System.Collections.Immutable;
using BeamOs.StructuralAnalysis.Contracts.Common;
using BeamOs.Tests.Common;
using BeamOs.WebApp.Components.Features.Editor;
using BeamOs.WebApp.Components.Layout;
using Fluxor;
using Fluxor.Blazor.Web.Components;

namespace BeamOs.WebApp.Components.Features.TestExplorer;

public partial class TestExplorer(
    IState<TestExplorerState> state,
    IState<EditorComponentState> editorState,
    IDispatcher dispatcher
) : FluxorComponent
{
    private EditorComponent? editorComponent;

    protected override void OnInitialized()
    {
        base.OnInitialized();
        dispatcher.Dispatch(new OpenDrawer());
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
            var entity = testFixture?.MapToResponse();
            // dispatcher.Dispatch(new ChangeDisplayedObject(new(entity.GetType(), entity.IdString)));

            await this.editorComponent.LoadBeamOsEntity(testFixture.MapToResponse());
            //if (editorComponent.State.Value.LoadedModelId.HasValue)
        });
    }

    protected override async ValueTask DisposeAsyncCore(bool disposing)
    {
        if (disposing)
        {
            dispatcher.Dispatch(new CloseDrawer());
        }
        await base.DisposeAsyncCore(disposing);
    }

    public const string Href = "/test-explorer";
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

public readonly record struct ChangeDisplayedObject(DisplayedObject? DisplayedObject);

public readonly record struct DisplayedObject(Type Type, string Id);

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
    SourceInfo? SelectedSourceInfo,
    DisplayedObject? DisplayedObject
)
{
    private TestExplorerState()
        : this(null, null, null, null, null, true, true, true, null, null, null) { }
}

public static class TestExplorerStateReducers
{
    [ReducerMethod]
    public static TestExplorerState Reducer(
        TestExplorerState state,
        ChangeSelectedProblemTests action
    )
    {
        var newTestResults =
            action.SelectedProblemTests == null
                ? null
                : ImmutableDictionary<
                    BeamOsObjectType,
                    ImmutableDictionary<string, List<TestResult>>
                >.Empty;

        return state with
        {
            SelectedProblemTests = action.SelectedProblemTests,
            TestResults = newTestResults,
        };
    }

    [ReducerMethod]
    public static TestExplorerState Reducer(TestExplorerState state, TestResultComputed action)
    {
        ImmutableDictionary<string, List<TestResult>>? resultsForType;
        if (state.TestResults is null)
        {
            resultsForType = ImmutableDictionary<string, List<TestResult>>.Empty;
        }
        else if (
            !state.TestResults.TryGetValue(action.TestResult.BeamOsObjectType, out resultsForType)
        )
        {
            resultsForType = ImmutableDictionary<string, List<TestResult>>.Empty;
        }

        if (!resultsForType.TryGetValue(action.TestResult.BeamOsObjectId, out var resultsList))
        {
            resultsList = [];
            //resultsForType.Add(action.TestResult.BeamOsObjectId, resultsList);
        }
        resultsList.Add(action.TestResult);

        return state with
        {
            TestResults = (
                state.TestResults?.Remove(action.TestResult.BeamOsObjectType)
                ?? ImmutableDictionary<
                    BeamOsObjectType,
                    ImmutableDictionary<string, List<TestResult>>
                >.Empty
            ).Add(
                action.TestResult.BeamOsObjectType,
                resultsForType
                    .Remove(action.TestResult.BeamOsObjectId)
                    .Add(action.TestResult.BeamOsObjectId, resultsList)
            ),
        };
    }
}
