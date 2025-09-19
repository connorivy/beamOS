using BeamOs.CodeGen.SpeckleConnectorApi;
using BeamOs.CodeGen.StructuralAnalysisApiClient;
using BeamOs.Common.Api;
using BeamOs.StructuralAnalysis;
using BeamOs.StructuralAnalysis.Api;
using BeamOs.StructuralAnalysis.Api.Endpoints;
using BeamOs.StructuralAnalysis.Sdk;
using Microsoft.Extensions.DependencyInjection;

namespace BeamOs.Tests.StructuralAnalysis.Integration;

public static partial class AssemblySetup
{
    public static BeamOsResultApiClient StructuralAnalysisApiClient { get; set; }

    public static Func<BeamOsResultApiClient> GetStructuralAnalysisApiClientV1() =>
        () =>
        {
            return StructuralAnalysisApiClient;
        };

    public static BeamOsResultApiClient CreateApiClientWebAppFactory(HttpClient httpClient)
    {
        var services = new ServiceCollection();
        services.AddBeamOsRemoteTest(httpClient);
        var serviceProvider = services.BuildServiceProvider();
        return serviceProvider.GetRequiredService<BeamOsResultApiClient>();
    }

    private static IServiceCollection AddBeamOsRemoteTest(
        this IServiceCollection services,
        HttpClient httpClient
    )
    {
        services.AddSingleton(httpClient);

        services.AddScoped<IStructuralAnalysisApiClientV1, StructuralAnalysisApiClientV1>();
        services.AddScoped<ISpeckleConnectorApi, SpeckleConnectorApi>();

        services.AddStructuralAnalysisSdkRequired();

        return services;
    }
}
