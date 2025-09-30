using System.Text.Json.Serialization;
using BeamOs.CodeGen.AiApiClient;
using BeamOs.CodeGen.SpeckleConnectorApi;
using BeamOs.CodeGen.StructuralAnalysisApiClient;
using BeamOs.StructuralAnalysis.Api;
using BeamOs.StructuralAnalysis.Api.Endpoints;
using BeamOs.StructuralAnalysis.Contracts.Common;
using BeamOs.StructuralAnalysis.Sdk;
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
            options.SerializerOptions.TypeInfoResolverChain.Insert(
                0,
                BeamOsWebAppJsonSerializerContext.Default
            );
            options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
        });
        services.AddStructuralAnalysisSdkRequired();
        services.AddSingleton(typeof(IAssemblyMarkerWebApp).Assembly);
        services.RegisterSharedServices<IAssemblyMarkerWebApp>();
        services.RegisterSharedServicesConfigurable<IAssemblyMarkerWebApp>();
        services.AddHttpContextAccessor();
        services.AddHttpClient();
        services.AddCascadingAuthenticationState();
        // services.AddTestServices();

        return services;
    }

    public static IServiceCollection AddWebAppConfigurable(this IServiceCollection services)
    {
        services.AddHttpClient<IStructuralAnalysisApiClientV1, StructuralAnalysisApiClientV1>(
            client => client.BaseAddress = new("http://localhost:5223")
        );
        services.AddHttpClient<IStructuralAnalysisApiClientV2, StructuralAnalysisApiClientV2>(
            client => client.BaseAddress = new("http://localhost:5223")
        );
        services.AddHttpClient<ISpeckleConnectorApi, SpeckleConnectorApi>(client =>
            client.BaseAddress = new("http://localhost:5223")
        );
        services.AddHttpClient<IAiApiClient, AiApiClient>(client =>
            client.BaseAddress = new("http://localhost:5223")
        );
        services.AddScoped<IEditorApiProxyFactory, EditorApiProxyFactory>();
        services.AddTransient<EditorEventsApi>();
        //services.AddHostedService<DatabaseSeeder>();

        return services;
    }
}

public interface IAssemblyMarkerWebApp { }
