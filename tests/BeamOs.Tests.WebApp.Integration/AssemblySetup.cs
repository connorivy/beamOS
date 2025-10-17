using BeamOs.CodeGen.StructuralAnalysisApiClient;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Api;
using BeamOs.StructuralAnalysis.Infrastructure;
using BeamOs.Tests.Common;
using BeamOs.Tests.Common.Integration;
using BeamOs.WebApp;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace BeamOs.Tests.WebApp.Integration;

public class AssemblySetup
{
    private static WebApplicationFactory<IAssemblyMarkerStructuralAnalysisApi>? backendFactory;
    public static BlazorApplicationFactory<_Imports> WebAppFactory { get; private set; } = null!;
    public static string FrontendAddress { get; set; }
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
        // client = backendFactory.CreateClient();

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
        FrontendAddress = "http://localhost:5173";
        Initialized = true;
    }

    [After(TUnit.Core.HookType.Assembly)]
    public static async Task TearDown()
    {
        client?.Dispose();
        WebAppFactory?.Dispose();
    }

    public static Func<PageContext, Task> CreateAuthenticatedUser { get; set; } =
        (pageContext) => Task.CompletedTask;
}
