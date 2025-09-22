using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using BeamOs.CodeGen.SpeckleConnectorApi;
using BeamOs.CodeGen.StructuralAnalysisApiClient;
using BeamOs.StructuralAnalysis;
using BeamOs.StructuralAnalysis.Api.Endpoints;
using BeamOs.StructuralAnalysis.Sdk;
using BeamOs.Tests.Common;
using DiffEngine;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.PostgreSql;

namespace BeamOs.Tests.StructuralAnalysis.Integration;

public static partial class AssemblySetup
{
    public static PostgreSqlContainer DbContainer { get; private set; }

    public static bool UseLocalApi { get; }
    public static bool ApiIsRunning { get; set; }
    public static bool SetupWebApi { get; set; } = true;
    public static bool SkipOpenSeesTests { get; set; } = BeamOsEnv.IsCiEnv();
    public static BeamOsResultApiClient StructuralAnalysisRemoteApiClient { get; set; }
    public static BeamOsResultApiClient StructuralAnalysisLocalApiClient { get; set; }
    public static VerifySettings ThisVerifierSettings { get; } = new();

    public static Func<BeamOsResultApiClient> GetStructuralAnalysisApiClientV1() =>
        () =>
        {
            return StructuralAnalysisRemoteApiClient;
        };

    public static BeamOsResultApiClient CreateApiClientWebAppFactory(HttpClient httpClient)
    {
        var services = new ServiceCollection();
        services.AddBeamOsRemoteTest(httpClient);
        var serviceProvider = services.BuildServiceProvider();
        return serviceProvider.GetRequiredService<BeamOsResultApiClient>();
    }

    private static IServiceCollection AddBeamOsRemoteTest(
        this IServiceCollection services,
        HttpClient httpClient
    )
    {
        services.AddSingleton(httpClient);

        services.AddScoped<IStructuralAnalysisApiClientV1, StructuralAnalysisApiClientV1>();
        services.AddScoped<ISpeckleConnectorApi, SpeckleConnectorApi>();

        services.AddStructuralAnalysisSdkRequired();

        return services;
    }

    private static readonly char[] separator = [' ', '\n', '\r'];

    [Before(TUnitHookType.Assembly)]
    public static async Task Setup()
    {
        if (ApiIsRunning || !SetupWebApi)
        {
            return;
        }

        TestUtils.Asserter = new();

        StructuralAnalysisLocalApiClient = ApiClientFactory.CreateResultLocal();
        await InitializeRemoteApiClient();

        ApiIsRunning = true;
    }

    private static async Task InitializeRemoteApiClient()
    {
        DbContainer = new PostgreSqlBuilder().WithImage("postgres:15-alpine").Build();
        await DbContainer.StartAsync();

#pragma warning disable IL3050 // Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.
        var webAppFactory = new WebAppFactory(
            $"{DbContainer.GetConnectionString()};Include Error Detail=True"
        );
#pragma warning restore IL3050 // Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.
        StructuralAnalysisRemoteApiClient = CreateApiClientWebAppFactory(
            webAppFactory.CreateClient()
        );
    }

    // private static void UseLocalApiClient()
    // {
    //     StructuralAnalysisApiClient = CreateApiClientLocal();
    // }

    public static async Task TearDown()
    {
        (string Stdout, string Stderr) x = await DbContainer.GetLogsAsync();

        await DbContainer.StopAsync();
    }

    [ModuleInitializer]
    public static void Init()
    {
        VerifierSettings.AddExtraSettings(settings =>
            settings.DefaultValueHandling = Argon.DefaultValueHandling.Include
        );

        // DiffTools.UseOrder(
        //     DiffTool.VisualStudioCode,
        //     DiffTool.Neovim,
        //     DiffTool.VisualStudio,
        //     DiffTool.Rider
        // );

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            DiffTools.UseOrder(
                DiffTool.VisualStudioCode,
                DiffTool.Neovim,
                DiffTool.VisualStudio,
                DiffTool.Rider
            );
        }
        else
        {
            DiffTools.UseOrder(
                DiffTool.VisualStudio,
                DiffTool.Rider,
                DiffTool.VisualStudioCode,
                DiffTool.Neovim
            );
        }
        {
            using var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "whereis",
                    Arguments = "code",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                },
            };
            process.Start();
            string output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();

            // Split output into paths and find the one containing "vscode-server"
            var vscodeServerPath = output
                .Split(separator, StringSplitOptions.RemoveEmptyEntries)
                .FirstOrDefault(p => p.Contains("vscode-server"));

            // if (!string.IsNullOrEmpty(vscodeServerPath))
            // {
            //     _ = DiffTools.AddToolBasedOn(
            //         DiffTool.VisualStudioCode,
            //         "VS Code in separate window",
            //         launchArguments: new(
            //             Left: (temp, target) => $"--diff \"{target}\" \"{temp}\" -n",
            //             Right: (temp, target) => $"--diff \"{temp}\" \"{target}\" -n"
            //         ),
            //         exePath: vscodeServerPath
            //     );
            // }

            if (!string.IsNullOrEmpty(vscodeServerPath))
            {
                // for some reason the default diff runner does not work in dev containers
                // so I'm just doing it manually for now
                DiffRunner.Disabled = true;
                VerifierSettings.OnFirstVerify(
                    async (filePair, receivedText, autoVerify) =>
                    {
                        File.Create(filePair.VerifiedPath);
                        using var process = new Process
                        {
                            StartInfo = new ProcessStartInfo
                            {
                                FileName = vscodeServerPath,
                                Arguments =
                                    $"--diff \"{filePair.ReceivedPath}\" \"{filePair.VerifiedPath}\"",
                                RedirectStandardOutput = true,
                                UseShellExecute = false,
                                CreateNoWindow = true,
                            },
                        };

                        process.Start();
                        await process.WaitForExitAsync();
                    }
                );
                VerifierSettings.OnVerifyMismatch(
                    async (filePair, message, autoVerify) =>
                    {
                        using var process = new Process
                        {
                            StartInfo = new ProcessStartInfo
                            {
                                FileName = vscodeServerPath,
                                Arguments =
                                    $"--diff \"{filePair.ReceivedPath}\" \"{filePair.VerifiedPath}\"",
                                RedirectStandardOutput = true,
                                UseShellExecute = false,
                                CreateNoWindow = true,
                            },
                        };

                        process.Start();
                        await process.WaitForExitAsync();
                    }
                );
            }
        }
    }
}
