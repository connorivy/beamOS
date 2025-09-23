using BeamOs.CodeGen.StructuralAnalysisApiClient;
using BeamOs.StructuralAnalysis.Api;
using BeamOs.StructuralAnalysis.Api.Endpoints;
using BeamOs.StructuralAnalysis.Infrastructure;
using BeamOs.StructuralAnalysis.Sdk;
using Microsoft.Extensions.DependencyInjection;

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

#if Sqlite || InMemory
    public static BeamOsApiClient CreateLocal()
    {
        var services = new ServiceCollection();
        services.AddBeamOsLocal();

        var serviceProvider = services.BuildServiceProvider();
        var apiClient = serviceProvider.GetRequiredService<IStructuralAnalysisApiClientV2>();

        return new BeamOsApiClient(apiClient);
    }

    public static BeamOsResultApiClient CreateResultLocal()
    {
        var services = new ServiceCollection();
        services.AddBeamOsLocal();

#if !CODEGEN
        services.AddScoped<IStructuralAnalysisApiClientV2, InMemoryApiClient2>();
#endif

        var serviceProvider = services.BuildServiceProvider();
        var client = serviceProvider.GetRequiredService<BeamOsResultApiClient>();

#if Sqlite
        using var scope = serviceProvider.CreateScope();
        scope.EnsureDbCreated();
#endif
        // client.Disposables.Add(sqliteConnection);
        return client;
    }
#endif
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
