using BeamOs.CodeGen.StructuralAnalysisApiClient;
using BeamOs.StructuralAnalysis.Api;
using BeamOs.StructuralAnalysis.Api.Endpoints;
using DotWrap;
using Microsoft.Extensions.DependencyInjection;

namespace BeamOs.StructuralAnalysis.Sdk;

// [DotWrapExpose]
public static class ModelBuilderFactory
{
    public static async Task<BeamOsModelBuilder> CreateRemote(IBeamOsModel model, string apiToken)
    {
        var services = new ServiceCollection();
        services.AddBeamOsRemote(apiToken);
        return default;
    }

    public static BeamOsModelBuilder CreateLocal(IBeamOsModel model)
    {
        var services = new ServiceCollection();
        services
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
{
    internal IStructuralAnalysisApiClientV2 ApiClient => apiClient;
}

public sealed class BeamOsResultApiClient(IStructuralAnalysisApiClientV2 apiClient)
    : BeamOsFluentResultApiClient(apiClient)
{
    internal IStructuralAnalysisApiClientV2 ApiClient => apiClient;
}
