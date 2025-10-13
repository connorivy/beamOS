using System.Diagnostics;
using BeamOs.CodeGen.StructuralAnalysisApiClient;
using BeamOs.StructuralAnalysis.Api;
using BeamOs.StructuralAnalysis.Infrastructure;
using BeamOs.Tests.Common;
using BeamOs.Tests.Common.Integration;
using BeamOs.WebApp;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace BeamOs.Tests.WebApp.Integration;

public class AssemblySetup
{
    private static WebApplicationFactory<IAssemblyMarkerStructuralAnalysisApi>? backendFactory;
    public static BlazorApplicationFactory<_Imports> WebAppFactory { get; private set; } = null!;
    private static HttpClient? client;

    [Before(TUnit.Core.HookType.Assembly)]
    public static async Task Setup()
    {
        await DbTestContainer.InitializeAsync();

        // var x = new BlazorApplicationFactory<IAssemblyMarkerStructuralAnalysisApi>();
        // await x.StartAsync();
        // factory = x;
        backendFactory =
            new WebApplicationFactory<IAssemblyMarkerStructuralAnalysisApi>().WithWebHostBuilder(
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
                }
            );
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

    [After(TUnit.Core.HookType.Assembly)]
    public static async Task TearDown()
    {
        client?.Dispose();
        WebAppFactory?.Dispose();
    }
}
