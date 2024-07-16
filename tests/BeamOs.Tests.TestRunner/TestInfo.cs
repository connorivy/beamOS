using System.Reflection;
using System.Text;
using BeamOS.Tests.Common;
using BeamOS.Tests.Common.Fixtures;
using BeamOS.Tests.Common.Interfaces;

namespace BeamOs.Tests.TestRunner;

public class TestInfo
{
    public TestInfo(
        Type testClassType,
        object[]? testData,
        MethodInfo methodInfo,
        Dictionary<string, string[]> traitNameToValueDict
    )
    {
        this.TestData = testData;
        this.MethodInfo = methodInfo;
        this.TestClassType = testClassType;
        this.TraitNameToValueDict = traitNameToValueDict;

        StringBuilder sb = new();
        sb.Append($"{testClassType.FullName}.{methodInfo.Name}");
        if (testData?.Length > 0)
        {
            if (testData.Length == 1 && testData[0] is FixtureBase fixture)
            {
                sb.Append($".{fixture.Id}");
            }
            else
            {
                foreach (var item in testData)
                {
                    sb.Append(item);
                }
            }
        }
        this.Id = sb.ToString();
    }

    public string Id { get; }
    public object[]? TestData { get; }
    public MethodInfo MethodInfo { get; }
    public Type TestClassType { get; }
    public Dictionary<string, string[]> TraitNameToValueDict { get; }

    public ITestFixtureDisplayable? GetDisplayable() =>
        this.TestData?.FirstOrDefault() as ITestFixtureDisplayable;

    public SourceInfo? SourceInfo => this.GetDisplayable()?.SourceInfo;

    public async Task<TestResult> RunTest()
    {
        TaskCompletionSource<TestResult> tcs = new();
        TestResult? result = null;

        void OnAssertedEqual<T>(object? _, ComparedObjectEventArgs<T> args) =>
            result = new TestResult<T>(
                args.Expected,
                args.Calculated,
                args.ComparedObjectName,
                TestResultStatus.Success,
                null
            );

        Asserter.DoublesAssertedEqual += OnAssertedEqual;
        Asserter.DoubleArrayAssertedEqual += OnAssertedEqual;
        Asserter.Double2dArrayAssertedEqual += OnAssertedEqual;

        try
        {
            await this.RunAndThrow();
            tcs.SetResult(result ?? throw new Exception("Result was unset"));
        }
        catch (TargetInvocationException ex) when (ex.InnerException is Xunit.SkipException skipEx)
        {
            tcs.SetResult(new TestResult(TestResultStatus.Skipped, skipEx.Message));
        }
        catch (TargetInvocationException ex)
            when (ex.InnerException is Xunit.Sdk.XunitException xEx)
        {
            tcs.SetResult(new TestResult(TestResultStatus.Failure, xEx.Message));
        }
        finally
        {
            Asserter.DoublesAssertedEqual -= OnAssertedEqual;
            Asserter.DoubleArrayAssertedEqual -= OnAssertedEqual;
            Asserter.Double2dArrayAssertedEqual -= OnAssertedEqual;
        }

        return await tcs.Task;
    }

    private async Task RunAndThrow()
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
