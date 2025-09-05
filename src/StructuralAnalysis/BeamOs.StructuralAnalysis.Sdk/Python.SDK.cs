using BeamOs.CodeGen.StructuralAnalysisApiClient;
using BeamOs.StructuralAnalysis.Api;
using BeamOs.StructuralAnalysis.Api.Endpoints;
using Microsoft.Extensions.DependencyInjection;

namespace BeamOs.StructuralAnalysis.Sdk;

// [DotWrapExpose]
public static class ApiClientFactory
{
    public static BeamOsApiClient CreateRemote(string apiToken)
    {
        var services = new ServiceCollection();
        services.AddBeamOsRemote(apiToken);
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

        var serviceProvider = services.BuildServiceProvider();
        var apiClient = serviceProvider.GetRequiredKeyedService<IStructuralAnalysisApiClientV1>(
            "InMemory"
        );
        // return new BeamOsModelBuilder(model, apiClient);
        return default;
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
public sealed class ApiClient : BeamOsFluentResultApiClient
{
    public ApiClient(IStructuralAnalysisApiClientV2 apiClient)
        : base(apiClient) { }
}

public sealed class BeamOsApiClient(IStructuralAnalysisApiClientV2 apiClient)
    : BeamOsFluentApiClient(apiClient)
// IDisposable
{
    internal IStructuralAnalysisApiClientV2 ApiClient => apiClient;

    // public void Dispose()
}

public sealed class BeamOsResultApiClient(IStructuralAnalysisApiClientV2 apiClient)
    : BeamOsFluentResultApiClient(apiClient)
{
    internal IStructuralAnalysisApiClientV2 ApiClient => apiClient;
}
