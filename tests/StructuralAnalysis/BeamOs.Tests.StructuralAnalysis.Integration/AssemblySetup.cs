using System.Runtime.CompilerServices;
using BeamOs.CodeGen.StructuralAnalysisApiClient;
using Testcontainers.PostgreSql;

namespace BeamOs.Tests.StructuralAnalysis.Integration;

public static class AssemblySetup
{
    public static PostgreSqlContainer DbContainer { get; } =
        new PostgreSqlBuilder().WithImage("postgres:15-alpine").Build();

    public static StructuralAnalysisApiClientV1 StructuralAnalysisApiClient { get; set; }
    public static bool ApiIsRunning { get; set; }
    public static bool SetupWebApi { get; set; } = true;

    [Before(Assembly)]
    public static async Task Setup()
    {
        if (ApiIsRunning || !SetupWebApi)
        {
            return;
        }

        await DbContainer.StartAsync();

        var webAppFactory = new WebAppFactory(DbContainer.GetConnectionString());

        StructuralAnalysisApiClient = new(webAppFactory.CreateClient());

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
