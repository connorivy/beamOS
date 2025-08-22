using BeamOs.CodeGen.StructuralAnalysisApiClient;
using Microsoft.Extensions.DependencyInjection;
using DotWrap;

namespace BeamOs.StructuralAnalysis.Sdk;

[DotWrapExpose]
public static class ModelBuilderFactory
{
    public static BeamOsModelBuilder CreateRemote(IBeamOsModel model, string apiToken)
    {
        var services = new ServiceCollection();
        services.AddBeamOsRemote(apiToken);
        var serviceProvider = services.BuildServiceProvider();
        var apiClient = serviceProvider.GetRequiredService<IStructuralAnalysisApiClientV1>();
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
