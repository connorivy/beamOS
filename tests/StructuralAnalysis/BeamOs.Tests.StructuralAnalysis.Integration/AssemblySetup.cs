using System.Runtime.CompilerServices;
using BeamOs.CodeGen.StructuralAnalysisApiClient;
using BeamOs.Tests.Common;
using Testcontainers.PostgreSql;

namespace BeamOs.Tests.StructuralAnalysis.Integration;

public static partial class AssemblySetup
{
    public static PostgreSqlContainer DbContainer { get; } =
        new PostgreSqlBuilder().WithImage("postgres:15-alpine").Build();

    public static bool ApiIsRunning { get; set; }
    public static bool SetupWebApi { get; set; } = true;
    public static bool SkipOpenSeesTests { get; set; } = BeamOsEnv.IsCiEnv();

    [Before(Assembly)]
    public static async Task Setup()
    {
        if (ApiIsRunning || !SetupWebApi)
        {
            return;
        }

        await DbContainer.StartAsync();

        var webAppFactory = new WebAppFactory(DbContainer.GetConnectionString());

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

    [ModuleInitializer]
    public static void Init() =>
        VerifierSettings.AddExtraSettings(
            settings => settings.DefaultValueHandling = Argon.DefaultValueHandling.Include
        );
}
