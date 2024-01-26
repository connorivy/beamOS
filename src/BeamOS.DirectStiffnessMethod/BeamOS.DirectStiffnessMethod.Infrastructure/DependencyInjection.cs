using BeamOS.Common.Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace BeamOS.DirectStiffnessMethod.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddDirectStiffnessMethodInfrastructure(
        this IServiceCollection services
    )
    {
        //_ = services.AddScoped<IRepository<Ana, Model>, ModelDbContextRepository>();
        //_ = services.AddScoped<IRepository<NodeId, Node>, NodeDbContextRepository>();
        //_ = services.AddScoped<IRepository<Element1DId, Element1D>, Element1dDbContextRepository>();
        //_ = services.AddSingleton<IRepository<PointLoadId, PointLoad>, InMemoryRepository<PointLoadId, PointLoad>>();

        return services;
    }
}
