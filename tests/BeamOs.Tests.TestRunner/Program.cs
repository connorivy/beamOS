using System.Reflection;
using BeamOs.Api.IntegrationTests;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Xunit;
using Xunit.Runners;

namespace BeamOs.Tests.TestRunner;

public class Program
{
    // We use consoleLock because messages can arrive in parallel, so we want to make sure we get
    // consistent console output.
    static object consoleLock = new object();

    // Use an event to know when we're done
    static ManualResetEvent finished = new ManualResetEvent(false);

    // Start out assuming success; we'll set this to 1 if we get a failed test
    static int result = 0;

    public static async Task Main(string[] args)
    {
        HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);
        builder.Services.AddTransient<CustomWebApplicationFactory<BeamOs.Api.Program>>();
        builder.Services.RegisterTestClasses();
        using IHost host = builder.Build();
        //var testAssembly = args[0];
        //var typeNames = new List<string>();
        //for (int idx = 1; idx < args.Length; ++idx)
        //    typeNames.Add(args[idx]);
        foreach (var testAssembly in AssemblyScanning.GetTestingAssemblies())
        {
            foreach (
                var exportedType in testAssembly
                    .GetExportedTypes()
                    .Where(t => !t.IsInterface && !t.IsAbstract)
            )
            {
                object? testClassInstance = null;
                foreach (var methodInfo in exportedType.GetMethods())
                {
                    TestInfo[] testInfo = AssemblyScanning
                        .GetTestInfoFromMethod(methodInfo, exportedType)
                        .ToArray();
                    foreach (var test in testInfo)
                    {
                        testClassInstance ??= host.Services.GetService(exportedType);
                        await (Task)methodInfo.Invoke(testClassInstance, test.TestData);
                    }
                }
            }
            Assembly.LoadFrom(testAssembly.Location);
            using (var runner = AssemblyRunner.WithoutAppDomain(testAssembly.GetName().Name))
            {
                runner.OnDiscoveryComplete = OnDiscoveryComplete;
                runner.OnExecutionComplete = OnExecutionComplete;
                runner.OnTestFailed = OnTestFailed;
                runner.OnTestPassed += OnTestPassed;
                runner.OnTestSkipped = OnTestSkipped;

                Console.WriteLine("Discovering...");

                //var options = new AssemblyRunnerStartOptions { TypesToRun = typeNames.ToArray() };
                runner.Start(parallelAlgorithm: null);

                finished.WaitOne();
                finished.Dispose();

                //return result;
            }
        }
    }

    private static void OnTestPassed(TestPassedInfo info) => throw new NotImplementedException();

    static void OnDiscoveryComplete(DiscoveryCompleteInfo info)
    {
        lock (consoleLock)
            Console.WriteLine(
                $"Running {info.TestCasesToRun} of {info.TestCasesDiscovered} tests..."
            );
    }

    static void OnExecutionComplete(ExecutionCompleteInfo info)
    {
        lock (consoleLock)
            Console.WriteLine(
                $"Finished: {info.TotalTests} tests in {Math.Round(info.ExecutionTime, 3)}s ({info.TestsFailed} failed, {info.TestsSkipped} skipped)"
            );

        finished.Set();
    }

    static void OnTestFailed(TestFailedInfo info)
    {
        lock (consoleLock)
        {
            Console.ForegroundColor = ConsoleColor.Red;

            Console.WriteLine("[FAIL] {0}: {1}", info.TestDisplayName, info.ExceptionMessage);
            if (info.ExceptionStackTrace != null)
                Console.WriteLine(info.ExceptionStackTrace);

            Console.ResetColor();
        }

        result = 1;
    }

    static void OnTestSkipped(TestSkippedInfo info)
    {
        lock (consoleLock)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("[SKIP] {0}: {1}", info.TestDisplayName, info.SkipReason);
            Console.ResetColor();
        }
    }
}
