using BeamOS.Tests.Common;

namespace BeamOs.Tests.TestRunner;

public static class TestRunner
{
    public static async Task<TestResult> Run(TestInfo testInfo)
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
            await RunTest(testInfo);
            tcs.SetResult(result ?? throw new Exception("Result was unset"));
        }
        catch (Xunit.SkipException skipEx)
        {
            tcs.SetResult(new TestResult(TestResultStatus.Skipped, skipEx.Message));
        }
        catch (Xunit.Sdk.XunitException xEx)
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

    private static async Task RunTest(TestInfo testInfo)
    {
        object? test = testInfo
            .MethodInfo
            .Invoke(Activator.CreateInstance(testInfo.TestClassType), testInfo.TestData);
        if (test is Task t)
        {
            await t.ConfigureAwait(false);
        }
    }
}
