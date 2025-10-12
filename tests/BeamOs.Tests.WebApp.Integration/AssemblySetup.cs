using System.Diagnostics;
using BeamOs.CodeGen.StructuralAnalysisApiClient;
using BeamOs.StructuralAnalysis.Api;
using BeamOs.Tests.Common.Integration;
using BeamOs.WebApp;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace BeamOs.Tests.WebApp.Integration;

public class AssemblySetup
{
    private static WebApplicationFactory<IAssemblyMarkerStructuralAnalysisApi>? backendFactory;
    public static BlazorApplicationFactory<_Imports> WebAppFactory { get; private set; } = null!;
    private static HttpClient? client;
    private static Process? structuralAnalysisApiProcess;
    private const int StructuralAnalysisApiPort = 7071;

    [Before(TUnit.Core.HookType.Assembly)]
    public static async Task Setup()
    {
        await DbTestContainer.InitializeAsync();

        // var x = new BlazorApplicationFactory<IAssemblyMarkerStructuralAnalysisApi>();
        // await x.StartAsync();
        // factory = x;
        backendFactory = new WebApplicationFactory<IAssemblyMarkerStructuralAnalysisApi>();
        client = backendFactory.CreateClient();

        WebAppFactory = new BlazorApplicationFactory<_Imports>(builder =>
        {
            builder.ConfigureServices(services =>
            {
                var apiClient = new StructuralAnalysisApiClientV1(client!);
                services.RemoveAll<IStructuralAnalysisApiClientV1>();
                services.AddSingleton<IStructuralAnalysisApiClientV1>(apiClient);
            });
        });
        await WebAppFactory.StartAsync();
    }

    public static async Task StartStructuralAnalysisApi()
    {
        var projectPath =
            "/workspaces/beamOS.Admin/beamOS/src/StructuralAnalysis/BeamOs.StructuralAnalysis.Api";

        var processStartInfo = new ProcessStartInfo
        {
            FileName = "dotnet",
            Arguments =
                $"run --project {projectPath} --urls=http://localhost:{StructuralAnalysisApiPort}",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
        };

        var process = new Process { StartInfo = processStartInfo };
        process.OutputDataReceived += (sender, args) =>
        {
            if (!string.IsNullOrEmpty(args.Data))
            {
                Console.WriteLine($"[StructuralAnalysis API] {args.Data}");
            }
        };
        process.ErrorDataReceived += (sender, args) =>
        {
            if (!string.IsNullOrEmpty(args.Data))
            {
                Console.Error.WriteLine($"[StructuralAnalysis API ERROR] {args.Data}");
            }
        };

        process.Start();
        process.BeginOutputReadLine();
        process.BeginErrorReadLine();

        structuralAnalysisApiProcess = process;

        // Give the server some time to start
        await Task.Delay(45000);
    }

    public static async Task<bool> IsPortOpen(
        string scheme,
        int port,
        string? path,
        TimeSpan timeout
    )
    {
        try
        {
            using var httpClient = new HttpClient();
            while (timeout > TimeSpan.Zero)
            {
                try
                {
                    var response = await httpClient.GetAsync($"{scheme}://localhost:{port}/{path}");
                    return response.IsSuccessStatusCode;
                }
                catch
                {
                    await Task.Delay(200);
                    timeout -= TimeSpan.FromMilliseconds(200);
                }
            }
        }
        catch
        {
            return false;
        }
        return false;
    }

    [After(TUnit.Core.HookType.Assembly)]
    public static async Task TearDown()
    {
        // Clean up the StructuralAnalysis API process if we started it
        if (structuralAnalysisApiProcess != null && !structuralAnalysisApiProcess.HasExited)
        {
            try
            {
                structuralAnalysisApiProcess.Kill();
                await structuralAnalysisApiProcess.WaitForExitAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error stopping StructuralAnalysis API process: {ex.Message}");
            }
            finally
            {
                structuralAnalysisApiProcess?.Dispose();
            }
        }

        client?.Dispose();
        WebAppFactory?.Dispose();
    }
}
