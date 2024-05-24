using System.Reflection;
using BeamOS.Tests.Common.Interfaces;

namespace BeamOs.Tests.TestRunner;

public class TestInfo
{
    public TestInfo(
        Type testClassType,
        object[] testData,
        MethodInfo methodInfo,
        Dictionary<string, string[]> traitNameToValueDict
    )
    {
        this.TestData = testData;
        this.MethodInfo = methodInfo;
        this.TestClassType = testClassType;
        this.TraitNameToValueDict = traitNameToValueDict;
    }

    public object[] TestData { get; }
    public MethodInfo MethodInfo { get; }
    public Type TestClassType { get; }
    public Dictionary<string, string[]> TraitNameToValueDict { get; }

    public ITestFixtureDisplayable? GetDisplayable() =>
        this.TestData.FirstOrDefault() as ITestFixtureDisplayable;

    public async Task RunTest()
    {
        // todo : will fail for tests with constructor
        object? test = this.MethodInfo.Invoke(
            Activator.CreateInstance(this.TestClassType),
            this.TestData
        );
        if (test is Task t)
        {
            await t.ConfigureAwait(false);
        }
    }
}
