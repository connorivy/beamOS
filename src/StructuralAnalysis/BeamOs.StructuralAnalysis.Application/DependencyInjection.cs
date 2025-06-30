using BeamOs.Common.Application;
using Microsoft.Extensions.DependencyInjection;

namespace BeamOs.StructuralAnalysis.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddStructuralAnalysisApplication(
        this IServiceCollection services
    )
    {
        services.AddObjectThatImplementInterface<IAssemblyMarkerStructuralAnalysisApplication>(
            typeof(ICommandHandler<,>),
            ServiceLifetime.Scoped,
            false
        );
        services.AddObjectThatImplementInterface<IAssemblyMarkerStructuralAnalysisApplication>(
            typeof(IQueryHandler<,>),
            ServiceLifetime.Scoped,
            false
        );
        return services;
    }
}
