using BeamOs.Common.Api;
using BeamOs.Common.Application;
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
    )
    {
        services.AddStructuralAnalysisInfrastructureConfigurable(connectionString);
        return services;
    }

    public static IServiceCollection AddStructuralAnalysisApi(this IServiceCollection services)
    {
        services.AddObjectThatExtendsBase<IAssemblyMarkerStructuralAnalysisApiEndpoints>(
            typeof(BeamOsBaseEndpoint<,>),
            ServiceLifetime.Scoped
        );

        services.AddObjectThatImplementInterface<IAssemblyMarkerStructuralAnalysisApiEndpoints>(
            typeof(ICommandHandler<,>),
            ServiceLifetime.Scoped,
            false
        );
        return services;
    }
}
