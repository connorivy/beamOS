using System.Reflection;
using System.Text;
using BeamOs.ApiClient.Builders;
using BeamOS.Tests.Common;
using BeamOS.Tests.Common.Fixtures;
using BeamOS.Tests.Common.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace BeamOs.Tests.TestRunner;

public class TestInfo
{
    public TestInfo(
        Type testClassType,
        object[]? testData,
        MethodInfo methodInfo,
        Dictionary<string, string> traitNameToValueDict
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
            if (testData.Length == 1 && testData[0] is FixtureBase2 fixture)
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
    public Dictionary<string, string> TraitNameToValueDict { get; }

    public IHasFixtureId? GetTestFixture() => this.TestData?.FirstOrDefault() as IHasFixtureId;

    public SourceInfo? SourceInfo =>
        (this.TestData?.FirstOrDefault() as IHasSourceInfo)?.SourceInfo;

    public async Task<TestResult> RunTest(IServiceProvider serviceProvider)
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
            await this.RunAndThrow(serviceProvider);
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

    public event EventHandler<TestResult2>? OnTestResult;

    public async Task RunTest2(IServiceProvider serviceProvider)
    {
        void OnAssertedEqual2(object? _, ComparedObjectEventArgs2 args) =>
            OnTestResult?.Invoke(
                this,
                new TestResult2(
                    //args.BeamOsObjectType,
                    args.BeamOsObjectId,
                    this.MethodInfo.Name,
                    args.ComparedObjectPropertyName,
                    args.ExpectedValue,
                    args.CalculatedValue,
                    TestResultStatus.Success,
                    null
                )
            );

        Asserter.AssertedEqual2 += OnAssertedEqual2;

        try
        {
            await this.RunAndThrow(serviceProvider);
        }
        catch (TargetInvocationException ex) when (ex.InnerException is Xunit.SkipException skipEx)
        {
            //tcs.SetResult(new TestResult(TestResultStatus.Skipped, skipEx.Message));
        }
        catch (TargetInvocationException ex)
            when (ex.InnerException is Xunit.Sdk.XunitException xEx)
        {
            //tcs.SetResult(new TestResult(TestResultStatus.Failure, xEx.Message));
        }
        finally
        {
            Asserter.AssertedEqual2 -= OnAssertedEqual2;
        }
    }

    private async Task RunAndThrow(IServiceProvider serviceProvider)
    {
        using var testScope = serviceProvider.CreateScope();
        object? testClass = testScope.ServiceProvider.GetRequiredService(this.TestClassType);
        if (testClass is IAsyncLifetime asyncLifetime)
        {
            await asyncLifetime.InitializeAsync();
        }
        object? test = this.MethodInfo.Invoke(testClass, this.TestData);

        if (test is Task t)
        {
            await t.ConfigureAwait(false);
        }
    }
}
