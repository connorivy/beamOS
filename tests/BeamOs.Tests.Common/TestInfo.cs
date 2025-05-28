using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using BeamOs.StructuralAnalysis.Contracts.Common;
using Microsoft.Extensions.DependencyInjection;

namespace BeamOs.Tests.Common;

public class TestInfo
{
    public object[]? TestData { get; init; }
    public required MethodInfo MethodInfo { get; init; }
    public required Type TestClassType { get; init; }

    public virtual object CreateTestClass(IServiceProvider serviceProvider)
    {
        return serviceProvider.GetRequiredService(this.TestClassType);
    }

    public virtual ITestFixture? GetTestFixture() =>
        this.TestData?.FirstOrDefault() as ITestFixture;

    public SourceInfo? SourceInfo => this.GetTestFixture()?.SourceInfo;

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

        // Asserter.AssertedEqual2 += OnAssertedEqual;

        try
        {
            await this.RunAndThrow(serviceProvider);
        }
        catch (Exception ex)
        {
            ;
        }
        finally
        {
            // Asserter.AssertedEqual2 -= OnAssertedEqual;
        }
    }

    private async Task RunAndThrow(IServiceProvider serviceProvider)
    {
        using var testScope = serviceProvider.CreateScope();
        object? testClass = this.CreateTestClass(testScope.ServiceProvider);
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

public class ClassWithModelFixtureTestInfo : TestInfo
{
    [SetsRequiredMembers]
    public ClassWithModelFixtureTestInfo(
        MethodInfo methodInfo,
        Type testClassType,
        ModelFixture modelFixture
    )
    {
        this.ModelFixture = modelFixture;
        this.MethodInfo = methodInfo;
        this.TestClassType = testClassType;
    }

    public ModelFixture ModelFixture { get; set; }

    public override object CreateTestClass(IServiceProvider serviceProvider)
    {
        return Activator.CreateInstance(TestClassType, [this.ModelFixture]);
    }

    public override ITestFixture? GetTestFixture() => this.ModelFixture;
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
    NotStarted,
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
