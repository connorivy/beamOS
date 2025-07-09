using System.Diagnostics;
using System.Runtime.CompilerServices;
using BeamOs.CodeGen.StructuralAnalysisApiClient;
using BeamOs.Tests.Common;
using DiffEngine;
using Testcontainers.PostgreSql;

namespace BeamOs.Tests.StructuralAnalysis.Integration;

public static partial class AssemblySetup
{
    public static PostgreSqlContainer DbContainer { get; } =
        new PostgreSqlBuilder().WithImage("postgres:15-alpine").Build();

    public static bool ApiIsRunning { get; set; }
    public static bool SetupWebApi { get; set; } = true;
    public static bool SkipOpenSeesTests { get; set; } = BeamOsEnv.IsCiEnv();
    private static readonly char[] separator = [' ', '\n', '\r'];

    [Before(HookType.Assembly)]
    public static async Task Setup()
    {
        if (ApiIsRunning || !SetupWebApi)
        {
            return;
        }

        await DbContainer.StartAsync();

        TestUtils.Asserter = new();

        var webAppFactory = new WebAppFactory(
            $"{DbContainer.GetConnectionString()};Include Error Detail=True"
        );

        StructuralAnalysisApiClient = new StructuralAnalysisApiClientV1(
            webAppFactory.CreateClient()
        );

        ApiIsRunning = true;
    }

    public static async Task TearDown()
    {
        (string Stdout, string Stderr) x = await DbContainer.GetLogsAsync();

        await DbContainer.StopAsync();
    }

    private static bool isFirstTestRun = true;
    private static Lock firstRunLock = new();

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

        if (BeamOsEnv.IsDevContainer())
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
                VerifierSettings.OnVerifyMismatch(
                    async (filePair, message, autoVerify) =>
                    {
                        // string endflags;
                        // lock (firstRunLock)
                        // {
                        //     if (isFirstTestRun)
                        //     {
                        //         endflags = " --new-window";
                        //         isFirstTestRun = false;
                        //     }
                        //     else
                        //     {
                        //         endflags = " --reuse-window";
                        //     }
                        // }

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
