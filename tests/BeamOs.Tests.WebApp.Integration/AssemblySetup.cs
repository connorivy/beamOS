using BeamOs.CodeGen.StructuralAnalysisApiClient;
using BeamOs.StructuralAnalysis;
using BeamOs.StructuralAnalysis.Api;
using BeamOs.StructuralAnalysis.Api.Endpoints;
using BeamOs.StructuralAnalysis.Infrastructure;
using BeamOs.Tests.Common;
using BeamOs.Tests.Common.Integration;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TUnit.Core;

namespace BeamOs.Tests.WebApp.Integration;

public class AssemblySetup
{
    private static WebApplicationFactory<IAssemblyMarkerStructuralAnalysisApi>? backendFactory;
    public static string FrontendAddress { get; set; }
    public static BeamOsResultApiClient BeamOsResultApiClient { get; private set; }
    private static HttpClient? client;
    public static bool Initialized { get; set; }

    [Before(TUnit.Core.HookType.Assembly)]
    public static async Task Setup()
    {
        if (Initialized)
        {
            return;
        }
        await DbTestContainer.InitializeAsync();

        var backend = new BlazorApplicationFactory<IAssemblyMarkerStructuralAnalysisApi>(
            builder =>
            {
                Environment.SetEnvironmentVariable(
                    "TEST_CONNECTION_STRING",
                    DbTestContainer.GetConnectionString()
                );
                Environment.SetEnvironmentVariable("DB_INITIALIZED", "true");
                builder.ConfigureServices(services =>
                {
                    using IServiceScope scope = services.BuildServiceProvider().CreateScope();
                    var structuralDbContext =
                        scope.ServiceProvider.GetRequiredService<StructuralAnalysisDbContext>();

                    if (BeamOsEnv.IsCiEnv())
                    {
                        structuralDbContext.Database.Migrate();
                    }
                    else
                    {
                        structuralDbContext.Database.EnsureCreated();
                    }
                });
            },
            7071
        );
        await backend.StartAsync();
        backendFactory = backend;

        var httpClient = new HttpClient() { BaseAddress = new Uri(backend.ServerAddress) };
        var apiClient = new StructuralAnalysisApiClientV1(httpClient);
        BeamOsResultApiClient = new BeamOsResultApiClient(
            new StructuralAnalysisApiClientV2(apiClient)
        );

        // WebAppFactory = new BlazorApplicationFactory<_Imports>(builder =>
        // {
        //     builder.ConfigureServices(services =>
        //     {
        //         var apiClient = new StructuralAnalysisApiClientV1(client!);
        //         services.RemoveAll<IStructuralAnalysisApiClientV1>();
        //         services.AddSingleton<IStructuralAnalysisApiClientV1>(apiClient);
        //     });
        // });
        // await WebAppFactory.StartAsync();
        // FrontendAddress = WebAppFactory.ServerAddress;
        FrontendAddress = "http://localhost:3000";
        Initialized = true;
    }

    [After(TUnit.Core.HookType.Assembly)]
    public static async Task TearDown()
    {
        client?.Dispose();
    }

    public static Func<PageContext, Task> CreateAuthenticatedUser { get; set; } =
        (pageContext) => Task.CompletedTask;
}
