using BeamOs.CodeGen.StructuralAnalysisApiClient;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Api;
using BeamOs.StructuralAnalysis.Api.Endpoints;
using BeamOs.StructuralAnalysis.Sdk;
using DotWrap;
using Microsoft.Extensions.DependencyInjection;
#if Sqlite
using BeamOs.StructuralAnalysis.Infrastructure;
#endif

namespace BeamOs.StructuralAnalysis;

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

    public static BeamOsResultApiClient CreateResultRemote(string apiToken)
    {
        var services = new ServiceCollection();
        services.AddBeamOsRemote(apiToken);
        services.AddStructuralAnalysisSdkRequired();
        var serviceProvider = services.BuildServiceProvider();
        return serviceProvider.GetRequiredService<BeamOsResultApiClient>();
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
