using BeamOs.CodeGen.StructuralAnalysisApiClient;
using BeamOs.StructuralAnalysis.Api;
using BeamOs.StructuralAnalysis.Api.Endpoints;
using Microsoft.Extensions.DependencyInjection;

namespace BeamOs.StructuralAnalysis.Sdk;

// [DotWrapExpose]
internal static class ApiClientFactory
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
        var apiClient = serviceProvider.GetRequiredKeyedService<IStructuralAnalysisApiClientV2>(
            "InMemory"
        );

        InMemoryApiClient2 x = default;
        return new BeamOsApiClient(apiClient);
        // return new BeamOsModelBuilder(model, apiClient);
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
internal sealed class BeamOsApiClient : BeamOsFluentApiClient
// IDisposable
{
    internal BeamOsApiClient(IStructuralAnalysisApiClientV2 apiClient)
        : base(apiClient) { }

    public BeamOsApiClient(HttpClient httpClient)
        : base(new StructuralAnalysisApiClientV2(httpClient)) { }

    // public void Dispose()
}

// [DotWrapExpose]
internal sealed class BeamOsResultApiClient : BeamOsFluentResultApiClient
{
    internal BeamOsResultApiClient(IStructuralAnalysisApiClientV2 apiClient)
        : base(apiClient) { }

    public BeamOsResultApiClient(HttpClient httpClient)
        : base(new StructuralAnalysisApiClientV2(httpClient)) { }
}
