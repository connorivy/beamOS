using BeamOs.Common.Application;
using BeamOs.StructuralAnalysis.Application.Common;
using Microsoft.Extensions.DependencyInjection;

namespace BeamOs.StructuralAnalysis.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddStructuralAnalysisApplication(
        this IServiceCollection services
    )
    {
        return services.AddObjectThatImplementInterface<IAssemblyMarkerStructuralAnalysisApplication>(
            typeof(ICommandHandler<,>),
            ServiceLifetime.Scoped,
            false,
            true
        );
    }
}
