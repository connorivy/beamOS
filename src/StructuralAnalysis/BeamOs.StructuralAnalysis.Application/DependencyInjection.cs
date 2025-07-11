using BeamOs.Common.Application;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Application.SystemOperations;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Models;
using Microsoft.Extensions.DependencyInjection;

namespace BeamOs.StructuralAnalysis.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddStructuralAnalysisApplication(
        this IServiceCollection services
    ) =>
        services
            .AddStructuralAnalysisApplicationRequired()
            .AddStructuralAnalysisApplicationConfigurable();

    public static IServiceCollection AddStructuralAnalysisApplicationRequired(
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

    public static IServiceCollection AddStructuralAnalysisApplicationConfigurable(
        this IServiceCollection services
    )
    {
        services.AddScoped<
            ICommandHandler<ModelResourceRequest<DateTimeOffset>, ModelResponse>,
            ModelRestoreCommandHandler
        >();
        return services;
    }
}
