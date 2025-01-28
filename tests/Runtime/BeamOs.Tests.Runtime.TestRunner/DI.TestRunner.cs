using BeamOs.Tests.StructuralAnalysis.Integration;
using Microsoft.Extensions.DependencyInjection;

namespace BeamOs.Tests.Runtime.TestRunner;

public static class DI
{
    public static IServiceCollection AddTestServices(this IServiceCollection services)
    {
        services.AddScoped<OpenSeesTests>();

        return services;
    }
}
