using System.Text.Json.Serialization;
using BeamOs.CodeGen.StructuralAnalysisApiClient;
using BeamOs.StructuralAnalysis.Contracts.Common;
using BeamOs.StructuralAnalysis.CsSdk;
using BeamOs.Tests.Common;
using BeamOs.Tests.Common.SolvedProblems.Kassimali_MatrixAnalysisOfStructures2ndEd;
using BeamOs.WebApp.Components;
using BeamOs.WebApp.Components.Features.Common;
using BeamOs.WebApp.Components.Features.Editors.ReadOnlyEditor;

namespace BeamOs.WebApp;

public static class DI
{
    public static IServiceCollection AddWebAppRequired(this IServiceCollection services)
    {
        services.ConfigureHttpJsonOptions(options =>
        {
            BeamOsSerializerOptions.DefaultConfig(options.SerializerOptions);
            options
                .SerializerOptions
                .TypeInfoResolverChain
                .Insert(0, BeamOsWebAppJsonSerializerContext.Default);
            options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
        });
        services.AddSingleton(typeof(IAssemblyMarkerWebApp).Assembly);
        services.RegisterSharedServices<IAssemblyMarkerWebApp>();
        services.AddHttpContextAccessor();
        services.AddHttpClient();
        services.AddCascadingAuthenticationState();

        return services;
    }

    public static IServiceCollection AddWebAppConfigurable(this IServiceCollection services)
    {
        services.AddHttpClient<IStructuralAnalysisApiClientV1, StructuralAnalysisApiClientV1>(
            client => client.BaseAddress = new("http://localhost:5223")
        );
        services.AddScoped<IEditorApiProxyFactory, EditorApiProxyFactory>();
        services.AddTransient<EditorEventsApi>();
        //services.AddHostedService<DatabaseSeeder>();

        return services;
    }

    public static async Task InitializeBeamOsData(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            //Thread.Sleep(5000);
            using var scope = app.Services.CreateScope();
            var apiClient = scope
                .ServiceProvider
                .GetRequiredService<IStructuralAnalysisApiClientV1>();

            foreach (var modelBuilder in AllSolvedProblems.ModelFixtures())
            {
                await modelBuilder.Build(apiClient);
            }
        }
    }
}

public interface IAssemblyMarkerWebApp { }

public class DatabaseSeeder(IServiceProvider serviceProvider) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = serviceProvider.CreateScope();
        var apiClientV1 = scope
            .ServiceProvider
            .GetRequiredService<IStructuralAnalysisApiClientV1>();

        foreach (var modelBuilder in this.ModelFixtures())
        {
            await modelBuilder.Build(apiClientV1);
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    public IEnumerable<BeamOsModelBuilder> ModelFixtures()
    {
        yield return new Kassimali_Example3_8();
        yield return new Kassimali_Example8_4();
    }
}
