using System.Threading.Tasks;
using BeamOs.CodeGen.StructuralAnalysisApiClient;
using BeamOs.StructuralAnalysis.Api.Endpoints;
using DotWrap;
using Microsoft.Extensions.DependencyInjection;

namespace BeamOs.StructuralAnalysis.Sdk;

[DotWrapExpose]
public static class ModelBuilderFactory
{
    public static async Task<BeamOsModelBuilder> CreateRemote(IBeamOsModel model, string apiToken)
    {
        var services = new ServiceCollection();
        services.AddBeamOsRemote(apiToken);
        var serviceProvider = services.BuildServiceProvider();
        var apiClient = serviceProvider.GetRequiredService<IStructuralAnalysisApiClientV1>();
        ApiClient x = default!;
        var w = x.models[model.Id];
        var y = await x.models[model.Id].analyze.opensees.RunOpenSeesAnalysisAsync(null, default);
        var z = await x.models[Guid.Empty].result_sets[0].GetResultSetAsync(default);
        var resultSet = await w.result_sets[1].GetResultSetAsync(default);
        // var resultSet2 = await w.
        // var asdf = resultSet.Value.NodeResults
        var a = z.Value.NodeResults[0].Displacements.DisplacementAlongX;
        // TaskDotWrapWrapper_BeamOs
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

[DotWrapExpose]
public sealed class ApiClient : FluentApiClient
{
    public ApiClient(IStructuralAnalysisApiClientV1 apiClient)
        : base(apiClient) { }
}
