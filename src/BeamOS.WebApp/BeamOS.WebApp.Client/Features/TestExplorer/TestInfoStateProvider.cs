using BeamOs.Tests.TestRunner;
using MudBlazor;

namespace BeamOS.WebApp.Client.Features.TestExplorer;

public class TestInfoStateProvider
{
    // todo : save in local storage
    private readonly Dictionary<string, TestResult> testIdToResultDict = [];

    public TestResult? GetResultFromCache(TestInfo testInfo) =>
        this.testIdToResultDict.GetValueOrDefault(testInfo.Id);

    public async Task<TestResult> GetOrComputeTestResult(TestInfo testInfo)
    {
        if (this.testIdToResultDict.TryGetValue(testInfo.Id, out var cachedResult))
        {
            return cachedResult;
        }

        TestResult result = await testInfo.RunTest();
        this.testIdToResultDict.Add(testInfo.Id, result);
        return result;
    }

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
