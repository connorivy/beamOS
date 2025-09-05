using BeamOs.StructuralAnalysis.Domain.DirectStiffnessMethod;
using BeamOs.StructuralAnalysis.Sdk;
using BeamOs.Tests.StructuralAnalysis.Unit.DirectStiffnessMethod;
using Microsoft.Extensions.DependencyInjection;

namespace BeamOs.Tests.Runtime.TestRunner;

public static class DI
{
    public static IServiceCollection AddTestServices(this IServiceCollection services)
    {
        // services.AddScoped<OpenSeesTests>();
        services.AddScoped<DsmElement1dTests>();
        services.AddScoped<DsmModelTests>();
        services.AddScoped<TestInfoRetriever>();

        services.AddSingleton<ISolverFactory, CholeskySolverFactory>();
        services.AddInMemoryInfrastructure();

        return services;
    }
}
