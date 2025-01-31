using System.Reflection;
using BeamOs.StructuralAnalysis.Contracts.Common;
using Microsoft.Extensions.DependencyInjection;

namespace BeamOs.Tests.Common;

public class TestInfo
{
    public object[]? TestData { get; init; }
    public MethodInfo MethodInfo { get; init; }
    public Type TestClassType { get; init; }

    public ITestFixture? GetTestFixture() => this.TestData?.FirstOrDefault() as ITestFixture;

    public SourceInfo? SourceInfo =>
        (this.TestData?.FirstOrDefault() as IHasSourceInfo)?.SourceInfo;

    public event EventHandler<TestResult>? OnTestResult;

    public async Task RunTest(IServiceProvider serviceProvider)
    {
        TaskCompletionSource<TestResultBase> tcs = new();

        void OnAssertedEqual(object? _, ComparedObjectEventArgs args) =>
            this.OnTestResult?.Invoke(
                this,
                new TestResult(
                    args.BeamOsObjectType,
                    args.BeamOsObjectId,
                    this.MethodInfo.Name,
                    args.ComparedObjectPropertyName,
                    args.ExpectedValue,
                    args.CalculatedValue,
                    TestResultStatus.Success,
                    null,
                    args.ComparedValueNameCollection
                )
            );

        Asserter.AssertedEqual2 += OnAssertedEqual;
        //Asserter.DoublesAssertedEqual += OnAssertedEqual;
        //Asserter.DoubleArrayAssertedEqual += OnAssertedEqual;
        //Asserter.Double2dArrayAssertedEqual += OnAssertedEqual;

        try
        {
            await this.RunAndThrow(serviceProvider);
        }
        //catch (TargetInvocationException ex) when (ex.InnerException is Xunit.SkipException skipEx)
        //{
        //    tcs.SetResult(new TestResultBase(TestResultBaseStatus.Skipped, skipEx.Message));
        //}
        //catch (TargetInvocationException ex)
        //    when (ex.InnerException is Xunit.Sdk.XunitException xEx)
        //{
        //    tcs.SetResult(new TestResultBase(TestResultBaseStatus.Failure, xEx.Message));
        //}
        catch (Exception ex)
        {
            ;
        }
        finally
        {
            Asserter.AssertedEqual2 -= OnAssertedEqual;
            //Asserter.DoublesAssertedEqual -= OnAssertedEqual;
            //Asserter.DoubleArrayAssertedEqual -= OnAssertedEqual;
            //Asserter.Double2dArrayAssertedEqual -= OnAssertedEqual;
        }
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

public enum TestProgressStatus
{
    Undefined = 0,
    Finished,
    InProgress,
    NotStarted
}

public record TestResultBase(TestResultStatus Status, string? ResultMessage);

public record TestResultBase<T>(
    T ExpectedValue,
    T CalculatedValue,
    string ComparedValueName,
    TestResultStatus ResultStatus,
    string? ResultMessage
) : TestResultBase(ResultStatus, ResultMessage);

public record TestResult(
    BeamOsObjectType BeamOsObjectType,
    string BeamOsObjectId,
    string TestName,
    string ComparedValueName,
    object ExpectedValue,
    object CalculatedValue,
    TestResultStatus ResultStatus,
    string? ResultMessage,
    ICollection<string>? ComparedValueNameCollection = null
) : TestResultBase(ResultStatus, ResultMessage)
{
    public string Id =>
        field ??= $"{this.BeamOsObjectType} {this.BeamOsObjectId} {this.ComparedValueName}";
}

//public class ComparedObjectEventArgs<T>(T expected, T calculated, string comparedObjectName)
//    : EventArgs
//{
//    public T Expected { get; } = expected;
//    public T Calculated { get; } = calculated;
//    public string ComparedObjectName { get; } = comparedObjectName;
//}
