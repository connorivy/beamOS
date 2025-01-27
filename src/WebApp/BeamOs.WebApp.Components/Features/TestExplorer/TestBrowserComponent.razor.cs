using System.Reflection;
using BeamOs.Tests.Common;
using Microsoft.Extensions.DependencyInjection;

namespace BeamOs.WebApp.Components.Features.TestExplorer;

public partial class TestBrowserComponent { }

public class TestInfo
{
    public object[]? TestData { get; }
    public MethodInfo MethodInfo { get; }
    public Type TestClassType { get; }

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
        //catch (TargetInvocationException ex) when (ex.InnerException is Xunit.SkipException skipEx)
        //{
        //    tcs.SetResult(new TestResult(TestResultStatus.Skipped, skipEx.Message));
        //}
        //catch (TargetInvocationException ex)
        //    when (ex.InnerException is Xunit.Sdk.XunitException xEx)
        //{
        //    tcs.SetResult(new TestResult(TestResultStatus.Failure, xEx.Message));
        //}
        finally
        {
            Asserter.DoublesAssertedEqual -= OnAssertedEqual;
            Asserter.DoubleArrayAssertedEqual -= OnAssertedEqual;
            Asserter.Double2dArrayAssertedEqual -= OnAssertedEqual;
        }

        return await tcs.Task;
    }

    private async Task RunAndThrow(IServiceProvider serviceProvider)
    {
        using var testScope = serviceProvider.CreateScope();
        object? testClass = testScope.ServiceProvider.GetRequiredService(this.TestClassType);
        //if (testClass is IAsyncLifetime asyncLifetime)
        //{
        //    await asyncLifetime.InitializeAsync();
        //}
        object? test = this.MethodInfo.Invoke(testClass, this.TestData);

        if (test is Task t)
        {
            await t.ConfigureAwait(false);
        }
    }
}

public enum TestResultStatus
{
    Undefined = 0,
    Failure,
    Skipped,
    Success,
}

public record TestResult(TestResultStatus Status, string? ResultMessage);

public record TestResult<T>(
    T ExpectedValue,
    T CalculatedValue,
    string ComparedValueName,
    TestResultStatus ResultStatus,
    string? ResultMessage
) : TestResult(ResultStatus, ResultMessage);

//public class ComparedObjectEventArgs<T>(T expected, T calculated, string comparedObjectName)
//    : EventArgs
//{
//    public T Expected { get; } = expected;
//    public T Calculated { get; } = calculated;
//    public string ComparedObjectName { get; } = comparedObjectName;
//}
