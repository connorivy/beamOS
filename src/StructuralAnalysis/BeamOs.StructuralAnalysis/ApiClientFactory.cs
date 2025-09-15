using System.Threading.Tasks;
using BeamOs.CodeGen.StructuralAnalysisApiClient;
using BeamOs.StructuralAnalysis.Api;
using BeamOs.StructuralAnalysis.Api.Endpoints;
using BeamOs.StructuralAnalysis.Sdk;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
#if Sqlite
using BeamOs.StructuralAnalysis.Infrastructure;
#endif

namespace BeamOs.StructuralAnalysis;

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
            .AddStructuralAnalysisConfigurable();

#if Sqlite
        var sqliteConnection = DI_Sqlite.AddSqliteInMemoryAndReturnConnection(services);
        services
            .AddStructuralAnalysisInfrastructureRequired()
            .AddStructuralAnalysisInfrastructureConfigurable("dummy");
#endif

#if !CODEGEN
        services.AddScoped<IStructuralAnalysisApiClientV2, InMemoryApiClient2>();
#endif
        services.AddLogging(builder => builder.SetMinimumLevel(LogLevel.Debug));

        var serviceProvider = services.BuildServiceProvider();
        var client = serviceProvider.GetRequiredService<BeamOsResultApiClient>();
#if Sqlite
        using var scope = serviceProvider.CreateScope();
#pragma warning disable IL3050 // Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.
        scope.EnsureDbCreated();
#pragma warning restore IL3050 // Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.
        client.Disposables.Add(sqliteConnection);
#endif
        return client;
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
public sealed class BeamOsResultApiClient : BeamOsFluentResultApiClient, IDisposable
{
    internal List<IDisposable> Disposables { get; } = [];

    public BeamOsResultApiClient(IStructuralAnalysisApiClientV2 apiClient)
        : base(apiClient) { }

    //     public BeamOsResultApiClient(HttpClient httpClient)
    // #if CODEGEN
    //         : base(default) { }
    // #else
    //         : base(new StructuralAnalysisApiClientV2(httpClient)) { }
    // #endif
    public void Dispose()
    {
        foreach (var disposable in this.Disposables)
        {
            disposable.Dispose();
        }
    }
}
