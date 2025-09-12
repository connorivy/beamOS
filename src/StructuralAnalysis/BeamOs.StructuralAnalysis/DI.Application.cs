using BeamOs.Common.Application;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Application.Common;
using BeamOs.StructuralAnalysis.Application.SystemOperations;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Models;
using Microsoft.Extensions.DependencyInjection;
using ServiceScan.SourceGenerator;

namespace BeamOs.StructuralAnalysis.Application;

internal static partial class DependencyInjection
{
    public static IServiceCollection AddStructuralAnalysisApplicationRequired(
        this IServiceCollection services
    )
    {
        services.AddCommandHandlers();
        services.AddQueryHandlers();
        services.AddSingleton<InMemoryModelRepositoryStorage>();
        // services.AddObjectThatImplementInterface<IAssemblyMarkerStructuralAnalysisApplication>(
        //     typeof(ICommandHandler<,>),
        //     ServiceLifetime.Scoped,
        //     false
        // );
        // services.AddObjectThatImplementInterface<IAssemblyMarkerStructuralAnalysisApplication>(
        //     typeof(IQueryHandler<,>),
        //     ServiceLifetime.Scoped,
        //     false
        // );
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

    [GenerateServiceRegistrations(
        AssignableTo = typeof(ICommandHandler<,>),
        Lifetime = ServiceLifetime.Scoped,
        AsSelf = true
    )]
    public static partial IServiceCollection AddCommandHandlers(this IServiceCollection services);

    [GenerateServiceRegistrations(
        AssignableTo = typeof(IQueryHandler<,>),
        Lifetime = ServiceLifetime.Scoped,
        AsSelf = true
    )]
    public static partial IServiceCollection AddQueryHandlers(this IServiceCollection services);
}
