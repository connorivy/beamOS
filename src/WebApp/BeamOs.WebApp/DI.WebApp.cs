using System.Text.Json.Serialization;
using BeamOs.CodeGen.StructuralAnalysisApiClient;
using BeamOs.StructuralAnalysis.Contracts.Common;
using BeamOs.StructuralAnalysis.CsSdk;
using BeamOs.Tests.Common;
using BeamOs.Tests.Common.SolvedProblems.Kassimali_MatrixAnalysisOfStructures2ndEd;
using BeamOs.Tests.Runtime.TestRunner;
using BeamOs.WebApp.Components;
using BeamOs.WebApp.Components.Features.Common;
using BeamOs.WebApp.Components.Features.Editor;

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
        services.RegisterSharedServicesConfigurable<IAssemblyMarkerWebApp>();
        services.AddHttpContextAccessor();
        services.AddHttpClient();
        services.AddCascadingAuthenticationState();
        services.AddTestServices();

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
                if (await modelBuilder.CreateIfDoesntExist(apiClient))
                {
                    await apiClient.RunOpenSeesAnalysisAsync(modelBuilder.Id);
                }
            }
        }
    }
}

public interface IAssemblyMarkerWebApp { }
