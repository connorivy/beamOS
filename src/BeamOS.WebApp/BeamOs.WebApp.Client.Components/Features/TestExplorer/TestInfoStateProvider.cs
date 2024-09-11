using System.Collections.Immutable;
using BeamOs.Tests.TestRunner;
using Fluxor;
using MudBlazor;

namespace BeamOs.WebApp.Client.Components.Features.TestExplorer;

public class TestInfoStateProvider
{
    // todo : save in local storage
    private readonly Dictionary<string, TestResult> testIdToResultDict = [];

    public TestResult? GetResultFromCache(TestInfo testInfo) =>
        this.testIdToResultDict.GetValueOrDefault(testInfo.Id);

    //public async Task<TestResult> GetOrComputeTestResult(TestInfo testInfo)
    //{
    //    if (this.testIdToResultDict.TryGetValue(testInfo.Id, out var cachedResult))
    //    {
    //        return cachedResult;
    //    }

    //    TestResult result = await testInfo.RunTest();
    //    this.testIdToResultDict.Add(testInfo.Id, result);
    //    return result;
    //}

    public (string, Color) GetIconAndCssColorRepresentingTestResult(TestInfo testInfo)
    {
        TestResult? result = this.GetResultFromCache(testInfo);

        return result?.Status switch
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

[FeatureState]
public record TestInfoState(
    ImmutableDictionary<string, SingleTestState> TestInfoIdToTestResultDict,
    HashSet<string> TestIdsBeingRun
)
{
    private TestInfoState()
        : this(ImmutableDictionary<string, SingleTestState>.Empty, []) { }

    public TestResult? GetResultFromCache(TestInfo testInfo) =>
        this.GetResultFromCache(testInfo.Id);

    public TestResult? GetResultFromCache(string testInfoId) =>
        this.TestInfoIdToTestResultDict.GetValueOrDefault(testInfoId)?.TestResult;

    public (string, Color) GetIconAndCssColorRepresentingTestResult(TestInfo testInfo)
    {
        TestResult? result = this.GetResultFromCache(testInfo);

        return result?.Status switch
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

[FeatureState]
public record SingleTestState(TestProgressStatus TestProgressStatus, TestResult? TestResult)
{
    private SingleTestState()
        : this(TestProgressStatus.NotStarted, null) { }
}

public static class TestInfoStateReducers
{
    // todo : creating new dictionary every time could potentially be expensive / overkill
    [ReducerMethod]
    public static TestInfoState CreateSingleTestStateActionReducer(
        TestInfoState state,
        CreateSingleTestStateAction action
    )
    {
        if (state.TestInfoIdToTestResultDict.ContainsKey(action.TestId))
        {
            return state;
        }
        Dictionary<string, SingleTestState> dictCopy =
            new(state.TestInfoIdToTestResultDict)
            {
                { action.TestId, new(TestProgressStatus.NotStarted, null) }
            };

        return state with
        {
            TestInfoIdToTestResultDict = dictCopy.ToImmutableDictionary()
        };
    }

    [ReducerMethod]
    public static TestInfoState DisposeSingleTestStateActionReducer(
        TestInfoState state,
        DisposeSingleTestStateAction action
    )
    {
        if (!state.TestInfoIdToTestResultDict.ContainsKey(action.TestId))
        {
            return state;
        }

        Dictionary<string, SingleTestState> dictCopy = new(state.TestInfoIdToTestResultDict);
        _ = dictCopy.Remove(action.TestId);

        return state with
        {
            TestInfoIdToTestResultDict = dictCopy.ToImmutableDictionary()
        };
    }

    [ReducerMethod]
    public static TestInfoState ExecutionTestActionResultReducer(
        TestInfoState state,
        ExecutionTestActionResult action
    )
    {
        SingleTestState newState = new(TestProgressStatus.Finished, action.Result);

        Dictionary<string, SingleTestState> dictCopy =
            new(state.TestInfoIdToTestResultDict) { [action.TestId] = newState };

        return state with
        {
            TestInfoIdToTestResultDict = dictCopy.ToImmutableDictionary()
        };
    }
}

public readonly record struct CreateSingleTestStateAction(string TestId);

public readonly record struct CreateSingleTestStateActionResult(string TestId);

public readonly record struct DisposeSingleTestStateAction(string TestId);
