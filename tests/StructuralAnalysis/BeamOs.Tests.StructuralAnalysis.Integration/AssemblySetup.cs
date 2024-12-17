using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using BeamOs.CodeGen.StructuralAnalysisApiClient;
using Testcontainers.PostgreSql;
using VerifyTests;

namespace BeamOs.Tests.StructuralAnalysis.Integration;

public static class AssemblySetup
{
    private static readonly PostgreSqlContainer dbContainer = new PostgreSqlBuilder()
        .WithImage("postgres:15-alpine")
        .Build();
    private static Process apiProcess;
    public static StructuralAnalysisApiClientV1 StructuralAnalysisApiClient { get; private set; }

    [Before(Assembly)]
    public static async Task Setup()
    {
        await dbContainer.StartAsync();

        bool apiIsRunning = false;
        var client = new HttpClient() { BaseAddress = new($"http://localhost:7071/") };
        var sb = new StringBuilder();
        var errorSb = new StringBuilder();

        string funcDirectory = DirectoryHelper.GetStructuralAnalysisFunctionsDir();

        if (IsCiBuild())
        {
            string settings = /*lang=json,strict*/
            """
{
  "IsEncrypted": false,
  "Values": {
    "AzureWebJobsStorage": "",
    "FUNCTIONS_WORKER_RUNTIME": "dotnet-isolated"
  },
  "ConnectionStrings": {}
}
""";
            File.WriteAllText(Path.Combine(funcDirectory, "local.settings.json"), settings);
        }

        for (int i = 0; i < 1; i++)
        {
            ProcessStartInfo startInfo =
                new()
                {
                    FileName = "func",
                    Arguments = "start",
                    WorkingDirectory = funcDirectory,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    //CreateNoWindow = true,
                };
            startInfo.EnvironmentVariables["TEST_CONNECTION_STRING"] =
                dbContainer.GetConnectionString();

            apiProcess = new Process { StartInfo = startInfo };

            // hookup the eventhandlers to capture the data that is received
            apiProcess.OutputDataReceived += (sender, args) => sb.AppendLine(args.Data);
            apiProcess.ErrorDataReceived += (sender, args) => errorSb.AppendLine(args.Data);

            //// direct start
            //process.StartInfo.UseShellExecute = false;
            bool success = apiProcess.Start();
            apiProcess.BeginOutputReadLine();
            apiProcess.BeginErrorReadLine();

            //process.WaitForExit();

            //var x = sb.ToString();

            TimeSpan timeout = TimeSpan.FromSeconds(25);
            Stopwatch stopwatch = Stopwatch.StartNew();
            while (stopwatch.Elapsed < timeout)
            {
                Thread.Sleep(500);
                if (apiProcess.HasExited)
                {
                    break;
                }

                HttpResponseMessage pingResponse;
                try
                {
                    pingResponse = await client.GetAsync("api/ping");
                }
                catch (System.Net.Http.HttpRequestException)
                {
                    continue;
                }

                if (pingResponse.IsSuccessStatusCode)
                {
                    apiIsRunning = true;
                    break;
                }
            }

            if (apiIsRunning)
            {
                break;
            }
        }

        if (!apiIsRunning)
        {
            throw new InvalidOperationException(
                @$"""
        Api is not up and running,
        Errors : {errorSb}
        OtherLogs : {sb}
"""
            );
        }
        StructuralAnalysisApiClient = new(client);
    }

    [After(Assembly)]
    public static async Task TearDown()
    {
        (string Stdout, string Stderr) x = await dbContainer.GetLogsAsync();

        await dbContainer.StopAsync();
        apiProcess.Kill();
        apiProcess.Dispose();
    }

    [ModuleInitializer]
    public static void Init() =>
        VerifierSettings.AddExtraSettings(
            settings => settings.DefaultValueHandling = Argon.DefaultValueHandling.Include
        );

    private static bool IsCiBuild()
    {
        return bool.TryParse(
                Environment.GetEnvironmentVariable("ContinuousIntegrationEnv"),
                out bool isCiBuild
            ) && isCiBuild;
    }
}
