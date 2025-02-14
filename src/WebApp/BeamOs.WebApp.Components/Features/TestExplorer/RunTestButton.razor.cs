using System.Collections.Immutable;
using BeamOs.Tests.Common;
using Fluxor;
using Fluxor.Blazor.Web.Components;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace BeamOs.WebApp.Components.Features.TestExplorer;

public partial class RunTestButton(IStateSelection<TestInfoState, SingleTestState> singleTestState)
    : FluxorComponent
{
    [Parameter]
    public required string TestId { get; init; }

    protected override void OnInitialized()
    {
        base.OnInitialized();
        singleTestState.Select(x => x.TestInfoIdToTestResultDict[this.TestId]);
    }
}

[FeatureState]
public record TestInfoState(ImmutableDictionary<string, SingleTestState> TestInfoIdToTestResultDict)
{
    private TestInfoState()
        : this(ImmutableDictionary<string, SingleTestState>.Empty) { }

    //public TestResult? GetResultFromCache(TestInfo testInfo) =>
    //    this.GetResultFromCache(testInfo.Id);

    //public TestResult? GetResultFromCache(string testInfoId) =>
    //    this.TestInfoIdToTestResultDict.GetValueOrDefault(testInfoId)?.TestResult;

    public (string, Color) GetIconAndCssColorRepresentingTestResult(
        Tests.Common.TestResult testResult
    )
    {
        return testResult?.Status switch
        {
            TestResultStatus.Undefined => throw new NotImplementedException(),
            TestResultStatus.Failure => (@Icons.Material.Filled.Cancel, Color.Error),
            TestResultStatus.Skipped => (@Icons.Material.Filled.WarningAmber, Color.Warning),
            TestResultStatus.Success => (@Icons.Material.Filled.CheckCircle, Color.Success),
            null => (@Icons.Material.Filled.Help, Color.Info),
            _ => throw new NotImplementedException(),
        };
    }
}

public static class TestInfoStateReducers
{
    [ReducerMethod]
    public static TestInfoState Reducer(TestInfoState state, TestResultComputed action)
    {
        string testId = action.TestResult.Id;
        if (state.TestInfoIdToTestResultDict.ContainsKey(testId))
        {
            return state;
        }

        return state with
        {
            TestInfoIdToTestResultDict = state
                .TestInfoIdToTestResultDict
                //.Remove(testId)
                .Add(testId, new(TestProgressStatus.NotStarted, action.TestResult))
        };
    }

    [ReducerMethod]
    public static TestInfoState Reducer(TestInfoState state, TestResultProgressChanged action)
    {
        string testId = action.ResultId;
        var currentSingleTestResultState = state.TestInfoIdToTestResultDict[testId];

        if (currentSingleTestResultState.FrontEndProgressStatus == TestProgressStatus.Finished)
        {
            return state;
        }

        return state with
        {
            TestInfoIdToTestResultDict = state
                .TestInfoIdToTestResultDict
                .Remove(testId)
                .Add(
                    testId,
                    new(action.TestProgressStatus, currentSingleTestResultState.TestResult)
                )
        };
    }
}

[FeatureState]
public record SingleTestState(TestProgressStatus FrontEndProgressStatus, TestResult? TestResult)
{
    private SingleTestState()
        : this(TestProgressStatus.NotStarted, null) { }
}

public readonly record struct TestResultProgressChanged(
    string ResultId,
    TestProgressStatus TestProgressStatus
);
