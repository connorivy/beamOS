using BeamOs.Infrastructure;

namespace BeamOs.Api;

public static class DependencyInjectionConsolidated
{
    public static IServiceCollection AddRequiredAnalysisServices(this IServiceCollection services)
    {
        services
            .AddAnalysisEndpoints()
            .AddRequiredAnalysisEndpointServices()
            .AddRequiredInfrastructureServices();
        return services;
    }

    public static async Task InitializeAnalysisDb(this WebApplication app)
    {
        using (var scope = app.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<BeamOsStructuralDbContext>();
            _ = dbContext.Database.EnsureCreated();

            if (app.Environment.IsDevelopment())
            {
                await dbContext.SeedAsync();
            }
        }
    }
}
