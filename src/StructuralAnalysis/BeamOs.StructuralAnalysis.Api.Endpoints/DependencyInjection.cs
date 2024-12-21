using BeamOs.Common.Api;
using BeamOs.StructuralAnalysis.Application;
using BeamOs.StructuralAnalysis.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace BeamOs.StructuralAnalysis.Api.Endpoints;

public static class DependencyInjection
{
    public static IServiceCollection AddStructuralAnalysisRequired(
        this IServiceCollection services
    ) =>
        services
            .AddStructuralAnalysisApplication()
            .AddStructuralAnalysisInfrastructureRequired()
            .AddStructuralAnalysisApi();

    public static IServiceCollection AddStructuralAnalysisConfigurable(
        this IServiceCollection services,
        string connectionString
    ) =>
        services
        //.AddStructuralAnalysisApplication()
        //.AddStructuralAnalysisApi()
        .AddStructuralAnalysisInfrastructureConfigurable(connectionString);

    public static IServiceCollection AddStructuralAnalysisApi(this IServiceCollection services)
    {
        return services.AddObjectThatExtendsBase<IAssemblyMarkerStructuralAnalysisApiShared>(
            typeof(BeamOsBaseEndpoint<,>),
            ServiceLifetime.Scoped
        );
    }
}
