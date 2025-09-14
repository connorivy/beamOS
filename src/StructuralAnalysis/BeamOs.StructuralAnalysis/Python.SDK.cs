using BeamOs.CodeGen.StructuralAnalysisApiClient;
using BeamOs.StructuralAnalysis.Api;
using BeamOs.StructuralAnalysis.Api.Endpoints;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace BeamOs.StructuralAnalysis.Sdk;

// [DotWrapExpose]
public static class ApiClientFactory
{
    public static BeamOsApiClient CreateRemote(string apiToken)
    {
        var services = new ServiceCollection();
        services.AddBeamOsRemote(apiToken);
        services.AddStructuralAnalysisSdkRequired();
        var serviceProvider = services.BuildServiceProvider();
        return serviceProvider.GetRequiredService<BeamOsApiClient>();
    }

    public static BeamOsApiClient CreateLocal()
    {
        var services = new ServiceCollection();
        services
            .AddStructuralAnalysisSdkRequired()
            .AddStructuralAnalysisRequired()
            .AddStructuralAnalysisConfigurable()
            .AddInMemoryInfrastructure();
#if !CODEGEN
        services.AddScoped<IStructuralAnalysisApiClientV2, InMemoryApiClient2>();
#endif

        var serviceProvider = services.BuildServiceProvider();
        var apiClient = serviceProvider.GetRequiredService<IStructuralAnalysisApiClientV2>();

        // InMemoryApiClient2 x = default;
        return new BeamOsApiClient(apiClient);
        // return new BeamOsModelBuilder(model, apiClient);
    }

    public static BeamOsResultApiClient CreateResultLocal()
    {
        var services = new ServiceCollection();
        services
            .AddStructuralAnalysisSdkRequired()
            .AddStructuralAnalysisRequired()
            .AddStructuralAnalysisConfigurable()
            .AddInMemoryInfrastructure();
#if !CODEGEN
        services.AddScoped<IStructuralAnalysisApiClientV2, InMemoryApiClient2>();
#endif
        services.AddLogging(builder => builder.SetMinimumLevel(LogLevel.Debug));

        var sqliteConnection = DI_Sqlite.AddSqliteInMemoryAndReturnConnection(services);

        var serviceProvider = services.BuildServiceProvider();
        return serviceProvider.GetRequiredService<BeamOsResultApiClient>();
    }
    // public static BeamOsModel Local()
    // {

    // }
    //
    // public static BeamOsModel Hi()
    // {
    // }

    // public static BeamOsModel WebApp() { }
}

// [DotWrapExpose]
public sealed class BeamOsApiClient : BeamOsFluentApiClient
// IDisposable
{
    public BeamOsApiClient(IStructuralAnalysisApiClientV2 apiClient)
        : base(apiClient) { }

    //     public BeamOsApiClient(HttpClient httpClient)
    // #if CODEGEN
    //         : base(default) { }
    // #else
    //         : base(new StructuralAnalysisApiClientV2(httpClient)) { }
    // #endif

    // public void Dispose()
}

// [DotWrapExpose]
public sealed class BeamOsResultApiClient : BeamOsFluentResultApiClient
{
    public BeamOsResultApiClient(IStructuralAnalysisApiClientV2 apiClient)
        : base(apiClient) { }

    //     public BeamOsResultApiClient(HttpClient httpClient)
    // #if CODEGEN
    //         : base(default) { }
    // #else
    //         : base(new StructuralAnalysisApiClientV2(httpClient)) { }
    // #endif
}
