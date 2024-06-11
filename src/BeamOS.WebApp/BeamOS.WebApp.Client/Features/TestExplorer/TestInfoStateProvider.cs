using BeamOs.Tests.TestRunner;

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
}
