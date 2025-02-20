using BeamOs.Tests.StructuralAnalysis.Integration;
using BeamOs.Tests.StructuralAnalysis.Unit.DirectStiffnessMethod;
using Microsoft.Extensions.DependencyInjection;

namespace BeamOs.Tests.Runtime.TestRunner;

public static class DI
{
    public static IServiceCollection AddTestServices(this IServiceCollection services)
    {
        services.AddScoped<OpenSeesTests>();
        services.AddScoped<DsmElement1dTests>();
        services.AddScoped<DsmModelTests>();

        return services;
    }
}
