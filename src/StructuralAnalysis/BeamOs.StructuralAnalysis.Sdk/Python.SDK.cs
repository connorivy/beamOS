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
        var serviceProvider = services.BuildServiceProvider();
        var apiClient = serviceProvider.GetRequiredService<IStructuralAnalysisApiClientV1>();
        ApiClient x = default!;
        var asdf = await x.Models[default].Nodes[0].PutNodeAsync(default);
        BeamOsFluentApiClient z = default!;
        BeamOsFluentResultApiClient zz = default!;

        BeamOs.StructuralAnalysis.Api.IStructuralAnalysisApiClientV2 erd = default!;
        InMemoryApiClient2 zzz = default!;
        InMemoryApiClient zzx = default!;
        z.Models[default].Nodes[0].DeleteNodeAsync();
        z.Models[default].Nodes[0].Internal.GetInternalNodeAsync();
        var w = x.Models[model.Id];
        var y = await w.Proposals.GetModelProposalsAsync();
        return new BeamOsModelBuilder(model, apiClient);
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
        return new BeamOsModelBuilder(model, apiClient);
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
